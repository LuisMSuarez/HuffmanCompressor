namespace HuffmanCompressor.Lib;

/// <summary>
/// Interface for file compression and decompression.
/// </summary>
public interface IFileCompressor
{
    void Compress(string inputFilePath, string outputFilePath);
    void Inflate(string inputFilePath, string outputFilePath);
}
