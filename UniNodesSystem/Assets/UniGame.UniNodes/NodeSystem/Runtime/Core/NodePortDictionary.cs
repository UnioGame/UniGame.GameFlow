namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UniGame.Core.Runtime.DataStructure;

    [Serializable]
    public class NodePortDictionary : SerializableDictionary<string, NodePort>
    {
        public IReadOnlyList<NodePort> Ports => values;

    }
}