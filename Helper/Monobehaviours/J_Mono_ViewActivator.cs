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

        //the views we want to activate
        [BoxGroup("Views", true, true, -50), SerializeField, Required] private GameObject[] _views;
        //to decide if we want to start them as active
        [BoxGroup("Views", true, true, -50), SerializeField] protected bool _startsActive;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; }

        // --------------- INITIALIZATION --------------- //
        //used for initialization
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        //used to check that every element is valid
        protected virtual void SanityChecks() { Assert.IsTrue(_views.Length > 0, $"{gameObject.name} requires at least one view"); }

        //used to initialize this element
        protected virtual void InitThis() { ActivateView(_startsActive); }

        // --------------- ACTIVATION --------------- //
        //used to activate the views
        public void ActivateView(bool activateView)
        {
            for (int i = 0; i < _views.Length; i++)
            {
                if (_views[i] == null) continue;
                ActivateSpecificView(_views[i], activateView);
            }

            IsActive = activateView;
            OnActivation?.Invoke(activateView);
        }

        //this is used to activate a specific view
        private void ActivateSpecificView(GameObject viewToActivate, bool activeNow)
        {
            // JLog.QuickLog($"{name} is enabling {viewToActivate.name} = {activeNow}");
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
