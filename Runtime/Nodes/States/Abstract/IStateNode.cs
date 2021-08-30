namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using Cysharp.Threading.Tasks;

    public interface IStateNode
    {
        
        UniTask<bool> StopStateAsync();
    }
}