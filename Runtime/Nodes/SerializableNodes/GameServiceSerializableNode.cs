using System;
using UniGame.GameFlow.Runtime.Interfaces;
using UniModules.GameFlow.Runtime.Attributes;

namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    [Serializable]
    [HideNode]
    public abstract class GameServiceSerializableNode<TServiceApi> : 
        ServiceSerializableNode<TServiceApi>
        where TServiceApi : class, IGameService
    {
    }
}