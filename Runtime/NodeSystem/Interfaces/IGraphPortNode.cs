namespace UniModules.GameFlow.Runtime.Interfaces
{
    using Core;

    public interface IGraphPortNode : IUniNode
    {
        
        PortIO Direction { get; }
        
        IPortValue PortValue { get; }

        bool Visible { get; }

    }
}