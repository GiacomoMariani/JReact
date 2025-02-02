using System;

namespace JReact.Singleton
{
    /// <summary>
    /// an example of bitflag with more info at this link
    /// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators
    /// </summary>
    public struct J_SimpleFlag
    {
        public const int RightMask = 0b_0000_1111;
        public const int RightMaskFirstBit = 0;
        public const int MaxRightMask = 0b_1111;

        public const int LeftMask = 0b_1111_0000;
        public const int LeftMaskFirstBit = 4;
        public const int MaxLeftMask = 0b_1111;

        public int flags;

        public J_SimpleFlag(byte flags) { this.flags = flags; }

        // --------------- COMMANDS AND QUERIS --------------- //
        public readonly int GetRightMask() => (flags & RightMask) >> RightMaskFirstBit;

        public void SetRightMask(int value) { flags = (flags & ~RightMask) | ((value << RightMaskFirstBit) & RightMask); }

         public readonly int GetLeftMask() => (flags & LeftMask) >> LeftMaskFirstBit;

        public void SetLeftMask(int value) { flags = (flags & ~LeftMask) | ((value << LeftMaskFirstBit) & LeftMask); }
        
    }

    [Flags]
    public enum EnumFlag
    {
        None = 0,
        Flag1 = 1 << 0,
        Flag2 = 1 << 1,
        Flag3 = 1 << 2,
        
        Flag12 = Flag1 | Flag2

    }
    
    
}
