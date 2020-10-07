using System.Collections.Generic;
using UniGame.UniNodes.GameFlow.Runtime.Commands;
using UniGame.UniNodes.Nodes.Runtime.Common;
using UniModules.UniCore.Runtime.Interfaces;
using UniModules.UniGame.SerializableContext.Runtime.Addressables;

using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Nodes
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.Nodes.Runtime.Commands;
    using NodeSystem.Runtime.Core.Attributes;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniContextData.Runtime.Entities;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;

    /// <summary>
    /// 1. Create new Local Context on Init
    /// 2. Publish ALL contextDataSources INTO Output port
    /// 2. Publish ALL contextDataSources INTO Local Context
    /// 3. Publish Local Context INTO Context container
    /// </summary>
    [CreateNodeMenu("GameSystem/Local Context Source")]
    public class LocalContextSourceNode : InOutPortNode
    {
        private EntityContext _context;

        [SerializeField]
        private AssetReferenceContextContainer localContextContainer;
        
        [Header("Data Source")] 
        [SerializeField]
        private AssetReferenceDataSource dataSources;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            _context = _context ?? new EntityContext();
            LifeTime.AddDispose(_context);
            
            var port = UniTask.FromResult<IContext>(PortPair.OutputPort);
            var contextSource = UniTask.FromResult<IContext>(_context);

            var bindContextToOutput = new MessageBroadcastCommand(_context, PortPair.OutputPort);
            //register all Context Sources Data into target context asset
            var registerDataSourceIntoContext = new RegisterDataSourceCommand(contextSource,dataSources);
            //Register context to output port
            var contextToOutputPortCommand = new DataSourceTaskCommand<IContext>(contextSource,port);
        
            nodeCommands.Add(bindContextToOutput);
            nodeCommands.Add(registerDataSourceIntoContext);
            nodeCommands.Add(contextToOutputPortCommand);
        }

        protected override async void OnExecute()
        {
            base.OnExecute();
            var container = await localContextContainer.LoadAssetTaskAsync(LifeTime);
            container.SetValue(_context);
        }

    }
}
