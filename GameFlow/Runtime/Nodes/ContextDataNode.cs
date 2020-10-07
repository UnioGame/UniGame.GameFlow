namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using Cysharp.Threading.Tasks;
    using NodeSystem.Runtime.Core;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniNodes.Nodes.Runtime.Common;
    
    using UnityEngine;

    /// <summary>
    /// 1. Register ALL contextDataSources Source into target  contextAsset
    /// 2. Publish ALL contextDataSources INTO Output port
    /// 3. Publish target Context INTO Output port
    /// </summary>
    [CreateNodeMenu("GameSystem/Context Source")]
    public class ContextDataNode : InOutPortNode
    {
        [Header("Context")]
        public ContextAssetReference contextAsset;

        [Header("Data Source")] 
        public AssetReferenceDataSource contextDataSources;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            var outputContextTask = UniTask.FromResult<IContext>(PortPair.OutputPort);
            
            //register all Context Sources Data into target context asset
            var registerDataSourceIntoContext = new RegisterDataSourceToContextAssetCommand(contextAsset,contextDataSources);
            //Register All Sources data into Output port
            var sourceOutputPortCommand = new RegisterDataSourceCommand(outputContextTask,contextDataSources);
            //Register context to output port
            var contextToOutputPortCommand = new RegisterDataSourceCommand(outputContextTask,contextAsset);
            
            nodeCommands.Add(registerDataSourceIntoContext);
            nodeCommands.Add(sourceOutputPortCommand);
            nodeCommands.Add(contextToOutputPortCommand);
        }

        protected sealed override void OnExecute()
        {
            //unload all source addressables
            LifeTime.AddDispose(contextAsset);
            LifeTime.AddDispose(contextDataSources);
        }
    }
}
