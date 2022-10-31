using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// only for unmanaged
    /// this class represent an actor that may have many elements, implementing the actor pattern, explained in these slides
    /// https://gamedevacademy.org/lessons-learned-in-unity-after-5-years/
    /// remember to seal the derived class for better performance
    /// </summary>
    /// <typeparam name="T">the unmanaged actor type</typeparam>
    public abstract class J_Mono_UnmanagedActor<T> : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iUpdaterReadonly<T>[] _relatedElements;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        protected virtual void SanityChecks()
        {
            Assert.IsTrue(GetComponentsInChildren<J_Mono_UnmanagedActor<T>>().Length == 1,
                          $"{gameObject.name} with {GetType()} has more than one actor of {typeof(T)}");
        }

        protected virtual void InitThis() { _relatedElements = GetComponentsInChildren<iUpdaterReadonly<T>>(true); }

        // --------------- MAIN COMMAND --------------- //
        public void ActorUpdate(in T actor) { UpdateAllViews(in actor); }

        // --------------- VIEW UPDATE --------------- //
        protected virtual void UpdateAllViews(in T actor)
        {
            for (int i = 0; i < _relatedElements.Length; i++) { UpdateView(in actor, _relatedElements[i]); }
        }

        /// <summary>
        /// updates the specific views on this actor
        /// </summary>
        protected virtual void UpdateView(in T actor, iUpdaterReadonly<T> view) { view.UpdateThis(in actor); }
    }
}
