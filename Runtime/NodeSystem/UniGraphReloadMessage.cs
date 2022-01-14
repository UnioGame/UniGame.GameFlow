using System;

namespace UniModules.GameFlow.Runtime.Core
{
    [Serializable]
    public struct UniGraphReloadMessage
    {
        public UniGraph graph;
    }
    
    [Serializable]
    public struct UniGraphSaveMessage
    {
        public UniGraph graph;
    }
}