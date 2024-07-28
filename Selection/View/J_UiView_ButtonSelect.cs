using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.Selection
{
    /// <summary>
    /// a button to select a selectable
    /// </summary>
    /// <typeparam name="T">a selectable element</typeparam>
    [RequireComponent(typeof(Button), typeof(Image))]
    public abstract class J_UiView_ButtonSelect<T> : J_Mono_ActorElement<T>
        where T : class
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract J_Selector<T> _ThisSelector { get; }

        //the button related to this element
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Button _thisButton;
        protected Button ThisButton
        {
            get
            {
                if (_thisButton == null) _thisButton = GetComponent<Button>();
                return _thisButton;
            }
        }

        // --------------- INIT --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        protected virtual void SanityChecks() { Assert.IsNotNull(ThisButton, $"{gameObject.name} requires a {nameof(ThisButton)}"); }
        protected virtual void InitThis()     {}

        // --------------- COMMAND --------------- //
        private void ButtonCommand() { _ThisSelector.Select(_actor); }

        // --------------- LISTENERS --------------- //
        protected override void OnEnable()
        {
            base.OnEnable();
            ThisButton.onClick.AddListener(ButtonCommand);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ThisButton.onClick.RemoveListener(ButtonCommand);
        }
    }
}
