using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompressorLib
{
    /// <summary>
    /// Class to count the relative frequency of each byte in a stream of data.
    /// There is no upper bound on the number of bytes that can be counted.
    /// If the counter reaches the maximum value, the class will rebase all of the frequencies to avoid overflows.
    /// </summary>
    internal class FrequencyCounter
    {
        private readonly IDictionary<byte, UInt32> moduloCounter;
        private readonly IDictionary<byte, UInt32> frequencies;
        private int multiplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrequencyCounter"/> class.
        /// </summary>
        public FrequencyCounter()
        {
            this.moduloCounter = new Dictionary<byte, UInt32>();
            this.frequencies = new Dictionary<byte, UInt32>();
            multiplier = 1;

            // Initialize the dictionaries to make the code more straightforward in the rest of the class.
            // Avoid overflow of counter that would lead to an infinite loop using an int
            // https://stackoverflow.com/questions/43800147/iterating-from-minvalue-to-maxvalue-with-overflow
            for (int i = byte.MinValue; i <= byte.MaxValue; i++)
            {
                this.moduloCounter.Add((byte)i, 0);
                this.frequencies.Add((byte)i, 0);
            }
        }

        /// <summary>
        /// Increment the frequency of a byte by 1.
        /// </summary>
        /// <param name="value">The value to increment the frequency of.</param>
        public void Increment(byte value)
        {
            // Check to see if we have reached the rare theoretic limit for frequency counting.
            // In that case, the best we can do is stop counting to avoid an overlfow back to 0.
            // Note: this would still mean having read 16 million TB of a single character!
            if (this.moduloCounter[value] == UInt32.MaxValue &&
                this.frequencies[value] == UInt32.MaxValue)
            {
                return;
            }

            this.moduloCounter[value]++;
            if (this.moduloCounter[value] == multiplier)
            {
                // If the counter is about to overflow, we call the Rebase function
                if (this.frequencies[value] == UInt32.MaxValue)
                {
                    this.Rebase();
                }
                else
                {
                    this.frequencies[value]++;
                }

                this.moduloCounter[value] = 0;
            }
        }

        /// <summary>
        /// Set the frequency of a byte to a specific value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="frequency">The frequency to set.</param>
        public void SetFrequency(byte value, UInt32 frequency)
        {
            this.frequencies[(byte)value] = frequency;
            this.moduloCounter[(byte)value] = 0;
        }

        /// <summary>
        /// Get the frequency of a byte.
        /// </summary>
        /// <param name="value">The value to query the frequency of</param>
        /// <returns>Frequency of the value.</returns>
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

        /// <summary>
        /// Enumerator to allow caller to cycle through frequencies.
        /// </summary>
        /// <returns>Key value pair enumaration of non-zero frequencies</returns>
        public IEnumerable<KeyValuePair<byte, UInt32>> GetEnumerator()
        {
            for (int b = byte.MinValue; b <= byte.MaxValue; b++)
            {
                var frequency = this.GetFrequency((byte)b);
                if (frequency > 0)
                {
                    yield return new KeyValuePair<byte, UInt32>((byte)b, frequency);
                }
            }
        }

        /// <summary>
        /// Rebases all of the frequencies uniformly by dividing them by 2
        /// This allows us to handle very high counts without risk of overflowing and still
        /// preserving statistical accuracy.
        /// </summary>
        private void Rebase()
        {
            this.frequencies.ToList().ForEach( kvp => this.frequencies[kvp.Key] /= 2  + 1);
            this.multiplier *= 2;
        }

    }
}
