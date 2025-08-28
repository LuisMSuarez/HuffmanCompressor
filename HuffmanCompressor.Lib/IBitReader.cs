namespace HuffmanCompressor.Lib;
public interface IBitReader
{
    void Initialize(FileStream fileHandle);
    char ReadNextBit();
}