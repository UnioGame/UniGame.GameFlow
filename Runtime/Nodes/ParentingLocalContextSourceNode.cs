using UniModules.UniGame.AddressableTools.Runtime.Extensions;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.GameFlow.Runtime.Commands;
    using global::UniGame.UniNodes.Nodes.Runtime.Commands;
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using NodeSystem.Runtime.Core.Attributes;
    using UniGame.Context.Runtime.Connections;
    using UniGame.Core.Runtime.Interfaces;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [CreateNodeMenu("Common/Sources/Parenting Local Context Source")]
    public class ParentingLocalContextSourceNode : InOutPortNode
    {
        private IContextConnection _contextConnection;

        #region inspector
        
        [SerializeField]
#if ODIN_INSPECTOR
        [DrawWithUnity]
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
        [Header("Data Source")]
        [SerializeField]
        private AssetReferenceDataSource _dataSources;

        /// <summary>
        /// use graph context instead of separate one
        /// </summary>
        public bool useGraphContextAsOutput = false;
        
        #endregion
        
        protected sealed override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            _contextConnection =  useGraphContextAsOutput 
                ? Context 
                : _contextConnection ?? new ContextConnection().AddTo(LifeTime);
            
            var outPort = UniTask.FromResult<IContext>(PortPair.OutputPort);
            var contextSource = UniTask.FromResult<IContext>(_contextConnection);

            var bindContextToOutput = new MessageBroadcastCommand(_contextConnection, PortPair.OutputPort);
            //register all Context Sources Data into target context asset
            var registerDataSourceIntoContext = new OwnerRegisterDataSourceCommand(contextSource, _dataSources);
            //Register context to output port
            var contextToOutputPortCommand  = new DataSourceTaskCommand<IContext>(contextSource, outPort);
            var contextContainerBindCommand = new ParentContextContainerBindCommand(_contextConnection, _parentContextContainer);

            nodeCommands.Add(bindContextToOutput);
            nodeCommands.Add(registerDataSourceIntoContext);
            nodeCommands.Add(contextToOutputPortCommand);
            nodeCommands.Add(contextContainerBindCommand);
        }

        protected override async UniTask OnExecute()
        {
            if (_localContextContainer == null) return;
            
            var localContextContainer = await _localContextContainer.LoadAssetTaskAsync(LifeTime);
            LifeTime.AddDispose(localContextContainer);
            localContextContainer.SetValue(_contextConnection);
        }
    }
    
}