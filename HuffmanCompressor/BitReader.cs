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
        /// Creates an instance of <see cref="BitReader"/> class.
        /// </summary>
        /// <param name="fileHandle">File stream to which the reader will read bits from.</param>
        public BitReader(FileStream fileHandle)
        {
            ArgumentNullException.ThrowIfNull(fileHandle);
            this.bitIndex = 8;
            this.currentByte = 0x00;
            this.fileHandle = fileHandle;
        }

        /// <summary>
        /// Reads a bit from the file.
        /// </summary>
        /// <returns>Bit represented as a '0' or '1' character.</returns>
        /// <exception cref="EndOfStreamException">If the end of the stream is reached while attempting to read.</exception>
        public char ReadNextBit()
        {
            // Read 1 byte from the input file stream at a time, and use an index to return each bit from the byte.
            // When we get to the last bit, read the next byte and continue until end of file
            if (this.bitIndex >= 8)
            {
                var byteValue = this.fileHandle.ReadByte();
                if (byteValue == -1)
                {
                    throw new EndOfStreamException("Attempting to read past the end of the stream!");
                }
                this.currentByte = (byte)byteValue;
                this.bitIndex = 0;
            }

            byte mask = (byte)(0x80 >> this.bitIndex++);
            return (this.currentByte & mask) == 0x00
                ? '0' 
                : '1';
        }
    }
}
