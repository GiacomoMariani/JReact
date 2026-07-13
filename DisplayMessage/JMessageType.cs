namespace JReact.ScreenMessage
{
    /// <summary>
    /// Message type ids stored on <see cref="JMessage.MessageId"/>. A const library (like
    /// <see cref="JLogTags"/>) rather than an enum, so each game can extend the id space
    /// without editing this framework type.
    ///
    /// Ids 0..<see cref="GeneralMax"/> are reserved for general types shared by every game and
    /// defined here. Game-specific types start at <see cref="FirstGameSpecific"/> and are
    /// declared per game (in a game-side const class).
    /// </summary>
    public class JMessageType
    {
        // --------------- GENERAL TYPES (reserved 0..GeneralMax) --------------- //
        public const int System    = 0; // engine / narrator / UI feedback (no source)
        public const int Character = 1; // spoken by a character (carries a source)
        // 2..GeneralMax left free for future general types

        // --------------- RANGE BOUNDARY --------------- //
        public const int GeneralMax        = 99;  // last id reserved for JReact general types
        public const int FirstGameSpecific = 100; // game-specific message types start here
    }
}
