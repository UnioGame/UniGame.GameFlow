namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime.Nodes
{
    using GameFlow.Runtime.Nodes;
    using NodeSystem.Runtime.Core;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

    [CreateNodeMenu("Examples/DemoSystem/Simple3")]
    public class Simple3Node : GameServiceNode<SimpleSystem3>
    {
    }
}
