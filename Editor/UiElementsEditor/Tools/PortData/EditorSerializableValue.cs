using UniGame.Core.Runtime;

namespace UniModules.GameFlow.Editor.Tools.PortData
{
    using System;
    using UniCore.Runtime.Utils;
    using Object = UnityEngine.Object;

    public static class EditorSerializableValue
    {
        private static MemorizeItem<Type,ISerializableObject> serializableItemFactory = 
            new MemorizeItem<Type, ISerializableObject>(CreateSerializableObject);
        
        
        public static ISerializableObject Create(object value, Type type)
        {
            return null;
        }

        private static ISerializableObject CreateSerializableObject(Type type)
        {
            return null;
        }
        
    }
    
    

    public class IntSerializableValue : SerializableValue<int> {}
    public class FloatSerializableValue : SerializableValue<float> {}
    public class StringSerializableValue : SerializableValue<string> {}
    public class AssetSerializableValue : SerializableValue<Object> {}

}