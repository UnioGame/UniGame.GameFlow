using Cysharp.Threading.Tasks;
using UniModules.UniGame.Context.Runtime.Connections;

namespace UniModules.GameFlow.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using UnityEngine;

    public interface IUniGraph : INodeGraph, IUniNode
    {

        GameObject AssetInstance { get; }

        IReadOnlyList<IGraphPortNode> OutputsPorts { get; }

        IReadOnlyList<IGraphPortNode> InputsPorts { get; }

        UniTask ExecuteAsync(IContextConnection context);

    }
    
}