using GraphProcessor;

[System.Serializable]
public class BaseFlowNode : BaseNode
{
    public override string name => nameof(BaseFlowNode);

    protected override void Process()
    {
        base.Process();
    }

    public sealed override void InitializePorts()
    {
        base.InitializePorts();
        InitializeFlowPorts();
    }
    
    #region private ports

    protected virtual void InitializeFlowPorts()
    {
    }   
    
    #endregion
}
