namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Context.Runtime.Connections;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
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
        
        // TODO: надо будет сделать UniTask Execute, чтобы гарантировать прямое выполнение
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