namespace HuffmanCompressorTests
{
    using HuffmanCompressorLib;

    public class HuffmanCompressorTests
    {
        private const string TestDataFolderName = "TestData";

        [Fact]
        public void ConstructorTest()
        {
            // Arrange & Act
            var compressor = new HuffmanCompressor();

            // Assert
            Assert.NotNull(compressor);
        }

        [Theory]
        [InlineData("smallfile.txt")]
        [InlineData("emptyfile.txt")]
        [InlineData("singleCharacter.txt")]
        [InlineData("wordFile.docx")]
        public void CompressTest(string fileName)
        {
            // Arrange
            var inputFilePath = Utilities.GetTestPath(fileName);
            var compressedFilePath = Utilities.GetTestPath(fileName + ".huf");

            var compressor = new HuffmanCompressor();

            // Act
            compressor.Compress(inputFilePath, compressedFilePath);

            // Assert
            Assert.True(File.Exists(compressedFilePath));
        }

        [Theory]
        [InlineData("smallfile.txt")]
        [InlineData("emptyfile.txt")]
        [InlineData("singleCharacter.txt")]
        [InlineData("wordFile.docx")]
        public void InflateTest(string fileName)
        {
            // Arrange
            var inputFilePath = Utilities.GetTestPath(fileName);
            var compressedFilePath = Utilities.GetTestPath(fileName + ".huf");
            var inflatedFilePath = Utilities.GetTestPath(compressedFilePath + ".inf");

            var compressor = new HuffmanCompressor();

            // Act
            compressor.Compress(inputFilePath, compressedFilePath);

            // Assert
            Assert.True(File.Exists(compressedFilePath));

            // Act
            compressor.Inflate(compressedFilePath, inflatedFilePath);

            // Assert
            Assert.True(File.Exists(inflatedFilePath));
        }
    }
}
