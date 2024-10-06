namespace HuffmanCompressor
{
    public static class Program
    {
        private static IFileCompressor _compressor;

        internal static void SetCompressorReference(IFileCompressor compressor)
        {
            _compressor = compressor;
        }

        static Program()
        {
            _compressor = new HuffmanCompressor();
        }
        
        public static void Main(string[] args)
        {
            const string usageString = "Usage: [compress|inflate] [input file path] [output file path]";

            if (args.Length != 3)
            {
                Console.WriteLine(usageString);
                return;
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