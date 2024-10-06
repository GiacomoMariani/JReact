using System;
using UnityEngine.Events;

namespace JReact
{
    public interface iSelectable<out T>
    {
        string NameOfThis { get; }
        public bool IsSelected { get; }
        void Select();
        void DeSelect();
    }

    public interface iProcessable
    {
        UnityAction ThisAction { get; }
        void Process();
    }

    public interface iInitiator<in T>
    {
        void InjectThis(T elementToInject);
    }

    public interface iUpdater<in T>
    {
        void UpdateThis(T elementToUpdate);
    }

    public interface iUpdaterReadonly<T>
    {
        void UpdateThis(in T elementToUpdate);
    }

    public interface iResettable
    {
        void ResetThis();
    }

    public interface iDestroyable
    {
        void DestroyThis();
    }

    public interface iActivable : iResettable
    {
        bool IsActive { get; }
        void Activate();
        void End();
    }

    public interface iObservable<out T>
    {
        IDisposable Subscribe(iObserver<T> observer);
    }

    public interface iObserver<in T>
    {
        void OnCompleted();
        void OnError(Exception exception);
        void OnNext(T          value);
    }

    public interface jObservable
    {
        void Subscribe(Action   action);
        void UnSubscribe(Action action);
    }

    public interface jStateJObservable : iTask, jObservable
    {
    }

    public interface jObservable<out T>
    {
        void Subscribe(Action<T>   action);
        void UnSubscribe(Action<T> action);
    }

    public interface jObservableValue<out T> : jObservable<T>
    {
        T Current { get; }
    }

    public interface iStackable : jObservableValue<int>
    {
        int Grant(int  amount);
        int Remove(int amount);
    }

    public interface iFillable : iStackable
    {
        int Max { get; }
        int FreeCapacity { get; }
        void SubscribeToMaxCapacity(Action<int>   action);
        void UnSubscribeToMaxCapacity(Action<int> action);
    }

    public interface iTask
    {
        string Name { get; }
        bool IsActive { get; }
        void Activate();
        void SubscribeToEnd(Action   action);
        void UnSubscribeToEnd(Action action);
    }

    //an interface of a ticker that may give a timer
    public interface iDeltaTime
    {
        float ThisDeltaTime { get; }
    }

    public interface iInputAxisGetter
    {
        float GetAxis(string    axisId);
        float GetAxisRaw(string axisId);
    }
}
