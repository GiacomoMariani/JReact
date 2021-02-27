using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.UiView
{
    /// <summary>
    /// used to show a text
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class J_UiView_Text : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private TextMeshProUGUI _text;

        // --------------- INIT --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis() {}

        protected virtual void SanityChecks() { Assert.IsNotNull(_text, $"{gameObject.name} requires a {nameof(_text)}"); }

        // --------------- COMMANDS --------------- //
        protected virtual void SetText(string text) => _text.text = text;

        protected virtual void SetColor(Color color) => _text.color = color;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}
