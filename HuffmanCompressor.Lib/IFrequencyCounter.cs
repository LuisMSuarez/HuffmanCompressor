
namespace HuffmanCompressor.Lib;
public interface IFrequencyCounter
{
    IEnumerable<KeyValuePair<byte, uint>> GetEnumerator();
    uint GetFrequency(byte value);
    void Increment(byte value);
    void SetFrequency(byte value, uint frequency);
}