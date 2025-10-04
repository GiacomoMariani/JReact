using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public readonly struct JTextSetting
{
    // The 32-bit union value (overlaps the two 16-bit fields below)
    [FieldOffset(0)] public readonly int Union;

    // Low 16 bits (offset 0): SizeSet
    [FieldOffset(0)] private readonly ushort _sizeSet;

    // High 16 bits (offset 2): FontType
    [FieldOffset(2)] private readonly ushort _fontType;

    public ushort FontType => _fontType;
    public ushort SizeSet => _sizeSet;

    // Build from the two 16-bit pieces
    public JTextSetting(ushort fontType, ushort sizeSet)
    {
        Union     = ((int)fontType << 16) | sizeSet;
        _fontType = fontType;
        _sizeSet  = sizeSet;
    }

    // Build from the packed 32-bit union
    public JTextSetting(int union)
    {
        _fontType = (ushort)((union >> 16) & 0xFFFF);
        _sizeSet  = (ushort)(union         & 0xFFFF);
        Union     = union;
    }

    public static implicit operator int(JTextSetting setting) => setting.Union;

    public override string ToString() => $"Font {_fontType}, Size {_sizeSet} = {Union}";
}
