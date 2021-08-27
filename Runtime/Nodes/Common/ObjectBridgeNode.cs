namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.UniGame.Core.Runtime.Attributes.FieldTypeDrawer;

    [HideNode]
    public class ObjectBridgeNode<T> : TypeBridgeNode<T>
        where T : class
    {
        
        [HideNodeInspector]
        public bool skipEmptyValue = true;

    }
}
