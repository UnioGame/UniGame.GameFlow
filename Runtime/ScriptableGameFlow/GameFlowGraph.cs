using System;
using GraphProcessor;
using UniModules.UniCore.Runtime.DataFlow;
using UniModules.UniGame.Context.Runtime.Context;
using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniRx;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "UniGame/GameFlow/ScriptableGraph",fileName = "ScriptableGameFlow")]
public class GameFlowGraph : BaseGraph, ILifeTimeContext
{
    private ILifeTime     _lifeTime;
    private EntityContext _context;

    public ILifeTime LifeTime => _lifeTime;

    public IContext Context => _context;
    
    public void Initialize()
    {
        _context  ??= new EntityContext();
        _context.Release();
        _lifeTime = _context.LifeTime;
    }
    
    #region editor api

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button]
#endif
    public void OpenWindow()
    {
        MessageBroker.Default.Publish(new OpenGameFlowGraphMessage()
        {
            gameFlowGraph = this
        });
    }
    
    #endregion
    
}