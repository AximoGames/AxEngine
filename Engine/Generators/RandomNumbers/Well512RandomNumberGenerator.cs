#region usings

using System;

#endregion

namespace Aximo.Generators.RandomNumbers
{
    public class Well512RandomNumberGenerator : RandomNumberGeneratorBase
    {
        private uint state_i = 0;
        private uint[] state;

        //private static double NORM = (1.0 / 0x100000001L);
        //private static double NORM2 = 2.32830643653869628906e-10;

        private static uint M1 = 13;
        private static uint M2 = 9;
        private static uint MASK = 0xF;    // = 15

        public Well512RandomNumberGenerator()
        {
            state = new uint[16];
            var gen = new TrueRandomNumberGenerator();
            for (var i = 0; i < 16; i++)
                state[i] = gen.NextUInt();
        }

        public Well512RandomNumberGenerator(uint[] seed)
        {
            if (seed.Length != 16) throw new Exception("Seed needs 16 uint's");
            state = seed;
        }

        public Well512RandomNumberGenerator(long seed)
        {
            unchecked
            {
                Init((uint)seed);
            }
        }

        public Well512RandomNumberGenerator(int seed)
        {
            unchecked
            {
                Init((uint)seed);
            }
        }

        public Well512RandomNumberGenerator(uint seed)
        {
            Init(seed);
        }

        private void Init(uint seed)
        {
            state = new uint[16];
            var rnd = new XorShift128RandomNumberGenerator(NativeFunctions.HashInteger(seed));
            for (var i = 0; i < 16; i++)
                state[i] = NativeFunctions.HashInteger(rnd.NextUInt());
            //this.nextUInt();
        }

        public override double NextDouble()
        {
            return Next() * NativeFunctions.IntToDoubleMultiplier;
        }

        public override uint NextUInt()
        {
            unchecked
            {
                uint z0, z1, z2;
                z0 = state[(state_i + 15) & MASK];
                z1 = (state[state_i] ^ (state[state_i] << 16)) ^
                    (state[(state_i + M1) & MASK] ^ (state[(state_i + M1) & MASK] << 15));
                z2 = state[(state_i + M2) & MASK] ^
                    (state[(state_i + M2) & MASK] >> 11);
                state[state_i] = z1 ^ z2;
                state[(state_i + 15) & MASK] = (z0 ^ (z0 << 2)) ^ (z1 ^ (z1 << 18)) ^
                    (z2 << 28) ^ (state[state_i] ^
                    ((state[state_i] << 5) & 0xDA442D24));
                state_i = (state_i + 15) & MASK;

                return state[state_i];
            }
        }

        public override int Next()
        {
            unchecked
            {
                return (int)(NextUInt() & 0x7FFFFFFF);
            }
        }

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
            unchecked
            {
                // Fill up the bulk of the buffer in chunks of 4 bytes at a time.
                var i = 0;
                uint w;
                for (var bound = count - 3; i < bound;)
                {
                    // Generate 4 bytes.
                    // Increased performance is achieved by generating 4 random bytes per loop.
                    // Also note that no mask needs to be applied to zero out the higher order bytes before
                    // casting because the cast ignores thos bytes. Thanks to Stefan Troschütz for pointing this out.
                    w = NextUInt();

                    buffer[start + i++] = (byte)w;
                    buffer[start + i++] = (byte)(w >> 8);
                    buffer[start + i++] = (byte)(w >> 16);
                    buffer[start + i++] = (byte)(w >> 24);
                }

                // Fill up any remaining bytes in the buffer.
                if (i < count)
                {
                    // Generate 4 bytes.
                    w = NextUInt();

                    buffer[start + i++] = (byte)w;
                    if (i < buffer.Length)
                    {
                        buffer[start + i++] = (byte)(w >> 8);
                        if (i < buffer.Length)
                        {
                            buffer[start + i++] = (byte)(w >> 16);
                            if (i < buffer.Length)
                                buffer[start + i] = (byte)(w >> 24);
                        }
                    }
                }
            }
        }

    }

}
