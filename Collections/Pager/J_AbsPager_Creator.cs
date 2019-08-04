using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.UiView.Collections
{
    public abstract class J_AbsPager_Creator<T> : J_UiView_Collection<T>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_Pager _pager;

        // --------------- INIT --------------- //
        protected override void InitThis()
        {
            base.InitThis();
            _pager.ResetThis();
        }

        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_pager, $"{gameObject.name} requires a {nameof(_pager)}");
        }

        // --------------- ADD AND REMOVE --------------- //
        protected override void AddedView(T elementsToBeShown, J_Mono_Actor<T> newUiView)
        {
            base.AddedView(elementsToBeShown, newUiView);
            _pager.AddItem(newUiView.gameObject);
        }

        protected override void RemovedView(T elementChanged, J_Mono_Actor<T> uiView)
        {
            base.RemovedView(elementChanged, uiView);
            _pager.RemoveItem(uiView.gameObject);
        }
    }
}
