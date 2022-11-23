namespace UniModules.GameFlow.Runtime.Interfaces
{
    using global::UniGame.Core.Runtime;

    public interface INodeCommand : ICommand
    {
        /// <summary>
        /// attach to target node, create all ports or data
        /// </summary>
        /// <param name="targetNode"></param>
        void AttachToNode(IUniNode targetNode);

    }
}