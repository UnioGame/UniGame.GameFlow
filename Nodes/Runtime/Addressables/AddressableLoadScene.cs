namespace UniGame.UniNodes.Nodes.Runtime.Addressables
{
    using System.Collections.Generic;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Core.Commands;
    using NodeSystem.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;

    [CreateNodeMenu("Addressables/AddressableLoadScene")]
    public class AddressableLoadScene : UniNode
    {
        private const string portName = "data";
        
        [SerializeField]
        private AssetReference sceneAsset;

        [SerializeField]
        private LoadSceneMode loadSceneMode = LoadSceneMode.Single;

        [SerializeField]
        private bool activateOnLoad = true;

        [SerializeField]
        private int priority = 100;
        
        [SerializeField]
        private bool bindInOut = true;
        
        private IPortValue input;
        private IPortValue output;
        
        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            
            var portPairCommand = new ConnectedFormatedPairCommand(this, portName, bindInOut);
            input = portPairCommand.InputPort;
            output = portPairCommand.OutputPort;
            
            nodeCommands.Add(portPairCommand);
            
        }

        protected override void OnExecute()
        {
            input.PortValueChanged.
                Where(x => input.HasValue).
                First().
                Do(async x => await sceneAsset.LoadSceneTaskAsync(LifeTime)).
                Subscribe().
                AddTo(LifeTime);
        }
        
    }
}
