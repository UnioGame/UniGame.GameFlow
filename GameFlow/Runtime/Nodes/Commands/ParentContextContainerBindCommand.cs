namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniRx;

    public class ParentContextContainerBindCommand : ILifeTimeCommand
    {
        private readonly IBinder<IMessagePublisher>     _source;
        private readonly AssetReferenceContextContainer _parent;

        public ParentContextContainerBindCommand(IBinder<IMessagePublisher> source, AssetReferenceContextContainer parent)
        {
            _source = source;
            _parent = parent;
        }
        
        // TODO: надо будет сделать UniTask Execute, чтобы гарантировать прямое выполнение
        public async void Execute(ILifeTime lifeTime)
        {
            var parentContextContainer = await _parent.LoadAssetTaskAsync(lifeTime);
            
            parentContextContainer.
                Where(x => x != null).
                Subscribe(x =>
                {
                    _source.Bind(x).AddTo(lifeTime);
                }).
                AddTo(lifeTime);
        }
    }
}