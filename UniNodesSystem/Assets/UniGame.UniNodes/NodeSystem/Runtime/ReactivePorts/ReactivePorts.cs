using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.ReactivePorts
{
    using System;
    using UniContextData.Runtime.Entities;
    using UniRx;

    [Serializable]
    public class IntReactivePort : ReactivePropertyPort<IntReactiveProperty,int>{}
    
    [Serializable]
    public class StringReactivePort : ReactivePropertyPort<StringReactiveProperty,string> {}
    
    [Serializable]
    public class FloatReactivePort : ReactivePropertyPort<FloatReactiveProperty,float> {}
    
    [Serializable]
    public class DoubleReactivePort : ReactivePropertyPort<DoubleReactiveProperty,double> {}
    
    [Serializable]
    public class ByteReactivePort : ReactivePropertyPort<ByteReactiveProperty,byte> {}
    
    [Serializable]
    public class BoolReactivePort : ReactivePropertyPort<BoolReactiveProperty,bool> {}
    
    [Serializable]
    public class ContextReactivePort : ReactivePropertyPort<ContextReactiveProperty,EntityContext> {}

    [Serializable]
    public class ContextReactiveProperty : ReactiveProperty<EntityContext>{}
}
