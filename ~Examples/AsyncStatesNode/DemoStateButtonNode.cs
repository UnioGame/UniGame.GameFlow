using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

namespace HunterLands.Tests.Experiments.GraphExamples.States
{
    using System;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.UniGameFlow.Nodes.Runtime.States;

    [CreateNodeMenu("Examples/States/DemoStateButtonNode")]
    [Serializable]
    public class DemoStateButtonNode : AsyncStateNode
    {
        #region inspector

        [Port(PortIO.Output)]
        public object outputToken;

        #endregion

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Next()
        {
            if (Token == null)
                return;
            
            var outputTokenPort = GetPort(nameof(outputToken));
            
            PublishToken(outputTokenPort);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void StopFromNow()
        {
            Token?.StopSince(this);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void StopNext()
        {
            Token?.StopAfter(this);
        }
        
    }
}
