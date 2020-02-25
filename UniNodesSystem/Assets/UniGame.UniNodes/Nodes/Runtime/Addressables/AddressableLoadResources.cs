using UniGame.UniNodes.NodeSystem.Runtime.Core;

namespace UniGame.UniNodes.Nodes.Runtime.Addressables
{
    using System.Collections.Generic;
    using Commands;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UnityEngine.AddressableAssets;

    [CreateNodeMenu("Addressables/AddressableLoadResources","AddressableLoadResources")]
    public class AddressableLoadResources : UniNode
    {
        #region inspector
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public List<AssetReference> assetReferences = new List<AssetReference>();

        #endregion

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            var portCommand = new ConnectedPortPairCommands();
            portCommand.Initialize(this,"in","complete",false);
            
            
        }
    }
}
