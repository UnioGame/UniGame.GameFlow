using UniModules.GameFlow.Runtime.Core;

namespace HunterLands.Tests.Experiments.GraphExamples
{
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

    [CreateNodeMenu("Examples/Nodes/DemoStreamInOut")]
    public class DemoStreamInOut : UniNode
    {

        [Port(PortIO.Input)]
        public int inPort;

        [Port(PortIO.Output)]
        public int outPort;

        public int data;


#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void FireInputData()
        {
            GetPort(nameof(inPort)).Value.Publish(data);   
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void FireOutputData()
        {
            GetPort(nameof(outPort)).Value.Publish(data);   
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void ClearData()
        {
            GetPort(nameof(inPort)).Value.Remove<int>();   
            GetPort(nameof(outPort)).Value.Remove<int>();   
        }
    }
}
