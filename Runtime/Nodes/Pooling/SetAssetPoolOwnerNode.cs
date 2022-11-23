using Cysharp.Threading.Tasks;
using UniGame.UniNodes.Nodes.Runtime.Common;
using UniGame.Runtime.ObjectPool;
using UniGame.Core.Runtime;
using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
using UniRx;
using UnityEngine;

[CreateNodeMenu("Common/Pooling/Set Owner", nodeName = "SetAssetPoolOwner")]
public class SetAssetPoolOwnerNode : InOutPortBindNode
{

    #region inspector
    
    public GameObject owner;

    #endregion
    
    protected override UniTask OnExecute()
    {
        var poolLifeTime = owner ? owner.GetLifeTime() : LifeTime;
        poolLifeTime.ApplyPoolAssetLifeTime();

        return UniTask.CompletedTask;
    }
}
