namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces
{
    using UniRx;

    public interface IReactivePortValue<TValue> : 
        IReadOnlyReactiveProperty<TValue>,
        IReactiveSource
    {
        void SetValue(TValue portValue);
    }
}