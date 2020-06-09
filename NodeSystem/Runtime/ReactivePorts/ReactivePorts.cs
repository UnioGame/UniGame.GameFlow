namespace UniGame.UniNodes.NodeSystem.Runtime.ReactivePorts
{
    using System;
    using Core;
    using UniGreenModules.UniCore.Runtime.Interfaces;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public class IntReactivePort : ReactivePortValue<int>
    {
        
    }
    
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public class StringReactivePort : ReactivePortValue<string> {}
    
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public class FloatReactivePort : ReactivePortValue<float> {}
    
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public class DoubleReactivePort : ReactivePortValue<double> {}
    
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public class ByteReactivePort : ReactivePortValue<byte> {}
    
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public class BoolReactivePort : ReactivePortValue<bool> {}

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    [Serializable]
    public class ContextReactivePort : ReactivePortValue<IContext> { }

}
