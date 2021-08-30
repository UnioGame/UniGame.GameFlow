using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UnityEngine;

[System.Serializable, NodeMenuItem("GameFlow/FlowServiceNode")]
public class FlowServiceNode : BaseFlowNode
{
    [Input(name = "Condition")]
    public IContext	input;

    [Output(name = "Condition")]
    public IContext	output;

    public override string		name => nameof(FlowServiceNode);

    protected override void Process()
    {
        base.Process();
    }
}

[System.Serializable]
public class BaseFlowNode : BaseNode
{
    public IContext graphContext;
    
    
    public override string		name => nameof(FlowServiceNode);

    protected override void Process()
    {
        base.Process();
    }
}
