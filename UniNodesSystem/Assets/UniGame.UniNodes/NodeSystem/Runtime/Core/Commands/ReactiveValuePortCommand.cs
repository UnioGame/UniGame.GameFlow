using System;
using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
using UniGreenModules.UniCore.Runtime.Interfaces;
using UniGreenModules.UniCore.Runtime.Rx.Extensions;
using UniGreenModules.UniNodeSystem.Runtime.Commands;
using UniGreenModules.UniNodeSystem.Runtime.Core;
using UniGreenModules.UniNodeSystem.Runtime.Extensions;
using UniGreenModules.UniNodeSystem.Runtime.Interfaces;
using UnityEngine;

[Serializable]
public class ReactiveValuePortCommand : SerializedNodeCommand, ILifeTimeCommand
{
    [SerializeField]
    protected string portName;
    [SerializeReference]
    protected IReactiveSource target;
    [SerializeReference]
    protected IReactiveSource port;
    [SerializeField]
    protected PortIO portDirection;

    public ReactiveValuePortCommand(string portName,IReactiveSource target, PortIO portDirection)
    {
        this.portName = portName;
        this.target = target;
        this.portDirection = portDirection;
    }
    
    public override ILifeTimeCommand Create(IUniNode node)
    {
        port = node.UpdatePortValue(portName, portDirection);
        return this;
    }

    public void Execute(ILifeTime lifeTime)
    {
        var source      = portDirection == PortIO.Input ? port : target;
        var targetValue = portDirection == PortIO.Input ? target : port;
        
        source.Bind(targetValue).AddTo(lifeTime);
    }
}
