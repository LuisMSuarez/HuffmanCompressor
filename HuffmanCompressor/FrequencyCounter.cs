using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompressorLib
{
    internal class FrequencyCounter
    {
        private readonly IDictionary<byte, UInt32> moduloCounter;
        private readonly IDictionary<byte, UInt32> frequencies;
        private int multiplier;

        public FrequencyCounter()
        {
            this.moduloCounter = new Dictionary<byte, UInt32>();
            this.frequencies = new Dictionary<byte, UInt32>();
            multiplier = 1;

            // Initialize the dictionaries to make the code more straightforward in the rest of the class.
            for (var i = byte.MinValue; i <= byte.MaxValue; i++)
            {
                this.moduloCounter.Add(i, 0);
                this.frequencies.Add(i, 0);
                if (i == byte.MaxValue)
                {
                    break;
                }
            }
        }

        public void Increment(byte value)
        {
            this.moduloCounter[value]++;
            if (this.moduloCounter[value] == multiplier)
            {
                this.frequencies[value]++;
                this.moduloCounter[value] = 0;
            }

            // If the counter is about to overflow, we call the Rebase function
            if (this.frequencies[value] == UInt32.MaxValue)
            {
                this.Rebase();
            }
        }

        public void SetFrequency(byte value, UInt32 frequency)
        {
            this.frequencies[(byte)value] = frequency;
        }

        public UInt32 GetFrequency(byte value)
        {
            // First check for the case that the frequency counter has not yet been
            // bumped up, in that case, if we ever counted the value in the modulo counter, we must return
            // the smallest value possible, which is 1.
            if (this.frequencies[value] == 0)
            {
                if (this.moduloCounter[value] > 0)
                {
                    return 1;
                }

                return 0;
            }

            return this.frequencies[value];
        }

        // Returns non-zero frequencies
        public IEnumerable<KeyValuePair<byte, UInt32>> GetEnumerator()
        {
            for (byte b = byte.MinValue; b <= byte.MaxValue; b++)
            {
                var frequency = this.GetFrequency(b);
                if (frequency > 0)
                {
                    yield return new KeyValuePair<byte, UInt32>(b, frequency);
                }
                if (b == byte.MaxValue)
                {
                    break;
                }
            }
        }

        private void Rebase()
        {
            this.frequencies.ToList().ForEach( kvp => this.frequencies[kvp.Key] /= 2  + 1);
            this.multiplier *= 2;
        }

    }
}
