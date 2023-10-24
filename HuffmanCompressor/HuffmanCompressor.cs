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

        public HuffmanCompressor()
        {
            frequencies = new Dictionary<byte, int>(capacity: 2 ^ 8);
            mappings = new Dictionary<byte, string>(capacity: 2 ^ 8);
        }

        public void Compress(string inputFilePath, string outputFilePath)
        {
            throw new NotImplementedException();
        }

        private void InitializeFrequencyDictionary()
        {

        }
        private void SerializeFrequencyDictionary()
        {
        }

        private void CompressInput() 
        { 
        }
    }
}
