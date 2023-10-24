using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompressor
{
    internal class HuffmanCompressor : IFileCompressor
    {
        private IDictionary<byte, int> frequencies;
        private IDictionary<byte, string> mappings;
        private Node<byte> treeRoot;

        public HuffmanCompressor()
        {
            frequencies = new Dictionary<byte, int>(capacity: 256);
            mappings = new Dictionary<byte, string>(capacity: 256);
            this.treeRoot = null;
        }

        public void Compress(string inputFilePath, string outputFilePath)
        {
            this.InitializeFrequencyDictionary(inputFilePath);
            this.BuildTree();
            this.BuildMappings();
            this.WriteOutput(inputFilePath, outputFilePath);
        }

        private void InitializeFrequencyDictionary(string inputFilePath)
        {
            // Initialize all the keys in one shot for more efficiency
            for (int i = 0; i < 256; i++)
            {
                frequencies.Add(((byte)i), 0);
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
                frequencies[(byte)inputByte]++;
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

            treeRoot = priorityQueue.Dequeue();
        }

        private void BuildMappings()
        {
            // Initialize all the keys in one shot for more efficiency
            for (int i = 0; i < 256; i++)
            {
                mappings.Add(((byte)i), string.Empty);
            }

            BuildMappings(treeRoot, string.Empty);
        }

        private void BuildMappings(Node<byte> node, string binaryCode)
        {
            if (node.IsLeafNode)
            {
                mappings[node.Value] = binaryCode;
                return;
            }

            BuildMappings(node.GetLeft(), $"{binaryCode}0");
            BuildMappings(node.GetRight(), $"{binaryCode}1");
        }

        private void SerializeFrequencyDictionary()
        {
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

            int inputByte;
            while ((inputByte = inputStream.ReadByte()) != -1)
            {
                var text = mappings[(byte)inputByte];
                byte[] info = new UTF8Encoding(true).GetBytes(text);
                outputStream.Write(info, 0, info.Length);
            }

            inputStream.Close();
            outputStream.Close();
        }
    }
}
