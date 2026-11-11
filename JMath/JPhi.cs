namespace JReact
{
    /// <summary>
    /// The φ (golden ratio) timing ladder, in seconds. Every tween, transition and feedback beat takes one of these
    /// instead of an ad-hoc number. On the pure ladder each step is the sum of the two below it. Medium2 and Medium3
    /// split Medium1 to Medium4 into three equal φ^(1/3) steps. A higher number is always longer.
    /// </summary>
    public static class JPhiTime
    {
        /// <summary>Staggers and snaps.</summary>
        public const float XShort1 = 0.038f;
        public const float XShort2 = 0.062f;

        /// <summary>Feedback and fades.</summary>
        public const float Short1 = 0.100f;
        public const float Short2 = 0.162f;
        public const float Short3 = 0.262f;

        /// <summary>Transitions.</summary>
        public const float Medium1 = 0.424f;
        public const float Medium2 = 0.498f;
        public const float Medium3 = 0.584f;
        public const float Medium4 = 0.686f;

        /// <summary>Large reveals.</summary>
        public const float Long1 = 1.110f;
        public const float Long2 = 1.796f;

        /// <summary>Loops and pacing, never a transition the player waits on.</summary>
        public const float Ambient1 = 2.906f;
        public const float Ambient2 = 4.702f;
        public const float Ambient3 = 7.608f;

        /// <summary>The ladder as a serializable choice, so authored data stores a step rather than raw seconds.</summary>
        public enum Rung : byte
        {
            XShort1 = 0, XShort2 = 1,
            Short1 = 2, Short2 = 3, Short3 = 4,
            Medium1 = 5, Medium2 = 6, Medium3 = 7, Medium4 = 8,
            Long1 = 9, Long2 = 10,
            Ambient1 = 11, Ambient2 = 12, Ambient3 = 13,
        }

        public static float Seconds(Rung rung) => rung switch
        {
            Rung.XShort1  => XShort1,
            Rung.XShort2  => XShort2,
            Rung.Short1   => Short1,
            Rung.Short2   => Short2,
            Rung.Short3   => Short3,
            Rung.Medium1  => Medium1,
            Rung.Medium2  => Medium2,
            Rung.Medium3  => Medium3,
            Rung.Medium4  => Medium4,
            Rung.Long1    => Long1,
            Rung.Long2    => Long2,
            Rung.Ambient1 => Ambient1,
            Rung.Ambient2 => Ambient2,
            Rung.Ambient3 => Ambient3,
            _             => throw new System.ArgumentOutOfRangeException(nameof(rung), rung, null),
        };
    }

    /// <summary>
    /// The inverse-φ proportion ladder. A size, height or offset derived from another one takes a rung here
    /// instead of a hand-picked number.
    /// </summary>
    public static class JPhiRatio
    {
        /// <summary>1/φ, the major split.</summary>
        public const float Major = 0.618034f;
        /// <summary>(1/φ)², the minor split.</summary>
        public const float Minor = 0.381966f;
    }

    /// <summary>The inverse-φ opacity ladder, (1/φ)^n.</summary>
    public static class JPhiAlpha
    {
        public const float Full   = 1f;
        public const float High   = JPhiRatio.Major;
        public const float Medium = JPhiRatio.Minor;
        public const float Low    = JPhiRatio.Major * JPhiRatio.Minor;
        public const float Faint  = JPhiRatio.Minor * JPhiRatio.Minor;
    }
}
