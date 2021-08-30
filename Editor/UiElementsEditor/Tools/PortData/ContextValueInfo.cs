namespace UniModules.GameFlow.Editor.Tools.PortData
{
    using System;
    using System.Linq;
    using Object = UnityEngine.Object;

    [Serializable]
    public class ContextValueInfo
    {
        public string TypeValue = String.Empty;
        
        public Type Type;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
#endif
        public string TextValue = string.Empty;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()]
        [Sirenix.OdinInspector.ShowIf(nameof(IsAsset))]
#endif   
        public Object AssetValue;

        public bool IsAsset => AssetValue != null;

        public ContextValueInfo Update(object value, Type type)
        {
            Type       = type;
            var generics = type.GenericTypeArguments;
            TypeValue  = $"Type : [ {type.Name} {string.Join(" ",generics.Select(x => x.Name))}] Value {value}";
            AssetValue = value as Object;
            TextValue  = value.ToString();
            return this;
        }
    }
}