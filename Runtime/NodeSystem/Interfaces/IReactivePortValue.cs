namespace UniModules.GameFlow.Runtime.Core.Interfaces
{
    using UniRx;

    public interface IReactivePortValue<TValue> : 
        IReadOnlyReactiveProperty<TValue>,
        IReactiveSource
    {
        void Publish(TValue portValue);
    }
}