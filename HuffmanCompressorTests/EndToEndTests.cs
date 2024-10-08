namespace HuffmanCompressorTests
{
    using HuffmanCompressor;
    using System.Reflection;

    public class EndToEndTests
    {
        private const string TestDataFolderName = "TestData";

        [Theory]
        [InlineData("smallfile.txt", true)]
        [InlineData("emptyfile.txt", false)]
        void CompressTest(string fileName, bool verifyFileSize)
        {
            // Arrange
            // Note: Use unique output file name to ensure no collision if tests run in parallel.
            var inputFilePath = GetTestPath(fileName);
            var outputFilePath = GetTestPath(fileName + ".huf");

            // Act
            Program.Main(["compress", inputFilePath, outputFilePath]);

            // Assert
            Assert.True(File.Exists(outputFilePath));

            // Verify the compression produced a file smaller in size.
            // Note: The compressed file includes the overhead of the character frequency table
            // this means that there is a threshold for which compression will not be effective.
            if (verifyFileSize)
            {
                var inputFileInfo = new FileInfo(inputFilePath);
                var outputFileInfo = new FileInfo(outputFilePath);
                Assert.True(inputFileInfo.Length > outputFileInfo.Length);
            }

            // Act
            var inflatedFilePath = GetTestPath(inputFilePath + ".inf");
            Program.Main(["inflate", outputFilePath, inflatedFilePath]);

            // Cleanup
            File.Delete(outputFilePath);
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
