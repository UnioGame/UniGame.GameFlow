using UniGreenModules.UniNodeSystem.Runtime;

namespace UniGreenModules.UniNodeSystem.Nodes.DebugTools
{
    using System;
    using System.Collections.Generic;
    using Commands;
    using Runtime.Core;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
    using UniNodes.Runtime.Commands;
    using UniRx;
    using UnityEngine;

    [Serializable]
    [CreateNodeMenu("Debug/Log","Log")]
    public class LogNode : UniNode , IMessagePublisher
    {
        private const string logPortName = "log";

        private IPortValue logPort;
        
        public LogMode mode = LogMode.Log;

        public string message = "LogNode";
        
        protected override void OnExecute()
        {
            PrintLog(GetMessage(), mode);
            logPort.Bind(this).
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
            PrintLog($"{message}: GRAPH:{Graph.name} : {name} \n\t {value.GetType().Name} : {value}", mode);
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
}
