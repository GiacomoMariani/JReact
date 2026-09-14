namespace JReact
{
    /// <summary>
    /// The φ (golden ratio) timing ladder. Sixteen named durations: every tween, transition and feedback beat
    /// takes one of these instead of an ad-hoc number. The unsuffixed values are seconds (what PrimeTween and
    /// MEC consume), the <c>Ms</c> anchors are the source values.
    /// </summary>
    public static class JPhiTime
    {
        public const int InstantMs       = 38;
        public const int LightningMs     = 62;
        public const int SuperFastMs     = 100;
        public const int VeryFastMs      = 162;
        public const int FastMs          = 262;
        public const int ModerateFastMs  = 424;
        public const int ModerateMs      = 498;
        public const int ModerateSlowMs  = 586;
        public const int SlowMs          = 686;
        public const int VerySlowMs      = 792;
        public const int ExtremelySlowMs = 923;
        public const int UltraSlowMs     = 1110;
        public const int LongMs          = 1796;
        public const int SuperLongMs     = 2906;
        public const int UltraLongMs     = 4702;
        public const int ExtremeMs       = 7610;

        public const float Instant       = InstantMs       * 0.001f;
        public const float Lightning     = LightningMs     * 0.001f;
        public const float SuperFast     = SuperFastMs     * 0.001f;
        public const float VeryFast      = VeryFastMs      * 0.001f;
        public const float Fast          = FastMs          * 0.001f;
        public const float ModerateFast  = ModerateFastMs  * 0.001f;
        public const float Moderate      = ModerateMs      * 0.001f;
        public const float ModerateSlow  = ModerateSlowMs  * 0.001f;
        public const float Slow          = SlowMs          * 0.001f;
        public const float VerySlow      = VerySlowMs      * 0.001f;
        public const float ExtremelySlow = ExtremelySlowMs * 0.001f;
        public const float UltraSlow     = UltraSlowMs     * 0.001f;
        public const float Long          = LongMs          * 0.001f;
        public const float SuperLong     = SuperLongMs     * 0.001f;
        public const float UltraLong     = UltraLongMs     * 0.001f;
        public const float Extreme       = ExtremeMs       * 0.001f;
    }

    /// <summary>
    /// The inverse-φ proportion ladder. A size, height or offset derived from another one takes a rung here
    /// instead of a hand-picked number.
    /// </summary>
    public static class JPhiRatio
    {
        /// <summary>1/φ — the major split, one rung down from a whole.</summary>
        public const float Major = 0.618034f;
        /// <summary>(1/φ)² — the minor split, two rungs down.</summary>
        public const float Minor = 0.381966f;
    }

    /// <summary>
    /// The inverse-φ opacity ladder ((1/φ)^n). Keeps "how lit" consistent with "how big": hovers, dims, hints
    /// and ghosts all read from here.
    /// </summary>
    public static class JPhiAlpha
    {
        public const float Full   = 1f;
        public const float Hover  = 0.618f;
        public const float Dimmed = 0.382f;
        public const float Hint   = 0.236f;
        public const float Ghost  = 0.146f;
    }
}
