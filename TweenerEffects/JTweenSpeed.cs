using System.Collections.Generic;
using PrimeTween;

namespace JReact.TweenEffects
{
    /// <summary>
    /// A set of live tweens sharing one speed: new tweens inherit the current speed and a rush re-times every
    /// live one. Dead handles are swept on Add, so the list never grows past what is actually playing.
    /// </summary>
    public static class JTweenSpeed
    {
        private static readonly List<Tween> _Live = new List<Tween>(8);
        private static float _Speed = 1f;

        public static Tween AddToTimeSpeed(this Tween tween)
        {
            tween.timeScale = _Speed;
            for (int i = _Live.Count - 1; i >= 0; i--)
            {
                if (!_Live[i].isAlive) { _Live.RemoveAt(i); }
            }

            _Live.Add(tween);
            return tween;
        }

        public static void SetSpeed(float speed)
        {
            _Speed = speed;
            for (int i = 0; i < _Live.Count; i++)
            {
                Tween tween = _Live[i]; // struct handle — the indexer return cannot be assigned through
                if (tween.isAlive) { tween.timeScale = speed; }
            }
        }

        public static void StopAll()
        {
            for (int i = 0; i < _Live.Count; i++)
            {
                if (_Live[i].isAlive) { _Live[i].Stop(); }
            }
        }

        public static void Clear()
        {
            _Live.Clear();
            _Speed = 1f;
        }
    }
}
