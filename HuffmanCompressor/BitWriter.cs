namespace HuffmanCompressor
{
    /// <summary>
    /// Provides functionality to write individual bits to an output file stream.
    /// </summary>
    internal class BitWriter
    {
        private int bitIndex;
        private byte currentByte;
        private readonly FileStream fileHandle;

        /// <summary>
        /// Creates an instance of <see cref="BitWriter"/> class
        /// </summary>
        /// <param name="fileHandle">File stream to which the writer will write bits to</param>
        public BitWriter(FileStream fileHandle) 
        {
            ArgumentNullException.ThrowIfNull(fileHandle);
            this.bitIndex = 0;
            this.currentByte = 0x00;
            this.fileHandle = fileHandle;
        }

        /// <summary>
        /// Writes bits to the output file
        /// </summary>
        /// <param name="bitString">String of bits, only 0 and 1 are supported</param>
        /// <exception cref="ArgumentException">In case the bit string contains unsupported characters</exception>
        public void WriteBits(string bitString)
        {
            ArgumentNullException.ThrowIfNull(bitString);
            ArgumentNullException.ThrowIfNull(fileHandle);

            // Fill in currentByte from most significant bit to least significant bit by reading from input bitString
            // until the input bitString is exhausted.
            // As we fill out all available bits in currentByte, we flush it out to the file and start over with a fresh byte
            for (int i = 0; i < bitString.Length; i++) 
            {
                var bit = bitString[i];
                switch(bit)
                {
                    case '0':
                        break;
                    case '1':
                        // mask a '1' on the current byte
                        byte mask = 0x80;
                        mask = (byte)(mask >> bitIndex);
                        currentByte |= mask;
                        break;
                    default:
                        throw new ArgumentException($"Invalid bit detected {bit}");
                }

                if (bitIndex == 7)
                {
                    // currentByte has been exhausted for all of its 8 available bits
                    // commit to disk and mint a new byte to continue the process.
                    fileHandle.WriteByte(currentByte);
                    bitIndex = 0;
                    currentByte = 0;
                }
                else
                {
                    bitIndex++;
                }
            }
        }

        /// <summary>
        /// Write the outstanding byte to disk, prior to closing the file handle.
        /// The remaining bits will be padding.
        /// </summary>
        public void Flush()
        {
            fileHandle.WriteByte(currentByte);
        }
    }
}
