namespace HuffmanCompressorTests
{
    using HuffmanCompressorCmd;

    public class EndToEndTests
    {
        [Theory]
        [InlineData("E2E-SmallFile.txt", true)]
        [InlineData("E2E-EmptyFile.txt", false)]
        [InlineData("E2E-SingleCharacter.txt", false)]
        [InlineData("E2E-WordFile.docx", false)]
        public void CompressTest(string fileName, bool verifySmallerCompressedFileSize)
        {
            // Arrange
            // Note: Use unique output file name to ensure no collision if tests run in parallel.
            var inputFilePath = Utilities.GetTestPath(fileName);
            var compressedFilePath = Utilities.GetTestPath(fileName + ".huf");
            var inflatedFilePath = Utilities.GetTestPath(inputFilePath + ".inf");

            // Act
            Program.Main(["compress", inputFilePath, compressedFilePath]);

            // Assert
            Assert.True(File.Exists(compressedFilePath));

            // Verify the compression produced a file smaller in size.
            // Note: The compressed file includes the overhead of the character frequency table
            // this means that there is a threshold for which compression will not be effective.
            // Also, files that are already compressed, such as zip files or docx files have high entropy and will not compress further
            FileInfo inputFileInfo, compressedFileInfo, inflatedFileInfo;
            inputFileInfo = new FileInfo(inputFilePath);
            compressedFileInfo = new FileInfo(compressedFilePath);
            if (verifySmallerCompressedFileSize)
            {
                Assert.True(inputFileInfo.Length > compressedFileInfo.Length);
            }

            // Act
            Program.Main(["inflate", compressedFilePath, inflatedFilePath]);
            inflatedFileInfo = new FileInfo(inflatedFilePath);

            // Assert
            Assert.Equal(inputFileInfo.Length, inflatedFileInfo.Length);
            var originalHash = Utilities.GetFileHash(fileName);
            var inflatedHash = Utilities.GetFileHash(inflatedFilePath);
            Assert.Equal(originalHash, inflatedHash);
        }
    }
}
