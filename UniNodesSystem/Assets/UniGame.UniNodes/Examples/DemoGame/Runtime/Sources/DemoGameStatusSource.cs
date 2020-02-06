using UnityEngine;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.DemoGame.Runtime.Sources
{
    using Models;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Abstract;

    [CreateAssetMenu(fileName = nameof(DemoGameStatusSource),menuName = "UniGame/NodeSystem/Examples/DemoGame/DemoGameStatusSource")]
    public class DemoGameStatusSource : TypeValueSource<DemoGameStatusData>
    {
        
    }
}
