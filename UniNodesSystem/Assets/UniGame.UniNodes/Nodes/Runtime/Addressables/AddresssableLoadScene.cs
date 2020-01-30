namespace UniGame.Nodes.Scenes
{
    using System.Collections;
    using System.Collections.Generic;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Attributes;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
    using UniGreenModules.UniNodeSystem.Nodes.Commands;
    using UniGreenModules.UniNodeSystem.Runtime;
    using UniGreenModules.UniNodeSystem.Runtime.Core;
    using UniGreenModules.UniNodeSystem.Runtime.Extensions;
    using UniGreenModules.UniNodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniRoutine.Runtime;
    using UniGreenModules.UniRoutine.Runtime.Extension;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    [CreateNodeMenu("Addressable/AddressableLoadScene")]
    public class AddresssableLoadScene : UniNode
    {
        private const string portName = "data";
        
        [ShowAssetReference]
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
            
            var portPairCommand = new ConnectedFormatedPairCommand();
            portPairCommand.Initialize(this, portName, bindInOut);
            input = portPairCommand.InputPort;
            output = portPairCommand.OutputPort;
            
            nodeCommands.Add(portPairCommand);
            
        }

        protected override void OnExecute()
        {
            input.PortValueChanged.
                Where(x => input.HasValue).
                Do(async x => await sceneAsset.LoadSceneTaskAsync()).
                Subscribe().
                AddTo(LifeTime);
        }
        
    }
}
