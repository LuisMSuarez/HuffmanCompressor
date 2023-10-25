using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompressor
{
    internal interface IFileCompressor
    {
        void Compress(string inputFilePath, string outputFilePath);
        void Inflate(string inputFilePath, string outputFilePath);
    }
}
