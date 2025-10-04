using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Localization.LocalizationText
{
    [CreateAssetMenu(menuName = "Reactive/Localization/Library", fileName = "J_SO_LocalizationLibrary", order = 0)]
    public class J_SO_LocalizationLibrary : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _entryFolder = "Entries";
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private TextAsset _source;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private SystemLanguage[] _languages;
        public SystemLanguage[] Languages => _languages;
        public SystemLanguage DefaultLanguage => Languages.ArrayIsValid() ? Languages[0] : SystemLanguage.English;
        public int TotalLanguages => _languages.Length;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_LocalizationEntry[] _entries;

        public SystemLanguage IdToLanguage(int languageId) => Languages.ArrayIsValid() ? Languages[languageId] : DefaultLanguage;
        public int            LanguageToId(SystemLanguage language) => Array.IndexOf(_languages, language);
        public bool           HasLanguage(SystemLanguage language) => LanguageToId(language) >= 0;

        [Button] private void CheckTextDuplicates() => TextToValues();

        private Dictionary<string, (int lineIndex, string fullLine)> TextToValues()
        {
            var      result = new Dictionary<string, (int lineIndex, string fullLine)>();
            string[] lines  = _source.text.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrEmpty(line))
                {
                    JLog.Warning($"{nameof(_source)} has an empty line at {i}", JLogTags.Localization, this);
                    continue;
                }

                string[] lineItems = line.Split(',');
                string   key       = lineItems[0];
                if (result.ContainsKey(key))
                {
                    JLog.Warning($"{key} => {line} - {i}: duplicate of {result[key].lineIndex}.", JLogTags.Localization, this);
                    continue;
                }

                result.Add(key, (i, line));
            }

            return result;
        }

        private Dictionary<string, J_SO_LocalizationEntry> GenerateDictionary()
        {
            var dictionary = new Dictionary<string, J_SO_LocalizationEntry>();
            foreach (J_SO_LocalizationEntry entry in _entries)
            {
                Assert.IsNotNull(entry,     $"{name} requires a {nameof(entry)}");
                Assert.IsNotNull(entry.Key, $"{name} requires a {nameof(entry.Key)}");
                Assert.IsFalse(dictionary.ContainsKey(entry.Key), $"The key {entry.Key} is already in the dictionary");
                dictionary.Add(entry.Key, entry);
            }

            return dictionary;
        }

        public J_SO_LocalizationEntry TryCatch(string keyText)
        {
            Dictionary<string, J_SO_LocalizationEntry> dictionary = GenerateDictionary();
            dictionary.TryGetValue(keyText, out J_SO_LocalizationEntry result);
            if (result == default) { JLog.Warning($"The key {keyText} is not in the dictionary", JLogTags.Localization, this); }

            Dictionary<string, (int lineIndex, string fullLine)> textToValues = TextToValues();
            if (textToValues.TryGetValue(keyText, out (int lineIndex, string fullLine) value))
            {
                result = CreateEntry(value.lineIndex, keyText, true);
                result.SetData(value.lineIndex, SplitData(value.fullLine));
            }

            return result;
        }

        private J_SO_LocalizationEntry CreateEntry(int lineIndex, string entryKey, bool addToEntries = false)
        {
            J_SO_LocalizationEntry entry = CreateInstance<J_SO_LocalizationEntry>();

#if UNITY_EDITOR
            string folderPath = GetFolderPath();
            if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath)) { CreateFolder(); }

            entryKey = entryKey.TrimSpace();
            if (entryKey.Length > 10) { entryKey = entryKey.Substring(0, 10); }

            string indexedKey = $"{lineIndex:D6}_{entryKey}";

            string assetPath = System.IO.Path.Combine(folderPath, $"{indexedKey}.asset").Replace("\\", "/");
            UnityEditor.AssetDatabase.CreateAsset(entry, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
            if (addToEntries) { _entries = _entries.AddToArray(entry); }

            return entry;
        }

        private string[] SplitData(string text) => text.Split('|');

