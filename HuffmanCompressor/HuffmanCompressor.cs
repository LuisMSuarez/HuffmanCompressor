using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompressor
{
    internal class HuffmanCompressor : IFileCompressor
    {
        private readonly IDictionary<byte, int> frequencies;
        private readonly IDictionary<byte, string> mappings;
        private int numberDistinctBytes;
        private Node<byte> treeRoot;

        public HuffmanCompressor()
        {
            this.frequencies = new Dictionary<byte, int>(capacity: 256);
            this.mappings = new Dictionary<byte, string>(capacity: 256);
            this.numberDistinctBytes = 0;
            this.treeRoot = null;
        }

        void IFileCompressor.Compress(string inputFilePath, string outputFilePath)
        {
            this.InitializeFrequencyDictionary(inputFilePath);
            this.BuildTree();
            this.BuildBinaryCodeMappings();
            this.WriteOutput(inputFilePath, outputFilePath);
        }

        void IFileCompressor.Decompress(string inputFilePath, string outputFilePath)
        {
            throw new NotImplementedException();
        }

        private void InitializeFrequencyDictionary(string inputFilePath)
        {
            // Initialize all the keys in one shot for more efficiency
            for (int i = 0; i < 256; i++)
            {
                this.frequencies.Add(((byte)i), 0);
            }

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

            int inputByte;
            while ((inputByte = inputStream.ReadByte()) != -1)
            {
                this.frequencies[(byte)inputByte]++;
            }

            inputStream.Close();
        }

        private void BuildTree()
        {
            // Use a MinHeap priority queue, creating a node with the byte, and the frequency as priority
            var priorityQueue = new PriorityQueue<Node<byte>, int>(initialCapacity: 256);
            foreach (var kvp in frequencies)
            {
                if (kvp.Value > 0)
                {
                    this.numberDistinctBytes++;
                    priorityQueue.Enqueue(new Node<byte>(kvp.Key), kvp.Value);
                }
            }

            // Take the 2 nodes at the head of the queue (lowest frequency) and combine into a new node
            // The tree is complete when there is only 1 node left
            while (priorityQueue.Count > 1)
            {
                priorityQueue.TryDequeue(out var leftNode, out int leftPriority);
                priorityQueue.TryDequeue(out var rightNode, out int rightPriority);
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
            
            // Initialize all the keys in one shot for more efficiency
            for (int i = 0; i < 256; i++)
            {
                this.mappings.Add(((byte)i), string.Empty);
            }

            BuildBinaryCodeMappings(this.treeRoot, string.Empty);
        }

        private void BuildBinaryCodeMappings(Node<byte> node, string binaryCode)
        {
            if (node.IsLeafNode)
            {
                this.mappings[node.Value] = binaryCode;
                return;
            }

            // Left node gets tagged with 0, right node gets tagged with 1
            BuildBinaryCodeMappings(node.GetLeft(), $"{binaryCode}0");
            BuildBinaryCodeMappings(node.GetRight(), $"{binaryCode}1");
        }

        private void WriteOutput(string inputFilePath, string outputFilePath)
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
                bitWriter.WriteBits(this.mappings[(byte)inputByte]);
            }

            bitWriter.Flush();
            inputStream.Close();
            outputStream.Close();
        }

        /// <summary>
        /// Writes the frequency dictionary to the output file so that the binary tree can be rebuilt to decompress the file.
        /// As an optimization, we only write frequencies for the bytes that were present in the input file.
        /// </summary>
        /// <param name="outputStream"></param>
        private void WriteFrequencyDictionary(FileStream outputStream)
        {
            var writer = new BinaryWriter(outputStream);
            writer.Write(numberDistinctBytes);
            foreach (var kvp in frequencies)
            {
                if (kvp.Value > 0)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
            writer.Flush();
        }
    }
}
