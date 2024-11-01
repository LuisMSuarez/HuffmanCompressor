namespace HuffmanCompressorTests
{
    using System.IO.Hashing;
    using System.Reflection;
    using System.Text;

    public static class Utilities
    {
        public const string TestDataFolderName = "TestData";

        public static string GetTestPath(string relativePath)
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return Path.Combine(dirPath!, TestDataFolderName, relativePath);
        }

        public static string GetFileHash(string fileName)
        {
            using (FileStream fileStream = File.OpenRead(GetTestPath(fileName)))
            {
                var crc32 = new Crc32();
                crc32.Append(fileStream);
                return Encoding.UTF8.GetString(crc32.GetCurrentHash());
            }
        }
    }
}
