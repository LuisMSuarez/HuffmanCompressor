namespace HuffmanCompressorTests
{
    using HuffmanCompressorCmd;
    using HuffmanCompressorLib;
    using Moq;

    public class ProgramTests
    {
        [Fact]
        public void CompressMethodInvokedWithCompressParameterTest()
        {
            // Arrange
            var mockCompressor = new Mock<IFileCompressor>(MockBehavior.Strict);
            mockCompressor.Setup(c => c.Compress(It.IsAny<string>(), It.IsAny<string>()));
            Program.SetCompressorReference(mockCompressor.Object);

            // Act
            Program.Main(["compress", "input.txt", "output.bin" ]);

            // Assert
            mockCompressor.Verify(compressor => compressor.Compress(
                It.Is<string>(s => s.Equals("input.txt")),
                It.Is<string>(s => s.Equals("output.bin"))),
                "Expected input or output paths not found");
        }

        [Fact]
        public void InflateMethodInvokedWithCompressParameterTest()
        {
            // Arrange
            var mockCompressor = new Mock<IFileCompressor>(MockBehavior.Strict);
            mockCompressor.Setup(c => c.Inflate(It.IsAny<string>(), It.IsAny<string>()));
            Program.SetCompressorReference(mockCompressor.Object);

            // Act
            Program.Main(["inflate", "input.bin", "output.txt"]);

            // Assert
            mockCompressor.Verify(compressor => compressor.Inflate(
                It.Is<string>(s => s.Equals("input.bin")),
                It.Is<string>(s => s.Equals("output.txt"))),
                "Expected input or output paths not found");
        }

        [Fact]
        public void ArgumentExceptionThrownWithInvalidCommandTest()
        {
            // Arrange
            var mockCompressor = new Mock<IFileCompressor>(MockBehavior.Strict);
            Program.SetCompressorReference(mockCompressor.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => Program.Main(["foo", "input.txt", "output.bin"]));
        }

        [Theory]
        [InlineData("compress")]
        [InlineData("foo")]
        public void ArgumentExceptionThrownWithInvalidNumberOfParametersTest(string command)
        {
            // Arrange
            var mockCompressor = new Mock<IFileCompressor>(MockBehavior.Strict);
            Program.SetCompressorReference(mockCompressor.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => Program.Main([command]));
        }
    }
}