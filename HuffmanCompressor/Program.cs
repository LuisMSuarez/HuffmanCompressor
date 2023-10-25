namespace HuffmanCompressor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const string usageString = "Usage: [compress|inflate] [input file path] [output file path]";

            if (args.Length != 3)
            {
                Console.WriteLine(usageString);
                return;
            }

            IFileCompressor compressor = new HuffmanCompressor();
            switch (args[0].ToLower())
            {
                case "compress":
                    compressor.Compress(args[1], args[2]);
                    break;
                case "inflate":
                    compressor.Inflate(args[1], args[2]);
                    break;
                default:
                    Console.WriteLine(usageString);
                    break;
            }
        }
    }
}