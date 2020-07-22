namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniContextData.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = System.Object;

    [Serializable]
    public class DataSourceTaskCommand<TData> : ILifeTimeCommand 
    {
        private readonly UniTask<TData> _source;
        private readonly UniTask<IContext> _target;

        public IAsyncContextDataSource dataSource;

        public DataSourceTaskCommand(UniTask<TData> source,UniTask<IContext> target)
        {
            this._source = source;
            this._target = target;
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
