namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    [Serializable]
    public class DataSourceTaskCommand<TData> : ILifeTimeCommand 
    {
        private readonly UniTask<TData> _source;
        private readonly UniTask<IContext> _target;

        public DataSourceTaskCommand(UniTask<TData> source, UniTask<IContext> target)
        {
            _source = source;
            _target = target;
        }

        public async void Execute(ILifeTime lifeTime)
        {
            var context = await _target;
            var asset = await _source;
            if (asset == null) {
                GameLog.LogError($"NULL asset loaded from {GetType().Name}");
                return;
            }
            
            context.Publish(asset);
        }
    }
}
