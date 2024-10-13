using Cysharp.Threading.Tasks;

namespace JReact.Advertisting
{
    public class JInterval
    {
        public int MilliSecondsToNext { get; private set; }
        public readonly int IntervalInMilliSeconds;
        public readonly int DefaultDelayInMilliSeconds;

        public bool CanShow => MilliSecondsToNext > 0;
        public int MilliSecondsAfterPrevious => MilliSecondsToNext - IntervalInMilliSeconds;

        public JInterval(int intervalInMilliSeconds, int firstInterval = 0)
        {
            IntervalInMilliSeconds     = intervalInMilliSeconds;
            MilliSecondsToNext         = firstInterval;
            DefaultDelayInMilliSeconds = 100;
            StartCounting();
        }

        private async void StartCounting() { await Count(); }

        private async UniTask Count()
        {
            while (MilliSecondsToNext > 0)
            {
                await UniTask.Delay(DefaultDelayInMilliSeconds);
                MilliSecondsToNext        -= DefaultDelayInMilliSeconds;
            }
        }

        public void RestartCount()
        {
            MilliSecondsToNext   = IntervalInMilliSeconds;
            StartCounting();
        }
    }
}
