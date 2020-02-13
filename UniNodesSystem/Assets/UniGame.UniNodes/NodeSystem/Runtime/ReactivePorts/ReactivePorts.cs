using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.ReactivePorts
{
    using System;
    using UniContextData.Runtime.Entities;
    using UniRx;

    [Serializable]
    public class IntReactivePort : ReactivePortValue<int>{}
    
    [Serializable]
    public class StringReactivePort : ReactivePortValue<string> {}
    
    [Serializable]
    public class FloatReactivePort : ReactivePortValue<float> {}
    
    [Serializable]
    public class DoubleReactivePort : ReactivePortValue<double> {}
    
    [Serializable]
    public class ByteReactivePort : ReactivePortValue<byte> {}
    
    [Serializable]
    public class BoolReactivePort : ReactivePortValue<bool> {}
    
    [Serializable]
    public class ContextReactivePort : ReactivePortValue<EntityContext> {}

    [Serializable]
    public class ContextReactiveProperty : ReactiveProperty<EntityContext>{}
}
