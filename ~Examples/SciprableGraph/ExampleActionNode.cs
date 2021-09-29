using System;
using System.Collections.Generic;
using UniModules.GameFlow.Runtime.Attributes;
using UniModules.GameFlow.Runtime.Core;
using UniModules.GameFlow.Runtime.Core.Nodes;
using UniModules.GameFlow.Runtime.Extensions;
using UniModules.GameFlow.Runtime.Interfaces;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

[Serializable]
[CreateNodeMenu("Examples/ActionTest/SampleActionNode")]
public class ExampleActionNode : SNode
{

    [Port(PortIO.Output)]
    public string outputValue;

    public string message = nameof(outputValue);

    private IPortValue _outputPortValue;
    
    protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
    {
        base.UpdateCommands(nodeCommands);
        _outputPortValue = this.UpdatePortValue(nameof(outputValue), PortIO.Input);
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button]
#endif
    public void Fire()
    {
        _outputPortValue.Publish(message);
    }
    
}
