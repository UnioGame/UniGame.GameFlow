namespace UniModules.GameFlow.Runtime.Core.Extensions
{
    using Runtime.Interfaces;

    public static class PortExtensions 
    {
    
        public static bool IsTarget(this IPortConnection connection, INodePort target)
        {
            return connection.NodeId == target.NodeId && connection.PortName == target.ItemName;
        }
        
        public static bool IsEqual(this INodePort first, INodePort second)
        {
            return first.NodeId == second.NodeId && first.ItemName == second.ItemName;
        }
    }
}
