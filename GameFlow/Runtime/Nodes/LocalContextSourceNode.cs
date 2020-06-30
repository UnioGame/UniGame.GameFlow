using System.Collections.Generic;
using UniGame.UniNodes.GameFlow.Runtime.Commands;
using UniGame.UniNodes.Nodes.Runtime.Common;
using UniGame.UniNodes.NodeSystem.Runtime.Core;
using UniGreenModules.UniCore.Runtime.Interfaces;
using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
using UniRx.Async;
using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Nodes
{
    using NodeSystem.Runtime.Core.Attributes;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UniGreenModules.UniContextData.Runtime.Entities;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Abstract;

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
            
            //register all Context Sources Data into target context asset
            var registerDataSourceIntoContext = new RegisterDataSourceCommand(contextSource,dataSources);
            //Register All Sources data into Output port
            var sourceOutputPortCommand = new RegisterDataSourceCommand(port,dataSources);
            //Register context to output port
            var contextToOutputPortCommand = new DataSourceTaskCommand<IContext>(contextSource,port);
        
            nodeCommands.Add(registerDataSourceIntoContext);
            nodeCommands.Add(sourceOutputPortCommand);
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
