using System;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// A single logged message: its type, the (optional) source that produced it, the text,
    /// a monotonic ordering number and the Unix time it was raised.
    /// </summary>
    public readonly struct JMessage
    {
        // --------------- CORE --------------- //
        public readonly string Content;       // the resolved text to show
        public readonly int    MessageId;     // the message type (see JMessageType)
        public readonly int    MessageNumber; // monotonic sequence id, used for ordering

        // --------------- SOURCE --------------- //
        public readonly int    SourceId;      // 0 = none/system; otherwise a stable source id
        public readonly string SourceName;    // cached display name of the source ("" for system)

        // --------------- TIME --------------- //
        public readonly long TimeStamp;       // Unix time in milliseconds when the message was raised

        // --------------- CONVENIENCE --------------- //
        public bool HasSource => SourceId != 0 || !string.IsNullOrEmpty(SourceName);

        /// <summary>current Unix time in milliseconds, used to stamp new messages</summary>
        public static long NowUnixMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // --------------- CONSTRUCTORS --------------- //
        // Back-compatible: a sourceless (system) message.
        public JMessage(string content, int messageId, int messageNumber)
            : this(content, messageId, messageNumber, 0, null, 0L) { }

        // Full constructor.
        public JMessage(string content, int messageId, int messageNumber, int sourceId, string sourceName,
                        long timeStamp)
        {
            Content       = content;
            MessageId     = messageId;
            MessageNumber = messageNumber;
            SourceId      = sourceId;
            SourceName    = sourceName;
            TimeStamp     = timeStamp;
        }
    }
}
