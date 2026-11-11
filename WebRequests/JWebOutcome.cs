namespace JReact.JWebRequests
{
    /// <summary>
    /// What the caller should do with a request after JWebClient finished it.
    /// </summary>
    public enum JWebOutcome
    {
        /// <summary>Delivered.</summary>
        Success,
        /// <summary>Not delivered for a temporary reason: keep the payload and try again later.</summary>
        Retry,
        /// <summary>Too large: split the payload and resend the parts; drop it if it cannot be split.</summary>
        Split,
        /// <summary>Will never succeed as sent: discard it.</summary>
        Drop
    }
}
