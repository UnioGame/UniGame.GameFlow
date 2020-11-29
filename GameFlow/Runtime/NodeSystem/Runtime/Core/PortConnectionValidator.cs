namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;

    public class PortConnectionValidator : IPortConnectionValidator
    {
        [NonSerialized]
        private IReadOnlyList<Func<INodePort, INodePort, bool>> connectionsValidators;
        
        public IReadOnlyList<Func<INodePort, INodePort, bool>> ConnectionsValidators => connectionsValidators = 
            connectionsValidators ?? 
            new List<Func<INodePort, INodePort, bool>>() {
                IsNullPort,
                IsSavePort,
                IsAlreadyConnectedPort,
                IsSameDirection,
                TypeValidation,
            };
        
        public bool Validate(INodePort from, INodePort to)
        {
            return ConnectionsValidators.All(x => x(from, to));
        }
        
        public bool IsAlreadyConnectedPort(INodePort from, INodePort to)
        {
            var result = from.IsConnectedTo(to);
            return !result;
        }
        
        public bool TypeValidation(INodePort from, INodePort to)
        {
            var result = from.ValueTypes.Count == 0 || to.ValueTypes.Count == 0 ||
                         from.ValueTypes.Any(to.Value.IsValidPortValueType);
            if (result) return true;

            ReportError(from,to,$"CAN'T CONNECT BY TYPE VALIDATION");
            
            return false;
        }
        
        public bool IsSameDirection(INodePort from, INodePort to)
        {
            var result = from.Direction != to.Direction;
            if (result) return true;

            ReportError(from,to,$"SAME PORT DIRECTIONS {from.Direction}");
            
            return false;
        }

        public bool IsSavePort(INodePort from, INodePort to)
        {
            var result = to != from;
            if (result) return true;

            ReportError(from,to,"SAME PORT ERROR");
            return false;
        }

        public bool IsNullPort(INodePort from, INodePort to)
        {
            if (from != null && to != null)
                return true;
            if (from == null) {
                ReportError(from, to, "FROM port is NULL");
            }
            if (to == null) {
                ReportError(from, to, "TO port is NULL");
            }

            return false;
        }

        public void ReportError(INodePort from, INodePort to,string message)
        {
            GameLog.LogError($"CONNECTION :FROM {from?.ItemName} TO {to?.ItemName} Error. {message} Validation Failed");
        }
    }
}