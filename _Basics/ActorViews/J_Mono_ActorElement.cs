using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// component or view of an actor element and requires an J_Mono_Actor to be tracked
    /// remember to seal the derived class for better performance
    /// </summary>
    /// <typeparam name="T">the actor type related to this element</typeparam>
    public abstract class J_Mono_ActorElement<T> : MonoBehaviour, iUpdater<T>
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected T _actor;

        public void UpdateThis(T actor)
        {
            //remove the previous actor if any
            if (_actor != null) ActorIsRemoved(_actor);
            //set the new actor
            _actor = actor;
            ActorUpdate(actor);
        }

        // --------------- ABSTRACT IMPLEMENTATION --------------- //
        //change and remove actor methods
        protected abstract void ActorUpdate(T    actor);
        protected virtual  void ActorIsRemoved(T actor) {}

        // --------------- UNITY EVENTS --------------- //
        protected virtual void OnEnable() => ActorUpdate(_actor);

        protected virtual void OnDisable()
        {
            if (_actor != null) ActorIsRemoved(_actor);
        }

        protected virtual void OnDestroy()
        {
            if (_actor != null) ActorIsRemoved(_actor);
        }
    }
}
