using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Localization.LocalizationText
{
    [CreateAssetMenu(menuName = "Reactive/Localization/Entry", fileName = "J_SO_LocalizationEntry", order = 0)]
    public class J_SO_LocalizationEntry : ScriptableObject
    {
        [SerializeField] private int _id;
        public int ID => _id;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string KeySafe
            => _content.ArrayIsValid() ? _content[0] : "";
        public string Key => Content[0];

        [SerializeField] private string[] _content;
        public string[] Content => _content;

        public string GetTextOrDefault(int currentLanguage) => _content.ValidIndex(currentLanguage) ? _content[currentLanguage] : KeySafe;
        
        public string PrintAllContent(J_SO_LocalizationLibrary library)
        {
            string           result    = "";
            SystemLanguage[] languages = library.Languages;
            for (int i = 0; i < languages.Length; i++) { result += $"{languages[i]}: {GetTextOrDefault(i)}--"; }

            return result;
        }
        
        

        // --------------- UNITY EDITOR --------------- //
#if UNITY_EDITOR
        internal void SaveDefault(int id, string text)
        {
            _id        =   id;
            _content   ??= new string[1];
            Content[0] =   text;
        }

        internal void SetData(int id, string[] content)
        {
            _id      = id;
            _content = content;
        }
#endif
    }
}
