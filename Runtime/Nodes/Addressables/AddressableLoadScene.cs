using UniGame.AddressableTools.Runtime;
using UniGame.Context.Runtime;

namespace UniGame.UniNodes.Nodes.Runtime.Addressables
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Commands;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Extension;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using Core.Runtime;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;

    [CreateNodeMenu("Addressable/AddressableLoadScene")]
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

        protected override async UniTask OnExecute()
        {
            await input.PortValueChanged
                .Where(x => input.HasValue)
                .AwaitFirstAsync(LifeTime);

            await sceneAsset.LoadSceneTaskAsync(LifeTime)
                .AttachExternalCancellation(LifeTime.TokenSource);
        }
        
    }
}
