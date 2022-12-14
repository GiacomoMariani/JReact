using System;
using Unity.Burst;
using UnityEngine.Assertions;

namespace JReact.Random
{
    //Stress test: pcg get float: 0.000014
    //Stress test: pcg get int: 0.000011
    //Stress test: pcg get int 0 and 1: 0.000013
    //Stress test: x get float: 0.0000239
    //Stress test: x get int: 0.000020
    //Stress test: x get int 0 and 1: 0.000023
    
     /// <summary>
    /// XSH-RR: An xorshift mixes some high-order bits down, then bits 63–59 select a rotate amount to be applied to bits 27–58
    /// (64→32 bits) count = (int)(x >> 59); x ^= x >> 18; return rotr32((uint32_t)(x >> 27), count);
    /// </summary>
    [BurstCompile]
     public struct J_RandomPCG
    {
        private const ushort _MaxAttempts = 1000;
        private const ulong _Multiplier = 6364136223846793005u;
        private const ulong _Increment = 1442695040888963407u;
        private ulong _accumulator;

        /// <summary>
        /// generates the random struct using a seed
        /// </summary>
        /// <param name="seed">the seed to create the random number generator</param>
        public J_RandomPCG(ulong seed = 0x4d595df4d0f33173)
        {
            _accumulator = seed + _Increment;
            GetUInt();
        }

         private uint Rotate(uint x, int r) => (x >> r) | (x << (-r & 31));

        /// <summary>
        /// basic method to generates a uint from 0 to uint.max
        /// </summary>
        public uint GetUInt()
        {
            ulong x     = _accumulator;
            int   count = (byte)(x >> 59); // 59 = 64 - 5

            _accumulator =  x * _Multiplier + _Increment;
            x            ^= x >> 18;               // 18 = (64 - 27)/2
            return Rotate((uint)(x >> 27), count); // 27 = 32 - 5
        }

        /// <summary>
        /// generates a uint in a given range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public uint GetUInt(uint range)
        {
            //make sure to avoid the modulo bias
            var excluded   = uint.MaxValue % range;
            var validRange = uint.MaxValue - excluded;

            var attempts = 0;
            while (attempts < _MaxAttempts)
            {
                var value = GetUInt();
                if (value <= validRange) { return value % range; }

                attempts++;
            }

            return 0;
        }

        /// <summary>
        /// generates a int witn a min and a max inclusive
        /// </summary>
        /// <param name="min">the minimal result, inclusive</param>
        /// <param name="max">the max result inclusive -- max nimber is int.max -1</param>
        /// <returns>returns a random int within the given limts</returns>
        public int NextInt(int min, int max)
        {
            Assert.IsTrue(max < Int32.MaxValue, "To have it inclusive we need to add one");
            uint range = (uint)(max - min) + 1;
            var  value = (int)GetUInt(range);

            return value + min;
        }

        /// <summary>
        /// generates a float within the given range
        /// </summary>
        /// <param name="min">the minimum float</param>
        /// <param name="max">the max float</param>
        /// <returns>returns a random float within the given limts</returns>
        public float NextFloat(float min, float max)
        {
            var range = (max - min);
            var value = (float)GetUInt() / (float)(uint.MaxValue / range);

            return value + min;
        }

        public static implicit operator J_RandomPCG(ulong accumulator) => new(accumulator);

        public static implicit operator ulong(J_RandomPCG hash) => hash._accumulator;

        public override string ToString() => _accumulator.ToString();
    }
}
