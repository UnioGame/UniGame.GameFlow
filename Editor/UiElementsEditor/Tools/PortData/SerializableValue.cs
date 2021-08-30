namespace UniModules.GameFlow.Editor.Tools.PortData
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SerializableValue<TValue> : ISerializableEditorValue<TValue>
    {
        [SerializeField]
        public TValue value;

        public TValue Value => value;

        public Type Type => typeof(TValue);
        
        public virtual void   Apply(object source)
        {
            if (source is TValue valueSource)
                value = valueSource;
        }
    }
}