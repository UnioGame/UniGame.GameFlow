namespace UniModules.UniGameFlow.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.GameFlow.Runtime.Commands;
    using global::UniGame.UniNodes.Nodes.Runtime.Commands;
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using NodeSystem.Runtime.Core.Attributes;
    using UniCore.Runtime.Rx.Extensions;
    using UniGame.AddressableTools.Runtime.Extensions;
    using UniGame.Context.Runtime.Context;
    using UniGame.Core.Runtime.Interfaces;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [CreateNodeMenu("GameSystem/Owner Context Source")]
    public class OwnerContextSourceNode : InOutPortNode
    {
        #region inspector

        public bool useGraphContext = true;

#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        [SerializeField]
        private AssetReferenceContextContainer localContextContainer;

#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        [Header("Data Source")]
        [SerializeField]
        private AssetReferenceDataSource dataSources;

        #endregion

        private IContext _context;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            _context = useGraphContext ? Context : new EntityContext().AddTo(LifeTime);
            
            var port          = UniTask.FromResult<IContext>(PortPair.OutputPort);
            var contextSource = UniTask.FromResult<IContext>(_context);

            var bindContextToOutput = new MessageBroadcastCommand(_context, PortPair.OutputPort);
            //register all Context Sources Data into target context asset
            var registerDataSourceIntoContext = new OwnerRegisterDataSourceCommand(contextSource, dataSources);
            //Register context to output port
            var contextToOutputPortCommand = new DataSourceTaskCommand<IContext>(contextSource, port);

            nodeCommands.Add(bindContextToOutput);
            nodeCommands.Add(registerDataSourceIntoContext);
            nodeCommands.Add(contextToOutputPortCommand);
        }

        protected sealed override async UniTask OnExecute()
        {
            var container = await localContextContainer.LoadAssetTaskAsync(LifeTime);
            container.SetValue(_context);
            LifeTime.AddCleanUpAction(() => container.SetValue(null));
        }
    }
}