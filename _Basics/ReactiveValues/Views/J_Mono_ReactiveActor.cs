using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// an actor related to a reactive item
    /// </summary>
    /// <typeparam name="T">the reactive item</typeparam>
    public abstract class J_Mono_ReactiveActor<T> : J_Mono_Actor<T>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), ReadOnly, ShowInInspector] protected abstract jObservableValue<T> _ThisReactiveItem { get; }

        // --------------- INITIALIZATION --------------- //
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_ThisReactiveItem, $"{gameObject.name} requires a {nameof(_ThisReactiveItem)}");
        }

        // --------------- LISTENER SETUP --------------- //
        protected override void OnEnable()
        {
            ActorUpdate(_ThisReactiveItem.Current);
            _ThisReactiveItem.Subscribe(ActorUpdate);
        }

        protected virtual void OnDisable() => _ThisReactiveItem.UnSubscribe(ActorUpdate);
    }
}
