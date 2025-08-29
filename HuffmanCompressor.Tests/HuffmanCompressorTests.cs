namespace HuffmanCompressor.Tests;
using HuffmanCompressor.Lib;
using Moq;

public class HuffmanCompressorTests
{
    private const string TestDataFolderName = "TestData";

    [Fact]
    public void ConstructorTest()
    {
        // Arrange & Act
        var compressor = new HuffmanCompressor(Mock.Of<IFrequencyCounter>(), Mock.Of<IBitReader>(), Mock.Of<IBitWriter>());

        // Assert
        Assert.NotNull(compressor);
    }

    [Fact]
    public void CompressThrowsExceptionForWhitespaceInputFileName()
    {
        // Arrange
        var compressor = new HuffmanCompressor(Mock.Of<IFrequencyCounter>(), Mock.Of<IBitReader>(), Mock.Of<IBitWriter>());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => compressor.Compress(string.Empty, "outputFile.bin"));
    }

    [Fact]
    public void CompressThrowsExceptionForWhitespaceOutputFileName()
    {
        // Arrange
        var compressor = new HuffmanCompressor(Mock.Of<IFrequencyCounter>(), Mock.Of<IBitReader>(), Mock.Of<IBitWriter>());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => compressor.Compress("inputFile.txt", string.Empty));
    }

    [Fact]
    public void InflateThrowsExceptionForWhitespaceInputFileName()
    {
        // Arrange
        var compressor = new HuffmanCompressor(Mock.Of<IFrequencyCounter>(), Mock.Of<IBitReader>(), Mock.Of<IBitWriter>());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => compressor.Inflate(string.Empty, "outputFile.bin"));
    }

    [Fact]
    public void InflateThrowsExceptionForWhitespaceOutputFileName()
    {
        // Arrange
        var compressor = new HuffmanCompressor(Mock.Of<IFrequencyCounter>(), Mock.Of<IBitReader>(), Mock.Of<IBitWriter>());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => compressor.Inflate("inputFile.huf", string.Empty));
    }

    [Theory]
    [InlineData("Smallfile.txt")]
    [InlineData("Emptyfile.txt")]
    [InlineData("SingleCharacter.txt")]
    [InlineData("WordFile.docx")]
    public void CompressAndInflateTest(string fileName)
    {
        // Arrange
        var inputFilePath = Utilities.GetTestPath(fileName);
        var compressedFilePath = Utilities.GetTestPath(fileName + ".huf");
        var inflatedFilePath = Utilities.GetTestPath(compressedFilePath + ".inf");

        var compressor = new HuffmanCompressor(new FrequencyCounter(), new BitReader(), new BitWriter());

        // Act
        compressor.Compress(inputFilePath, compressedFilePath);

        // Assert
        Assert.True(File.Exists(compressedFilePath));

        // Act
        compressor.Inflate(compressedFilePath, inflatedFilePath);

        // Assert
        Assert.True(File.Exists(inflatedFilePath));
        var inputFileInfo = new FileInfo(inputFilePath);
        var inflatedFileInfo = new FileInfo(inflatedFilePath);
        Assert.Equal(inputFileInfo.Length, inflatedFileInfo.Length);

        var originalHash = Utilities.GetFileHash(fileName);
        var inflatedHash = Utilities.GetFileHash(inflatedFilePath);
        Assert.Equal(originalHash, inflatedHash);
    }
}
