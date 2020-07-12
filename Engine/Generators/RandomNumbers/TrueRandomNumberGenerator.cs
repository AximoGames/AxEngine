#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Aximo.Generators.RandomNumbers
{

    public class TrueRandomNumberGenerator : RandomNumberGeneratorBase
    {

        public override int Next(int maxValue)
        {
            unchecked
            {
                maxValue += 1;
                return (int)(NextDouble() * maxValue);
            }
        }

        public override int Next(int minValue, int maxValue)
        {
            unchecked
            {
                maxValue += 1;
                if (minValue > maxValue)
                    throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "upperBound must be >=lowerBound");

                var num = maxValue - minValue;
                if (num < 0)
                {
                    long num2 = maxValue - minValue;
                    return ((int)((long)(NextDouble() * num2))) + minValue;
                }
                return ((int)(NextDouble() * num)) + minValue;
            }
        }

        // Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
        // with bitBufferIdx.
        private uint bitBuffer;
        private uint bitMask = 1;

        /// <summary>
        /// Generates a single random bit.
        /// This method's performance is improved by generating 32 bits in one operation and storing them
        /// ready for future calls.
        /// </summary>
        public override bool NextBool()
        {
            unchecked
            {
                if (bitMask == 1)
                {
                    // Generate 32 more bits.
                    bitBuffer = NextUInt();

                    // Reset the bitMask that tells us which bit to read next.
                    bitMask = 0x80000000;
                    return (bitBuffer & bitMask) == 0;
                }

                return (bitBuffer & (bitMask >>= 1)) == 0;
            }
        }

        public override void NextBytes(byte[] buffer)
        {
            NextBytes(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// This method is functionally equivalent to System.Random.NextBytes().
        /// </summary>
        public override void NextBytes(byte[] buffer, int start, int count)
        {
            for (var i = start; i < start + count; i++)
                buffer[i] = NextByte();
        }

        private byte[] NextByteBuffer = new byte[10000];
        private System.Security.Cryptography.RandomNumberGenerator gen;
        private int NextByteBufferIndex = 10000;
        public byte NextByte()
        {
            if (NextByteBufferIndex >= 10000)
            {
                if (gen == null)
                    gen = System.Security.Cryptography.RandomNumberGenerator.Create();
                gen.GetBytes(NextByteBuffer);
                NextByteBufferIndex = 0;
            }
            return NextByteBuffer[NextByteBufferIndex++];
        }

        public override double NextDouble()
        {
            return Next() * NativeFunctions.IntToDoubleMultiplier;
        }

        private byte[] NextBuffer = new byte[4];
        public override uint NextUInt()
        {
            NextBuffer[0] = NextByte();
            NextBuffer[1] = NextByte();
            NextBuffer[2] = NextByte();
            NextBuffer[3] = NextByte();
            return BitConverter.ToUInt32(NextBuffer, 0);
        }

        public override int Next()
        {
            unchecked
            {
                return (int)(NextUInt() & 0x7FFFFFFF);
            }
        }

    }

}
