using System;
using GraphProcessor;
using UnityEngine;

[Serializable]
[NodeMenuItem("Examples/LogNode")]
public class DemoLogNode : BaseNode
{

    [Input(name = nameof(input))]
    public object input;

    public override string name => "debug log";

    protected override void Process()
    {
        var value = input == null ? string.Empty : input.ToString();
        Debug.Log($"{input?.GetType()} VALUE = {value}");
    }
    
}
