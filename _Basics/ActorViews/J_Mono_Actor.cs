using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// this class represent an actor that may have many elements, implementing the actor pattern, explained in these slides
    /// https://gamedevacademy.org/lessons-learned-in-unity-after-5-years/
    /// remember to seal the derived class for better performance
    /// </summary>
    /// <typeparam name="T">the actor type</typeparam>
    public abstract class J_Mono_Actor<T> : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        //sets the actor directly or by injection
        [InfoBox("Null => needs to be injected via code"), BoxGroup("Setup", true, true), SerializeField]
        protected T _actor;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _initCompleted;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iUpdater<T>[] _relatedElements;

        // --------------- INITIALIZATION --------------- //
        protected virtual void SanityChecks()
        {
            Assert.IsTrue(GetComponentsInChildren<J_Mono_Actor<T>>().Length == 1,
                          $"{gameObject.name} with {GetType()} has more than one actor of {typeof(T)} for a total of: {GetComponentsInChildren<J_Mono_Actor<T>>().Length}");
        }

        protected virtual void InitThis()
        {
            if (_initCompleted) { return; }

            _relatedElements = GetComponentsInChildren<iUpdater<T>>(true);
            _initCompleted   = true;
        }

        public void ActorUpdate(T actor)
        {
            if (_actor != null) { ActorRemoved(_actor); }

            _actor = actor;
            SanityChecks();
            if (!_initCompleted) { InitThis(); }

            UpdateAllViews(actor);
            ActorAdded(actor);
        }

        /// <summary>
        /// Clears the current actor by resetting it to its default value.
        /// Calls the update logic with a null or default actor, ensuring that all dependent views are reset accordingly.
        /// </summary>
        public void ClearActor() => ActorUpdate(default);

        // --------------- VIEW UPDATE --------------- //
        protected virtual void UpdateAllViews(T actor)
        {
            for (int i = 0; i < _relatedElements.Length; i++) { UpdateView(_relatedElements[i], actor); }
        }

        /// <summary>
        /// updates the specific views on this actor
        /// </summary>
        protected virtual void UpdateView(iUpdater<T> view, T actor) { view.UpdateThis(actor); }

        // --------------- ABSTRACT IMPLEMENTATION --------------- //
        protected virtual void ActorRemoved(T actor) {}
        protected virtual void ActorAdded(T   actor) {}

        protected virtual void OnEnable() => ActorUpdate(_actor);
    }
}
