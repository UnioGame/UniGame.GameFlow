using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.Extensions;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.GameFlow.Runtime.Commands;
    using global::UniGame.UniNodes.Nodes.Runtime.Commands;
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using NodeSystem.Runtime.Core.Attributes;
    using UniGame.Context.Runtime.Context;
    using UniGame.Core.Runtime.Interfaces;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UniRx;
    using UnityEngine;

    [CreateNodeMenu("Common/Sources/Graph Source Node")]
    public class GraphSourceNode : InOutPortNode
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [SerializeField]
        private AssetReferenceContextContainer contextContainer;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [Header("Data Source")]
        [SerializeField]
        private AssetReferenceDataSource dataSources;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            var context       = Context;
            var port          = UniTask.FromResult<IContext>(PortPair.OutputPort);
            var contextSource = UniTask.FromResult<IContext>(context);

            var bindContextToOutput = new MessageBroadcastCommand(context, PortPair.OutputPort);
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
            if (contextContainer.RuntimeKeyIsValid() == false)
                return;
            
            var container = await contextContainer.LoadAssetTaskAsync(LifeTime);
            container.SetValue(Context);
        }
    }
    
}