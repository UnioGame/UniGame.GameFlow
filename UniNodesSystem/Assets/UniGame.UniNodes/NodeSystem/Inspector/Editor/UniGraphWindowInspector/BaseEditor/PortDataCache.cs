namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System.Collections.Generic;
    using UniGame.Core.Runtime.DataStructure;
    using UnityEngine;

    [System.Serializable]
    public class PortDataCache : SerializableDictionary<System.Type, List<NodePort>>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<System.Type>    keys   = new List<System.Type>();
        [SerializeField] private List<List<NodePort>> values = new List<List<NodePort>>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in this) {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (var i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }
}