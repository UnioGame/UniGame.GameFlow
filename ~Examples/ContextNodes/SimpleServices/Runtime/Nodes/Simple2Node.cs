namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime.Nodes
{
    using GameFlow.Runtime.Nodes;
    using NodeSystem.Runtime.Core;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

    [CreateNodeMenu("Examples/DemoSystem/Simple2")]
    public class Simple2Node : GameServiceNode<SimpleSystem2>
    {
    }
}
