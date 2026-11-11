#if JLOCALIZATION
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace JReact.Localization.LocalizationText
{
    /// <summary>
    /// every entry of a library by key, so an entry is found without searching the library
    /// </summary>
    public sealed class J_LocalizationEntryMap : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(1)] private int _entriesPerFrame = 64;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<string, J_SO_LocalizationEntry> _map;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsReady { get; private set; }

        // --------------- COMMANDS --------------- //
#if UNITY_UNITASK
        public void Build(J_SO_LocalizationLibrary library) => BuildAsync(library).Forget();

        // Spread over frames: IsReady tells when it is complete.
        private async UniTaskVoid BuildAsync(J_SO_LocalizationLibrary library)
        {
            IReadOnlyList<J_SO_LocalizationEntry> entries = Begin(library);
            for (int i = 0; i < entries.Count; i++)
            {
                Add(entries[i]);
                if ((i + 1) % _entriesPerFrame == 0) { await UniTask.Yield(); }
            }

            Complete();
        }
#else
        public void Build(J_SO_LocalizationLibrary library)
        {
            IReadOnlyList<J_SO_LocalizationEntry> entries = Begin(library);
            for (int i = 0; i < entries.Count; i++) { Add(entries[i]); }

            Complete();
        }
#endif

        private IReadOnlyList<J_SO_LocalizationEntry> Begin(J_SO_LocalizationLibrary library)
        {
            Assert.IsNotNull(library, $"{name} requires a {nameof(library)}");
            Assert.IsNull(_map, $"{name} is already built");
            IReadOnlyList<J_SO_LocalizationEntry> entries = library.Entries;
            _map = new Dictionary<string, J_SO_LocalizationEntry>(entries.Count, StringComparer.Ordinal);
            return entries;
        }

        private void Add(J_SO_LocalizationEntry entry)
        {
            Assert.IsNotNull(entry, $"{name} found an empty entry in the library");
            _map.Add(entry.KeySafe, entry);
        }

        private void Complete()
        {
            IsReady = true;
            JLog.Log($"{name} ready - {_map.Count} entries", JLogTags.Localization, this);
        }

        // --------------- QUERIES --------------- //
        public J_SO_LocalizationEntry GetEntry(string key)
        {
            Assert.IsTrue(IsReady, $"{name} is not ready");
            bool found = _map.TryGetValue(key, out J_SO_LocalizationEntry entry);
            Assert.IsTrue(found, $"{name} has no entry with key {key}");
            return entry;
        }
    }
}
#endif
