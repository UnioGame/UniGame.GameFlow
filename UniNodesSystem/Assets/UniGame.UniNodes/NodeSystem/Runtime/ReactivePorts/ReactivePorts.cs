﻿using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.ReactivePorts
{
    using System;
    using UniContextData.Runtime.Entities;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniRx;

    [Serializable]
    public class IntReactivePort : ReactivePortValue<IntReactiveProperty,int>{}
    
    [Serializable]
    public class StringReactivePort : ReactivePortValue<StringReactiveProperty,string> {}
    
    [Serializable]
    public class FloatReactivePort : ReactivePortValue<FloatReactiveProperty,float> {}
    
    [Serializable]
    public class DoubleReactivePort : ReactivePortValue<DoubleReactiveProperty,double> {}
    
    [Serializable]
    public class ByteReactivePort : ReactivePortValue<ByteReactiveProperty,byte> {}
    
    [Serializable]
    public class BoolReactivePort : ReactivePortValue<BoolReactiveProperty,bool> {}
    
    [Serializable]
    public class ContextReactivePort : ReactivePortValue<ContextReactiveProperty,EntityContext> {}

    [Serializable]
    public class ContextReactiveProperty : ReactiveProperty<EntityContext>{}
}