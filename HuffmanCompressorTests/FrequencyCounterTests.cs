namespace HuffmanCompressorTests
{
    using HuffmanCompressorLib;

    public class FrequencyCounterTests
    {
        [Fact]
        public void AddItemTest()
        {
            // Arrange
            var counter = new FrequencyCounter();

            // Act
            counter.Increment((byte)'a');

            // Assert
            Assert.Equal((UInt32)1, counter.GetFrequency((byte)'a'));
            Assert.Equal((UInt32)0, counter.GetFrequency((byte)'b'));
        }

        [Fact]
        public void SetFrequencyTest()
        {
            // Arrange
            var counter = new FrequencyCounter();

            // Act
            counter.SetFrequency((byte)'a',23);

            // Assert
            Assert.Equal((UInt32)23, counter.GetFrequency((byte)'a'));
            Assert.Equal((UInt32)0, counter.GetFrequency((byte)'b'));
        }

        [Fact]
        public void GetEnumeratorTest()
        {
            // Arrange
            var counter = new FrequencyCounter();

            // Act
            counter.SetFrequency((byte)'a', 23);

            // Assert
            Assert.Single(counter.GetEnumerator());
            foreach ( var kvp in counter.GetEnumerator())
            {
                Assert.Equal((byte)'a', kvp.Key);
                Assert.Equal((UInt32)23, kvp.Value);
            }
        }

        [Fact]
        public void IncrementOverflowTest()
        {
            // Arrange
            var counter = new FrequencyCounter();

            // Act
            counter.SetFrequency((byte)'a', UInt32.MaxValue);

            // Assert
            Assert.Single(counter.GetEnumerator());
            Assert.Equal(UInt32.MaxValue, counter.GetFrequency((byte)'a'));

            // Act
            counter.Increment((byte)'a');

            // We don't check for a specific value, we allow the counter to use it's own method of rebasing the count,
            // just ensure it didn't wrap back to 0
            Assert.True(counter.GetFrequency((byte)'a') > 0);

        }
    }
}
