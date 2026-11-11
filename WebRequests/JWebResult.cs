namespace JReact.JWebRequests
{
    /// <summary>
    /// The final state of a request sent with a retry policy.
    /// </summary>
    public readonly struct JWebResult
    {
        public JWebOutcome Outcome { get; }
        public JWebResponse Response { get; }
        public int Attempts { get; }

        public JWebResult(JWebOutcome outcome, JWebResponse response, int attempts)
        {
            Outcome  = outcome;
            Response = response;
            Attempts = attempts;
        }
    }
}
