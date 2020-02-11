using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Interfaces;
using UniRx;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    public interface IReactivePortValue<TValue> : 
        IReadOnlyReactiveProperty<TValue>,
        IReactiveSource
    {
    }
}