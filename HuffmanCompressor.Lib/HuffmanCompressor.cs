namespace HuffmanCompressor.Lib;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class that implements the Huffman compression algorithm.
/// </summary>
public class HuffmanCompressor : IFileCompressor
{
    private IFrequencyCounter frequencyCounter;
    private IDictionary<short, string> binaryCodeMappings;
    private Node<short>? treeRoot;

    /// <summary>
    /// Code to mark End of File character.  Must not be a confused with a regular byte 0 to 255.
    /// </summary>
    private const short EndOfFileCode = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuffmanCompressor"/> class.
    /// </summary>
    public HuffmanCompressor(IFrequencyCounter frequencyCounter)
    {
        this.frequencyCounter = frequencyCounter ?? throw new ArgumentNullException(nameof(frequencyCounter));
        this.binaryCodeMappings = new Dictionary<short, string>();
    }

    /// <summary>
    /// Compresses a file.
    /// </summary>
    /// <param name="inputFilePath">File to be compressed.</param>
    /// <param name="outputFilePath">Path of destination (compressed) file.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="inputFilePath" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="outputFilePath" /> is <see langword="null" />.</exception>

    public void Compress(string inputFilePath, string outputFilePath)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(inputFilePath);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(outputFilePath);

        this.InitializeFrequencyDictionary(inputFilePath);
        this.BuildTree();
        this.BuildBinaryCodeMappings();
        this.CompressInternal(inputFilePath, outputFilePath);
    }

    /// <summary>
    /// Inflates a file.
    /// </summary>
    /// <param name="inputFilePath">File to be inflated.</param>
    /// <param name="outputFilePath">Path of destination (inflated) file.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="inputFilePath" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="outputFilePath" /> is <see langword="null" />.</exception>

    public void Inflate(string inputFilePath, string outputFilePath)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(inputFilePath);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(outputFilePath);

        var inputStream = this.ReadFrequencyDictionary(inputFilePath);
        this.BuildTree();
        this.BuildBinaryCodeMappings();
        this.InflateInternal(inputStream, outputFilePath);
    }

    private void InitializeFrequencyDictionary(string inputFilePath)
    {
        this.frequencyCounter = new FrequencyCounter();
        using (var inputStream = File.OpenRead(inputFilePath))
        {
            int inputByte;
            // -1 represents end of stream, otherwise byte cast as int
            while ((inputByte = inputStream.ReadByte()) != -1)
            {
                byte nativeByte = (byte)inputByte;
                this.frequencyCounter.Increment(nativeByte);
            }
        }
    }

    private void BuildTree()
    {
        // Use a MinHeap priority queue, creating a node with the byte, and the frequency as priority
        var priorityQueue = new PriorityQueue<Node<short>, UInt32>();
        foreach (var kvp in frequencyCounter.GetEnumerator())
        {
            priorityQueue.Enqueue(new Node<short>(kvp.Key), kvp.Value);
        }

        // Enque special "end of file" node with value -1 and priority 0. This special marker will appear at the end of the file so that during
        // decompression we deterministically know we have exhausted the input file.  This allows us to compress arbitrarily large input files without having to save in
        // the file header the number of bytes in the input, which could overflow for very large files.
        priorityQueue.Enqueue(new Node<short>(EndOfFileCode), 0);

        // Take the 2 nodes at the head of the queue (lowest frequency) and combine into a new node
        // The tree is complete when there is only 1 node left
        while (priorityQueue.Count > 1)
        {
            priorityQueue.TryDequeue(out var leftNode, out UInt32 leftPriority);
            priorityQueue.TryDequeue(out var rightNode, out UInt32 rightPriority);
            priorityQueue.Enqueue(new Node<short>(leftNode!, rightNode!), leftPriority + rightPriority);
        }

        this.treeRoot = priorityQueue.Dequeue();
    }

    private void BuildBinaryCodeMappings()
    {
        this.binaryCodeMappings = new Dictionary<short, string>();
        BuildBinaryCodeMappings(this.treeRoot!, string.Empty);
    }

    private void BuildBinaryCodeMappings(Node<short> node, string binaryCode)
    {
        if (node.IsLeafNode)
        {
            this.binaryCodeMappings!.Add(node.Value, binaryCode);
            return;
        }

        // Left node gets tagged with 0, right node gets tagged with 1
        this.BuildBinaryCodeMappings(node.GetLeft()!, $"{binaryCode}0");
        this.BuildBinaryCodeMappings(node.GetRight()!, $"{binaryCode}1");
    }

    private void CompressInternal(string inputFilePath, string outputFilePath)
    {
        using (var inputStream = File.OpenRead(inputFilePath))
        {
            // Open a filestream to the destination (compressed) file.  If the file already exists, it will be overwritten.
            using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                this.WriteFrequencyDictionary(outputStream);

                var bitWriter = new BitWriter(outputStream);

                // Main loop to encode each byte in the input stream according to its binary code mapping
                int inputByte;
                while ((inputByte = inputStream.ReadByte()) != -1)
                {
                    bitWriter.WriteBits(this.binaryCodeMappings![(byte)inputByte]);
                }

                // Write End of file code at the very end.
                bitWriter.WriteBits(this.binaryCodeMappings![EndOfFileCode]);

                bitWriter.Flush();
            }
        }
    }

    /// <summary>
    /// Writes the frequency dictionary to the output file so that the binary tree can be rebuilt to decompress the file.
    /// As an optimization, we only write frequencies for the bytes that were present in the input file.
    /// Note: storing the binary code mappings could be used as an alternate way to decode the file, but it would consume more disk space than the frequencies table
    /// defeating the purpose of an efficient compression algorithm.  Instead, the binary code mappings will be rebuilt at time of decompression from this frequency table.
    /// </summary>
    /// <param name="outputStream"></param>
    private void WriteFrequencyDictionary(FileStream outputStream)
    {
        var writer = new BinaryWriter(outputStream);

        // frequency table size can be 256 + 1 end of file character (257) at most, so we need 2 bytes at most (ushort) on the header of size of frequency table
        writer.Write((ushort)this.frequencyCounter.GetEnumerator().Count());
        foreach (var kvp in this.frequencyCounter.GetEnumerator())
        {
            writer.Write(kvp.Key);
            writer.Write(kvp.Value);
        }
        writer.Flush();
    }

    private FileStream ReadFrequencyDictionary(string inputFilePath)
    {
        this.frequencyCounter = new FrequencyCounter();
        FileStream inputStream;
        try
        {
            inputStream = File.OpenRead(inputFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception opening input file: {e.Message}");
            throw;
        }

        var reader = new BinaryReader(inputStream);
        var frequencyTableSize = reader.ReadUInt16();
        // 256 = all possible 8 bit characters
        if (frequencyTableSize < 0 || frequencyTableSize > 256)
        {
            throw new ArgumentOutOfRangeException($"Invalid frequency count {frequencyTableSize} in input file");
        }

        for (int i = 0; i < frequencyTableSize; i++)
        {
            var key = reader.ReadByte();
            var value = reader.ReadUInt32();
            this.frequencyCounter.SetFrequency(key, value);
        }

        // Hand off the input stream to the next steps of decompression so they continue reading after the frequency dictionary header
        return inputStream;
    }

    private void InflateInternal(FileStream inputStream, string outputFilePath)
    {
        // Open a filestream to the destination (uncompressed) file.  If the file already exists, it will be overwritten.
        using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
        {
            // Special case if input file was empty, nothing to do
            if (this.frequencyCounter.GetEnumerator().Count() == 0)
            {
                return;
            }

            // For decompression, we key off binary codes to obtain the corresponding byte, which is the opposite to what we do in compression.
            // This ensures that lookup for decompression has constant time complexity.
            var reverseBinaryCodeMappings = this.binaryCodeMappings?.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var bitReader = new BitReader(inputStream);
            var endOfFileFound = false;
            while (!endOfFileFound)
            {
                var bitString = string.Empty;
                var binaryCodeMatch = false;
                while (!binaryCodeMatch)
                {
                    var bit = bitReader.ReadNextBit();
                    bitString = $"{bitString}{bit}";
                    if (reverseBinaryCodeMappings!.ContainsKey(bitString))
                    {
                        binaryCodeMatch = true;
                        if (reverseBinaryCodeMappings![bitString] == EndOfFileCode)
                        {
                            endOfFileFound = true;
                        }
                        else
                        {
                            // A matching pattern in the binary code mappings is found, write the corresponding byte to the output
                            outputStream.WriteByte((byte)reverseBinaryCodeMappings[bitString]);
                            binaryCodeMatch = true;
                        }
                    }
                }
            }
            inputStream.Close();
        }
    }
}