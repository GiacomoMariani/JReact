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
    /// xxHash is an algorithm designed by Yann Collet
    /// further info: based on based on https://cyan4973.github.io/xxHash/ and  https://github.com/Cyan4973/xxHash/blob/dev/doc/xxhash_spec.md
    /// This is a variant of XXH32 that skips steps 2, 3, and 4 of the algorithm, and it used to create random numbers from the hash
    /// </summary>
    [BurstCompile]
    public struct J_RandomX
    {
        //a max number of attempts just to avoid overflows, in theory is just a safecheck
        private const ushort _MaxAttempts = 1000;
        // These binary prime numbers are used to manipulate bits. Chosen by Yann Collet, empirically
        private const uint _PrimeA = 0b10000101111010111100101001110111;
        private const uint _PrimeB = 0b11000010101100101010111000111101;
        private const uint _PrimeC = 0b00100111110101001110101100101111;
        private const uint _PrimeD = 0b00010110010101100110011110110001;

        //the state of this random is represented by an uint
        private uint _state;

        // --------------- CONSTRUCTORS AND INITIALIZERS --------------- //
        public static J_RandomX Seed(int seed) => (uint)seed + _PrimeD;

        public J_RandomX(uint state) { this._state = state; }

        // --------------- RANDOM FUNCTIONALITY --------------- //
        public J_RandomX Eat(uint data) => RotateLeft(_state + data * _PrimeB, 17) * _PrimeC;

        private static uint RotateLeft(uint data, int steps) => (data << steps) | (data >> 32 - steps);

        // --------------- RANDOM GENERATORS --------------- //
        /// <summary>
        /// gets the next random uint
        /// </summary>
        public uint NextUInt()
        {
            //this is the place where we have a change of state
            _state = Eat(_state);
            return _state;
        }

        /// <summary>
        /// gets the next random uint within a given range, inclusive
        /// </summary>
        public uint NextUInt(uint range)
        {
            //make sure to avoid the modulo bias
            var excluded   = uint.MaxValue % range;
            var validRange = uint.MaxValue - excluded;

            var attempts = 0;
            while (attempts < _MaxAttempts)
            {
                var value = NextUInt();
                if (value <= validRange) { return value % range; }

                attempts++;
            }

            return 0;
        }

        /// <summary>
        /// generates a int witn a min and a max inclusive
        /// </summary>
        /// <param name="min">the minimal result, inclusive</param>
        /// <param name="max">the max result inclusive -- max number is int.max -1</param>
        /// <returns>returns a random int within the given limits</returns>
        public int NextInt(int min, int max)
        {
            Assert.IsTrue(max < Int32.MaxValue, "To have it inclusive we need to add one");
            uint range = (uint)(max - min) + 1;
            var  value = (int)NextUInt(range);

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
            var range = max - min;
            var value = (float)NextUInt() / (float)(uint.MaxValue / range);

            return value + min;
        }

        // --------------- IMPLICIT CONVERSIONS --------------- //
        public static implicit operator J_RandomX(uint state) => new(state);

        public static implicit operator uint(J_RandomX hash)
        {
            uint avalanche = hash._state;
            avalanche ^= avalanche >> 15;
            avalanche *= _PrimeA;
            avalanche ^= avalanche >> 13;
            avalanche *= _PrimeB;
            avalanche ^= avalanche >> 16;
            return avalanche;
        }

        public override string ToString() => _state.ToString();
    }
}
