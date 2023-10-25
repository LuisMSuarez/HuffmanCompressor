namespace HuffmanCompressor
{
    /// <summary>
    /// Provides functionality to read individual bits from an input file stream.
    /// </summary>
    internal class BitReader
    {
        private int bitIndex;
        private byte currentByte;
        private readonly FileStream fileHandle;

        /// <summary>
        /// Creates an instance of <see cref="BitReader"/> class
        /// </summary>
        /// <param name="fileHandle">File stream to which the reader will read bits from</param>
        public BitReader(FileStream fileHandle)
        {
            ArgumentNullException.ThrowIfNull(fileHandle);
            this.bitIndex = 8;
            this.currentByte = 0x00;
            this.fileHandle = fileHandle;
        }

        /// <summary>
        /// Writes bits to the output file
        /// </summary>
        /// <param name="bitString">String of bits, only 0 and 1 are supported</param>
        /// <exception cref="ArgumentException">In case the bit string contains unsupported characters</exception>
        public char ReadNextBit()
        {
            // Read 1 byte from the input file stream at a time, and use an index to return each bit
            // When we get to the last bit, read the next byte and continue until end of file
            if (this.bitIndex >= 8)
            {
                this.currentByte = (byte)this.fileHandle.ReadByte();
                this.bitIndex = 0;
            }

            byte mask = (byte)(0x80 >> this.bitIndex++);
            return (this.currentByte & mask) == 0x00
                ? '0' 
                : '1';
        }
    }
}
