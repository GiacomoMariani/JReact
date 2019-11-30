using System.Collections.Generic;

namespace JReact.Random
{
    public class J_RandomIntGenerator
    {
        private int _desiredAmount;
        private int _startAt;
        private bool _restartWhenFinished;
        private List<int> _validNumbers;

        private J_RandomIntGenerator(int desiredAmount, int start = 0, bool restart = true)
        {
            _desiredAmount       = desiredAmount;
            _startAt             = start;
            _validNumbers        = new List<int>();
            _restartWhenFinished = restart;
            PopulateList(desiredAmount, start);
        }

        private void PopulateList(int desiredAmount, int startAt)
        {
            for (int i = 0; i < desiredAmount; i++) _validNumbers.Add(i + startAt);
        }

        private int GetRandomInt()
        {
            if (_validNumbers.Count == 0)
            {
                if (_restartWhenFinished) PopulateList(_desiredAmount, _startAt);
                return -1;
            }
            else
            {
                int nextIndex = UnityEngine.Random.Range(0, _validNumbers.Count - 1);
                int number    = _validNumbers[nextIndex];
                _validNumbers.RemoveAt(nextIndex);
                return number;
            }
        }
    }
}
