namespace HuffmanCompressorTests
{
    using HuffmanCompressor;
    using System.Reflection;

    public class EndToEndTests
    {
        private const string TestDataFolderName = "TestData";

        [Fact]
        void CompressSampleFile()
        {
            // Arrange
            // Note: Use unique output file name to ensure no collision if tests run in parallel.
            var outputFilePath = GetTestPath(@"outputSmallFile.bin");

            // Act
            Program.Main(["compress", GetTestPath("SmallFile.txt"), outputFilePath]);

            // Assert
            Assert.True(File.Exists(outputFilePath));
        }

        private static string GetTestPath(string relativePath)
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return Path.Combine(dirPath!, TestDataFolderName, relativePath);
        }
    }
}
