namespace UniGame.UniNodes.Nodes.Runtime.DebugTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Nodes;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;
    using UnityEngine;
    
    [CreateNodeMenu("Debug/Log","Log")]
    [NodeInfo("Logging Node","Profiling","Logging all data from input port")]
    public class LogNode : UniNode , IMessagePublisher
    {
        private const string logPortName = "log";
        
        public LogMode mode = LogMode.Log;

        public string message = "LogNode";

        private IPortValue logPort;

        protected override void OnExecute()
        {
            PrintLog(GetMessage(), mode);
            logPort.Broadcast(this).
                AddTo(LifeTime);
        }

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            logPort = this.UpdatePortValue(logPortName, PortIO.Input);
        }

        protected virtual string GetMessage()
        {
            return message;
        }

        public void Publish<T>(T value)
        {
            PrintLog($"{message}: GRAPH:{GraphData.ItemName} : {name} \n\t TYPE {typeof(T)} {value?.GetType().Name} : {value}", mode);
        }
        
        private void PrintLog(string messageData, LogMode logMode)
        {
            switch (logMode) {
                case LogMode.Runtime:
                    GameLog.LogRuntime(messageData);
                    break;
                case LogMode.Log:
                    GameLog.Log(messageData);
                    break;
                case LogMode.Warning:
                    GameLog.LogWarning(messageData);
                    break;
                case LogMode.Error:
                    GameLog.LogError(messageData);
                    break;
                case LogMode.Exception:
                    GameLog.LogError(messageData);
                    break;
            }
        }

    }

    [Serializable]
    public class DemoComponentNode : SNode
    {
        [Port(PortIO.Input)]
        public int intPort;
        
        protected override void OnExecute()
        {
            var intPortHandle = GetPort(nameof(intPort));
            var portValue     = intPortHandle.Value;
            portValue.Receive<int>().
                Subscribe(x => Debug.Log($"RECEIVE INT VALUE {x}")).
                AddTo(LifeTime);
        }
        
    }
}
