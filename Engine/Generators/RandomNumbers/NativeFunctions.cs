#region usings

using System;

#endregion

namespace Aximo.Generators.RandomNumbers
{
    internal class NativeFunctions
    {
        public const double IntToDoubleMultiplier = 1.0 / (int.MaxValue + 1.0);
        public const double UIntToDoubleMultiplier = 1.0 / (uint.MaxValue + 1.0);

        public static int RotateLeft(int value, int count)
        {
            unchecked
            {
                return (int)RotateLeft((uint)value, count);
            }
        }

        public static uint RotateLeft(uint value, int count)
        {
            unchecked
            {
                return (value << count) | (value >> (32 - count));
            }
        }

        public static ulong RotateLeft(ulong value, int count)
        {
            unchecked
            {
                return (value << count) | (value >> (64 - count));
            }
        }

        public static long RotateLeft(long value, int count)
        {
            unchecked
            {
                return (long)RotateLeft((ulong)value, count);
            }
        }

        public static int RotateRight(int value, int count)
        {
            unchecked
            {
                return (int)RotateRight((uint)value, count);
            }
        }
        public static uint RotateRight(uint value, int count)
        {
            unchecked
            {
                return (value >> count) | (value << (32 - count));
            }
        }
        public static ulong RotateRight(ulong value, int count)
        {
            unchecked
            {
                return (value >> count) | (value << (64 - count));
            }
        }

        public static long RotateRight(long value, int count)
        {
            unchecked
            {
                return (long)RotateRight((ulong)value, count);
            }
        }

        public static int HashInteger(int a)
        {
            unchecked
            {
                return (int)HashInteger((uint)a);
            }
        }

        public static int HashInteger(int a, int b)
        {
            unchecked
            {
                if (b == 0) return (int)HashInteger((uint)a);
                return HashInteger(Mix(a, b));
            }
        }

        public static int HashInteger(int a, int b, int c)
        {
            unchecked
            {
                if (b == 0 && c == 0) return (int)HashInteger((uint)a);
                return HashInteger(Mix(a, b, c));
            }
        }

        public static int HashInteger(int a, int b, int c, int d)
        {
            unchecked
            {
                if (b == 0 && c == 0 && d == 0) return (int)HashInteger((uint)a);
                return HashInteger(HashInteger(Mix(a, b, c), d));
            }
        }

        public static int HashInteger(int a, int b, int c, int d, int e)
        {
            unchecked
            {
                if (b == 0 && c == 0 && d == 0 && e == 0) return (int)HashInteger((uint)a);
                return HashInteger(HashInteger(Mix(a, b, c), d, e));
            }
        }

        public static int GetRandomNumberOfInteger(int value)
        {
            return HashInteger(value);
        }

        public static int GetRandomNumberOfInteger(int value, int version)
        {
            return HashInteger(value, version);
        }

        public static int GetRandomNumberOfInteger(int value, int version, int maxValue)
        {
            unchecked
            {
                maxValue += 1;
                var d = (HashInteger(value, version) & 0x7FFFFFFF) * IntToDoubleMultiplier;
                return (int)(d * maxValue);
            }
        }

        public static int GetRandomNumberOfInteger(int value, int version, int minValue, int maxValue)
        {
            unchecked
            {
                maxValue += 1;
                if (minValue > maxValue)
                    throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "upperBound must be >=lowerBound");

                var d = (HashInteger(value, version) & 0x7FFFFFFF) * IntToDoubleMultiplier;

                var num = maxValue - minValue;
                if (num < 0)
                {
                    long num2 = maxValue - minValue;
                    return ((int)((long)(d * num2))) + minValue;
                }
                return ((int)(d * num)) + minValue;
            }
        }

        /// <summary>
        /// https://burtleburtle.net/bob/hash/integer.html, by Thomas Wang
        /// </summary>
        public static uint HashInteger(uint a)
        {
            unchecked
            {
                a = (a + 0x7ed55d16) + (a << 12);
                a = (a ^ 0xc761c23c) ^ (a >> 19);
                a = (a + 0x165667b1) + (a << 5);
                a = (a + 0xd3a2646c) ^ (a << 9);
                a = (a + 0xfd7046c5) + (a << 3);
                a = (a ^ 0xb55a4f09) ^ (a >> 16);
                return a;
            }
        }

        public static uint HashLarson(int a)
        {
            unchecked
            {
                return HashLarson(BitConverter.GetBytes(a), 4);
            }
        }

        public static uint HashLarson(byte[] key, uint len)
        {
            unchecked
            {
                uint hash = 0;
                for (uint i = 0; i < len; ++i)
                    hash = (101 * hash) + key[i];
                return hash;
            }
        }

        public static long Combine2IntToInt64(int high, int low)
        {
            unchecked
            {
                return high << 32 | low;
            }
        }

        public static ulong Combine2IntToInt64(uint high, uint low)
        {
            unchecked
            {
                return high << 32 | low;
            }
        }

        public static int Hash6432shift(long key)
        {
            unchecked
            {
                key = (key << 18) - key - 1;
                key = key ^ RotateRight(key, 31);
                key = (key + (key << 2)) + (key << 4);
                key = key ^ RotateRight(key, 11);
                key = key + (key << 6);
                key = key ^ RotateRight(key, 22);
                return (int)key;
            }
        }

        public static int Mix(int a, int b)
        {
            unchecked
            {
                return (int)Mix((uint)a, (uint)b);
            }
        }

        public static uint Mix(uint a, uint b)
        {
            unchecked
            {
                var c = 0x9e3779b9;
                return Mix(a, b, c);
            }
        }

        public static int Mix(int a, int b, int c)
        {
            unchecked
            {
                return (int)Mix((uint)a, (uint)b, (uint)c);
            }
        }

        /// <summary>
        /// Robert Jenkins' 96 bit Mix Function
        /// </summary>
        public static uint Mix(uint a, uint b, uint c)
        {
            unchecked
            {
                a = a - b; a = a - c; a = a ^ RotateRight(c, 13);
                b = b - c; b = b - a; b = b ^ (a << 8);
                c = c - a; c = c - b; c = c ^ RotateRight(b, 13);
                a = a - b; a = a - c; a = a ^ RotateRight(c, 12);
                b = b - c; b = b - a; b = b ^ (a << 16);
                c = c - a; c = c - b; c = c ^ RotateRight(b, 5);
                a = a - b; a = a - c; a = a ^ RotateRight(c, 3);
                b = b - c; b = b - a; b = b ^ (a << 10);
                c = c - a; c = c - b; c = c ^ RotateRight(b, 15);
                return c;
            }
        }
    }

}
