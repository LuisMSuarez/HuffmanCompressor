using HuffmanCompressorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompressorTests
{
    public class FrequencyCounterTests
    {
        [Fact]
        void AddItemTest()
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
        void SetFrequencyTest()
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
        void GetEnumeratorTest()
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
    }
}
