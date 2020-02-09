namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class NodePortDictionary : Dictionary<string, NodePort>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<string>   keys   = new List<string>();
        [SerializeField] private List<NodePort> values = new List<NodePort>();

        public IReadOnlyList<NodePort> Ports => values;
        
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            if (keys.Count != values.Count)
                throw new System.Exception("there are " + keys.Count + " keys and " + values.Count +
                                           " values after deserialization. Make sure that both key and value types are serializable.");

            for (var i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }
}