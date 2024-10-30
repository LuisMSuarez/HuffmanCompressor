namespace HuffmanCompressorCmd
{
    using HuffmanCompressorLib;

    public static class Program
    {
        private static IFileCompressor _compressor;

        static Program()
        {
            _compressor = new HuffmanCompressor();
        }

        /// <summary>
        /// Internal method intended for the unit tests to be able to inject a mock interface for testing purposes.
        /// </summary>
        /// <param name="compressor">Instance of the compressor interface.</param>
        internal static void SetCompressorReference(IFileCompressor compressor)
        {
            _compressor = compressor;
        }

        public static void Main(string[] args)
        {
            const string usageString = "Usage: [compress|inflate] [input file path] [output file path]";

            if (args.Length != 3)
            {
                Console.WriteLine(usageString);
                throw new ArgumentException(usageString);
            }

            switch (args[0].ToLower())
            {
                case "compress":
                    _compressor.Compress(args[1], args[2]);
                    break;
                case "inflate":
                    _compressor.Inflate(args[1], args[2]);
                    break;
                default:
                    Console.WriteLine(usageString);
                    throw new ArgumentException(usageString);
            }
        }
    }
}