namespace HuffmanCompressorTests
{
    using HuffmanCompressorCmd;

    public class EndToEndTests
    {
        [Theory]
        [InlineData("smallfile.txt", true)]
        [InlineData("emptyfile.txt", false)]
        [InlineData("singleCharacter.txt", false)]
        [InlineData("wordFile.docx", false)]
        void CompressTest(string fileName, bool verifySmallerCompressedFileSize)
        {
            // Arrange
            // Note: Use unique output file name to ensure no collision if tests run in parallel.
            var inputFilePath = Utilities.GetTestPath(fileName);
            var compressedFilePath = Utilities.GetTestPath(fileName + ".huf");
            var inflatedFilePath = Utilities.GetTestPath(inputFilePath + ".inf");

            // Best-effor attempt to clean up temporary files from previous run
            // We do it here instead of at the end of the test to avoid pesky race
            // conditions that happen if we try to delete a file that was just created
            try
            {
                File.Delete(compressedFilePath);
                File.Delete(inflatedFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

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
