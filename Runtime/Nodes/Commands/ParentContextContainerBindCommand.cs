using UniGame.AddressableTools.Runtime;
using UniGame.Context.Runtime;

namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Context.Runtime.Connections;
    using Core.Runtime;
    using Context.Runtime;
    using UniRx;

    public class ParentContextContainerBindCommand : ILifeTimeCommand
    {
        private readonly IContextConnection             _source;
        private readonly AssetReferenceContextContainer _parent;

        public ParentContextContainerBindCommand(IContextConnection source, AssetReferenceContextContainer parent)
        {
            _source = source;
            _parent = parent;
        }
        
        public async UniTask Execute(ILifeTime lifeTime)
        {
            var parentContextContainer = await _parent.LoadAssetTaskAsync(lifeTime);
            parentContextContainer
                .Where(x => x != null)
                .Subscribe(x => _source.Connect(x).AddTo(lifeTime))
                .AddTo(lifeTime);
        }
    }
}