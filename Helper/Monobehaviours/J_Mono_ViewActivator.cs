using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// this is something to activate a number of views on the scene
    /// </summary>
    public class J_Mono_ViewActivator : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public event Action<bool> OnActivation;

        [BoxGroup("Views", true, true, -50), SerializeField, Required] private GameObject[] _views;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; }

        // --------------- INITIALIZATION --------------- //
        //used for initialization
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis() {}

        //used to check that every element is valid
        protected virtual void SanityChecks() => Assert.IsTrue(_views.Length > 0, $"{gameObject.name} requires at least one view");

        // --------------- ACTIVATION --------------- //
        //used to activate the views
        public virtual void ActivateView(bool activateView)
        {
            for (int i = 0; i < _views.Length; i++)
            {
                Assert.IsNotNull(_views[i], $"{gameObject.name} has a null view at index {i}");
                ActivateSpecificView(_views[i], activateView);
            }

            IsActive = activateView;
            OnActivation?.Invoke(activateView);
        }

        //this is used to activate a specific view
        private void ActivateSpecificView(GameObject viewToActivate, bool activeNow)
        {
            viewToActivate.SetActive(activeNow);
            if (activeNow) ActivateThis(viewToActivate);
            else DeActivateThis(viewToActivate);
        }

        // --------------- TEMPLATES --------------- //
        //if we want to add further actions to the view
        protected virtual void ActivateThis(GameObject   viewToActivate) {}
        protected virtual void DeActivateThis(GameObject viewToActivate) {}
    }
}
