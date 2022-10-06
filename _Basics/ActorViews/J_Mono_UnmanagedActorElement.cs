using UnityEngine;

namespace JReact
{
    /// <summary>
    /// only for unmanaged
    /// component or view of an actor element and requires an J_Mono_Actor to be tracked
    /// remember to seal the derived class for better performance
    /// </summary>
    /// <typeparam name="T">the unmanaged actor type related to this element</typeparam>
    public abstract class J_Mono_UnmanagedActorElement<T> : MonoBehaviour, iUpdaterReadonly<T>
    where T: unmanaged
    {
        public void UpdateThis(in T actor) { ActorUpdate(actor); }

        protected abstract void ActorUpdate(in T    actor);
    }
}
