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
    using Nodes;

    [Serializable]
    public class ReactiveValuePortCommand : SerializedNodeCommand, ILifeTimeCommand
    {
        #region inspector

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor]
#endif
        [SerializeField] public UniNode      node;
        [SerializeField] public NodePortData portData;
        [SerializeField] public bool         isUpdatable = true;
        [SerializeField] public string fieldName;

        [SerializeReference] public IReactiveSource targetSource;

        #endregion

        protected IPortValue port;

        public override bool IsUpdatable => isUpdatable;

        public void Initialize(UniNode target, IReactiveSource source, IPortData portData, bool updatable = true)
        {
            this.node       = target;
            this.portData     = new NodePortData(portData);
            this.targetSource = source;
            this.isUpdatable  = updatable;
            this.fieldName = portData.FieldName;
        }

        public override ILifeTimeCommand Create(IUniNode node)
        {
            port = node.UpdatePortValue(portData);
            return this;
        }

        public override bool Validate()
        {
            return string.IsNullOrEmpty(portData.FieldName) == false &&
                   node != null &&
                   node.GetPort(portData.FieldName) != null;
        }

        public void Execute(ILifeTime lifeTime)
        {
            var portDirection = portData.Direction;

            switch (portDirection) {
                case PortIO.Input:
                    port.Receive<>()
                    break;
                case PortIO.Output:
                    targetSource.Bind(port).
                        AddTo(lifeTime);
                    break;
            }
            var source      = portDirection == PortIO.Input ? (IConnector<IMessagePublisher>) port : targetSource;
            var targetValue = portDirection == PortIO.Input ? targetSource : (IMessagePublisher) port;

        }
    }
}