namespace HuffmanCompressorLib
{
    public interface IFileCompressor
    {
        void Compress(string inputFilePath, string outputFilePath);
        void Inflate(string inputFilePath, string outputFilePath);
    }
}
