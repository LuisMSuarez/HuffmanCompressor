namespace HuffmanCompressor.Lib;
public interface IBitWriter
{
    void Initialize(FileStream fileHandle);
    void Flush();
    void WriteBits(string bitString);
}