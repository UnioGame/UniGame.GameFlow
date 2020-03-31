namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using NodeSystem.Runtime.Attributes;
    using UniGreenModules.UniGame.Core.Runtime.Attributes.FieldTypeDrawer;

    [HideNode]
    public class ObjectBridgeNode<T> : TypeBridgeNode<T>
        where T : class
    {
        
        [HideNodeInspector]
        public bool skipEmptyValue = true;

    }
}
