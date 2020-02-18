namespace UniGame.UniNodes.NodeSystem.Runtime.ReactivePorts
{
    using System;
    using Core;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    [Serializable]
    public class IntReactivePort : ReactivePortValue<int>
    {
        
    }
    
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
    public class ContextReactivePort : ReactivePortValue<IContext> { }

}
