namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UniGame.Core.Runtime.DataStructure;
    using UnityEngine;

    [Serializable]
    public class NodePortDictionary : SerializableDictionary<string, NodePort>, ISerializationCallbackReceiver
    {
        public IReadOnlyList<NodePort> Ports => values;

    }
}