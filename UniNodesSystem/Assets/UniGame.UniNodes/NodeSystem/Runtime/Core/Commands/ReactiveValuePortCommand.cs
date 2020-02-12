using System;
using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
using UniGreenModules.UniCore.Runtime.Interfaces;
using UniGreenModules.UniCore.Runtime.Rx.Extensions;
using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces;
using UniGreenModules.UniNodeSystem.Runtime.Commands;
using UniGreenModules.UniNodeSystem.Runtime.Core;
using UniGreenModules.UniNodeSystem.Runtime.Extensions;
using UniGreenModules.UniNodeSystem.Runtime.Interfaces;
using UniRx;
using UnityEngine;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Commands
{
    [Serializable]
    public class ReactiveValuePortCommand : SerializedNodeCommand, ILifeTimeCommand
    {
        #region inspector
    
        [SerializeField]
        protected string portName;
        [SerializeReference]
        protected IReactiveSource target;
        [SerializeReference]
        protected IPortData portData;

        #endregion
    
        protected IPortValue port;
    
        public ReactiveValuePortCommand(string portName,IReactiveSource target, IPortData portData, bool updatable = true)
        {
            this.portName    = portName;
            this.target      = target;
            this.portData    = portData;
            this.isUpdatable = updatable;
        }
    
        public override ILifeTimeCommand Create(IUniNode node)
        {
            port = node.UpdatePortValue(portData);
            return this;
        }

        public void Execute(ILifeTime lifeTime)
        {
            var portDirection = portData.Direction;
            var source        = portDirection == PortIO.Input ? (IConnector<IMessagePublisher>)port : target;
            var targetValue   = portDirection == PortIO.Input ? target : (IMessagePublisher)port;
        
            source.Bind(targetValue).AddTo(lifeTime);
        }
    }
}
