using UniModules.GameFlow.Runtime.Attributes;

namespace UniGame.GameFlow.Runtime.Nodes
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.UniNodes.GameFlow.Runtime.Commands;
    using UniGame.UniNodes.Nodes.Runtime.Commands;
    using UniGame.UniNodes.Nodes.Runtime.Common;
    using UniGame.Runtime.ObjectPool.Extensions;
    using AddressableTools.Runtime;
    using UniModules.UniGame.Context.Runtime.Connections;
    using Core.Runtime;
    using Context.Runtime;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;
    using UnityEngine.Pool;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    [CreateNodeMenu("Common/Sources/Serializable Parent Context")]
    [NodeInfo(category:"Sources", description: 
        "1. Create new Local Context ot use Graph context on Init\n"+
        "2. Publish ALL contextDataSources INTO Output port\n"+
        "2. Publish ALL contextDataSources INTO Local Context\n"+
        "3. Publish Local Context INTO Context container")]
    public class ParentContextSerializedNode : SContextNode
    {
        private IContextConnection _contextConnection;

        #region inspector
        
        /// <summary>
        /// use graph context instead of separate one
        /// </summary>
        public bool useGraphContextAsOutput = true;

        public bool copyContextToContainer = false;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [DrawWithUnity]
        [ShowIf(nameof(copyContextToContainer))]
#endif
        public AssetReferenceContextContainer _localContextContainer;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        public AssetReferenceContextContainer _parentContextContainer;
        
#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        [SerializeField]
        private AssetReferenceDataSource _dataSources;

        #endregion

        protected sealed override async UniTask OnContextActivate(IContext context)
        {
            await BindContext();
            
            if (_localContextContainer == null || !copyContextToContainer) return;
            
            var localContextContainer = await _localContextContainer
                .LoadAssetTaskAsync(LifeTime);
            
            LifeTime.AddDispose(localContextContainer);
            localContextContainer.SetValue(_contextConnection);
            
            Complete();
        }
        

        private async UniTask BindContext()
        {
            var nodeCommands = ListPool<ILifeTimeCommand>.Get();
            
            _contextConnection =  useGraphContextAsOutput 
                ? Context 
                : _contextConnection ?? new ContextConnection().AddTo(LifeTime);
            
            var outPort = UniTask.FromResult<IContext>(Output);
            var contextSource = UniTask.FromResult<IContext>(_contextConnection);
            var bindContextToOutput = new MessageBroadcastCommand(_contextConnection, Output);
            
            //register all Context Sources Data into target context asset
            var registerDataSourceIntoContext = new OwnerRegisterDataSourceCommand(contextSource, _dataSources);
            //Register context to output port
            var contextToOutputPortCommand  = new DataSourceTaskCommand<IContext>(contextSource, outPort);
            var contextContainerBindCommand = new ParentContextContainerBindCommand(_contextConnection, _parentContextContainer);

            nodeCommands.Add(bindContextToOutput);
            nodeCommands.Add(registerDataSourceIntoContext);
            nodeCommands.Add(contextToOutputPortCommand);
            nodeCommands.Add(contextContainerBindCommand);

            foreach (var nodeCommand in nodeCommands)
                await nodeCommand.Execute(LifeTime);
            
            nodeCommands.Despawn();
        }
    }
}