#if UNITY_EDITOR
        // --------------- UNITY EDITOR --------------- //
        /// <summary>
        /// Loads localization entries from the provided source file, processes them, and updates the localization library.
        /// </summary>
        /// <remarks>
        /// This method parses the source file into individual lines, processes the header and entry lines,
        /// and updates the library with the parsed data. It also refreshes the associated Unity asset to ensure changes are saved.
        /// </remarks>
        [Button]
        public void LoadFromSource()
        {
            Dictionary<string, J_SO_LocalizationEntry>           entryDictionary = GenerateDictionary();
            Dictionary<string, (int lineIndex, string fullLine)> valuesFromText  = TextToValues();

            _entries = Array.Empty<J_SO_LocalizationEntry>();
            int currentEntry = 1;

            Dictionary<string, (int lineIndex, string fullLine)>.Enumerator enumerator = valuesFromText.GetEnumerator();
            enumerator.MoveNext();
            ProcessFirstLine(enumerator.Current.Value.fullLine);
            while (enumerator.MoveNext())
            {
                (int lineIndex, string fullLine) value     = enumerator.Current.Value;
                bool                             idAligned = value.lineIndex == currentEntry;
                if (!idAligned) { JLog.Warning($"Line {value.lineIndex} differs {currentEntry}", JLogTags.Localization, this); }

                if (ProcessEntryLine(currentEntry, value.fullLine, entryDictionary)) { currentEntry++; }
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

        private void ProcessFirstLine(string line)
        {
            string[] lineItems = SplitData(line);
            _languages = new SystemLanguage[lineItems.Length];
            for (int i = 0; i < lineItems.Length; i++)
            {
                var language = (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), lineItems[i]);
                _languages[i] = language;
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// Processes an individual localization entry line, parses its data, and updates or creates the corresponding entry in the localization dictionary.
        /// </summary>
        /// <param name="lineIndex">The index of the current line being processed, starting from 1.</param>
        /// <param name="line">The content of the line that contains the localization entry information.</param>
        /// <param name="entryDictionary">A dictionary mapping entry keys to their corresponding localization entries.</param>
        private bool ProcessEntryLine(int lineIndex, string line, Dictionary<string, J_SO_LocalizationEntry> entryDictionary)
        {
            Assert.IsTrue(lineIndex > 0, $"The line {lineIndex} is not a valid entry");
            if (string.IsNullOrEmpty(line)) { return false; }

            string[] lineItems = SplitData(line);

            int totalEntries = lineItems.Length;
            if (totalEntries > _languages.Length)
            {
                JLog.Error($"Entry {lineIndex} has {lineItems.Length} entries but there are {TotalLanguages} languages\n{line}");
                return false;
            }

            string entryKey        = lineItems[0];
            bool   requireCreation = false;
            if (!entryDictionary.TryGetValue(entryKey, out J_SO_LocalizationEntry entry))
            {
                entry           = CreateEntry(lineIndex, entryKey, true);
                requireCreation = true;
            }
            else { JLog.Warning($"{entryKey} overriding existing line from {lineIndex} in {name}", JLogTags.Localization, this); }

            entry.SetData(lineIndex, lineItems);
            entryDictionary[entryKey] = entry;
            UnityEditor.EditorUtility.SetDirty(entry);
            return requireCreation;
        }

        private string GetFolderPath()
        {
            string libraryPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(libraryPath), _entryFolder);
        }

        private void CreateFolder()
        {
            string libraryPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            UnityEditor.AssetDatabase.CreateFolder(System.IO.Path.GetDirectoryName(libraryPath), _entryFolder);
        }

        public J_SO_LocalizationEntry AddEntry(string defaultText)
        {
            if (TryCatch(defaultText) != default)
            {
                JLog.Warning($"The key {defaultText} has another entry, already in the dictionary, try catching with {nameof(TryCatch)}",
                             JLogTags.Localization, this);

                return default;
            }

            string                 newLine = _source.text.EndsWith("\n") ? defaultText : $"\n{defaultText}";
            J_SO_LocalizationEntry entry   = CreateEntry(_entries.Length + 1, defaultText, true);
            System.IO.File.AppendAllText(UnityEditor.AssetDatabase.GetAssetPath(_source), newLine);

            Assert.IsNotNull(entry,    $"{name} requires a {nameof(entry)}");
            Assert.IsNotNull(_entries, $"{name} requires a {nameof(_entries)}");
            entry.SaveDefault(_entries.Length, defaultText);
            UnityEditor.EditorUtility.SetDirty(entry);
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.SetDirty(this);
            return entry;
        }

        public J_SO_LocalizationEntry UpdateEntry(J_SO_LocalizationEntry entry, string text)
        {
            Assert.IsNotNull(entry, $"{name} requires a {nameof(entry)}");

            Dictionary<string, J_SO_LocalizationEntry> dict = GenerateDictionary();
            if (dict.ContainsKey(entry.Key) &&
                dict[entry.Key] != entry)
            {
                JLog.Warning($"The key {entry.Key} has another entry, already in the dictionary");
                return default;
            }

            entry.SaveDefault(entry.ID, text);
            UnityEditor.EditorUtility.SetDirty(entry);
            return entry;
        }

        [Button]
        private void HardClear()
        {
            var path = GetFolderPath();
            if (UnityEditor.AssetDatabase.IsValidFolder(path))
            {
                UnityEditor.AssetDatabase.DeleteAsset(path);
                UnityEditor.AssetDatabase.Refresh();
            }

            _entries = Array.Empty<J_SO_LocalizationEntry>();
        }
#endif
    }
}
