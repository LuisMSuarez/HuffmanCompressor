using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompressor
{
    internal class HuffmanCompressor : IFileCompressor
    {
        private IDictionary<byte, UInt32> frequencies;
        private IDictionary<byte, string> binaryCodeMappings;
        private Node<byte> treeRoot;

        void IFileCompressor.Compress(string inputFilePath, string outputFilePath)
        {
            this.InitializeFrequencyDictionary(inputFilePath);
            this.BuildTree();
            this.BuildBinaryCodeMappings();
            this.Compress(inputFilePath, outputFilePath);
        }

        void IFileCompressor.Inflate(string inputFilePath, string outputFilePath)
        {
            var inputStream = this.ReadFrequencyDictionary(inputFilePath);
            this.BuildTree();
            this.BuildBinaryCodeMappings();
            this.Inflate(inputStream, outputFilePath);
        }

        private void InitializeFrequencyDictionary(string inputFilePath)
        {
            FileStream inputStream = null;
            try
            {
                inputStream = File.OpenRead(inputFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception opening file: {e.Message}");
            }

            if (inputStream == null)
            {
                throw new Exception("Null filestream!");
            }

            this.frequencies = new Dictionary<byte, UInt32>(capacity: 256);
            int inputByte;
            // -1 represents end of stream, otherwise byte cast as int
            while ((inputByte = inputStream.ReadByte()) != -1)
            {
                byte nativeByte = (byte)inputByte;
                if (!this.frequencies.ContainsKey(nativeByte))
                {
                    this.frequencies.Add(nativeByte, 1);
                }
                else
                {
                    this.frequencies[nativeByte]++;
                }
            }

            inputStream.Close();
        }

        private void BuildTree()
        {
            // Use a MinHeap priority queue, creating a node with the byte, and the frequency as priority
            var priorityQueue = new PriorityQueue<Node<byte>, UInt32>(initialCapacity: 256);
            foreach (var kvp in frequencies)
            {
                priorityQueue.Enqueue(new Node<byte>(kvp.Key), kvp.Value);
            }

            // Take the 2 nodes at the head of the queue (lowest frequency) and combine into a new node
            // The tree is complete when there is only 1 node left
            while (priorityQueue.Count > 1)
            {
                priorityQueue.TryDequeue(out var leftNode, out UInt32 leftPriority);
                priorityQueue.TryDequeue(out var rightNode, out UInt32 rightPriority);
                priorityQueue.Enqueue(new Node<byte>(leftNode, rightNode), leftPriority + rightPriority);
            }

            // Protect against case where the input file was empty, and therefore there were never any nodes in the priority queue
            if (priorityQueue.Count > 0)
            {
                this.treeRoot = priorityQueue.Dequeue();
            }
        }

        private void BuildBinaryCodeMappings()
        {
            // Base case: empty input file means there is no binary tree, so nothing to do.
            if (this.treeRoot == null)
            {
                return;
            }

            this.binaryCodeMappings = new Dictionary<byte, string>(capacity: this.frequencies.Count);

            BuildBinaryCodeMappings(this.treeRoot, string.Empty);
        }

        private void BuildBinaryCodeMappings(Node<byte> node, string binaryCode)
        {
            if (node.IsLeafNode)
            {
                this.binaryCodeMappings.Add(node.Value, binaryCode);
                return;
            }

            // Left node gets tagged with 0, right node gets tagged with 1
            this.BuildBinaryCodeMappings(node.GetLeft(), $"{binaryCode}0");
            this.BuildBinaryCodeMappings(node.GetRight(), $"{binaryCode}1");
        }

        private void Compress(string inputFilePath, string outputFilePath)
        {
            FileStream inputStream = null;
            try
            {
                inputStream = File.OpenRead(inputFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception opening input file: {e.Message}");
            }

            if (inputStream == null)
            {
                throw new Exception("Null filestream!");
            }

            FileStream outputStream = null;
            try
            {
                outputStream = File.OpenWrite(outputFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception opening output file: {e.Message}");
            }

            if (outputStream == null)
            {
                throw new Exception("Null filestream!");
            }

            this.WriteFrequencyDictionary(outputStream);

            var bitWriter = new BitWriter(outputStream);
            int inputByte;
            while ((inputByte = inputStream.ReadByte()) != -1)
            {
                bitWriter.WriteBits(this.binaryCodeMappings[(byte)inputByte]);
            }

            bitWriter.Flush();
            inputStream.Close();
            outputStream.Close();
        }

        /// <summary>
        /// Writes the frequency dictionary to the output file so that the binary tree can be rebuilt to decompress the file.
        /// As an optimization, we only write frequencies for the bytes that were present in the input file.
        /// Note, the binary code mappings could be used as an alternate way to decode the file, but it would consume more disk space than the frequencies table
        /// defeating the purpose of an efficient compression algorithm.
        /// </summary>
        /// <param name="outputStream"></param>
        private void WriteFrequencyDictionary(FileStream outputStream)
        {
            var writer = new BinaryWriter(outputStream);

            // frequency count can be 256 at most, so we need 2 bytes at most
            writer.Write((ushort)this.frequencies.Count);
            foreach (var kvp in this.frequencies)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
            writer.Flush();
        }

        private FileStream ReadFrequencyDictionary(string inputFilePath)
        {
            FileStream inputStream = null;
            try
            {
                inputStream = File.OpenRead(inputFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception opening input file: {e.Message}");
            }

            if (inputStream == null)
            {
                throw new Exception("Null filestream!");
            }

            var reader = new BinaryReader(inputStream);
            var frequencyCount = reader.ReadUInt16();
            if (frequencyCount < 0 || frequencyCount > 256)
            {
                throw new ArgumentOutOfRangeException($"Invalid frequency count {frequencyCount} in input file");
            }

            this.frequencies = new Dictionary<byte, UInt32>(capacity: frequencyCount);
            for (int i = 0; i < frequencyCount; i++)
            {
                var key = reader.ReadByte();
                var value = reader.ReadUInt32();
                this.frequencies.Add(key, value);
            }

            // Hand off the input stream to the next steps of decompression so they continue reading after the frequency dictionary header
            return inputStream;
        }

        private void Inflate(FileStream inputStream, string outputFilePath)
        {
            FileStream outputStream = null;
            try
            {
                outputStream = File.OpenWrite(outputFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception opening output file: {e.Message}");
            }

            if (outputStream == null)
            {
                throw new Exception("Null filestream!");
            }

            // For decompression, we key off binary codes to obtain the corresponding byte, which is the opposite to what we do in compression.
            // This ensures that lookup for decompression has constant time complexity.
            var reverseBinaryCodeMappings = this.binaryCodeMappings.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var bitReader = new BitReader(inputStream);
            var numBytes = this.frequencies.Values.Aggregate((a, b) => a + b);
            for (int i = 0; i < numBytes; i++)
            {
                var bitString = string.Empty;
                var binaryCodeMatch = false;
                while (!binaryCodeMatch)
                {
                    var bit = bitReader.ReadNextBit();
                    bitString = $"{bitString}{bit}";
                    if (reverseBinaryCodeMappings.ContainsKey(bitString))
                    {
                        // A matching pattern in the binary code mappings is found, write the corresponding byte to the output
                        outputStream.WriteByte(reverseBinaryCodeMappings[bitString]);
                        binaryCodeMatch = true;
                    }
                }
            }

            outputStream.Close();
            inputStream.Close();
        }
    }
}