using System;
using GraphProcessor;
using UniRx;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "UniGame/GameFlow/ScriptableGraph",fileName = "ScriptableGameFlow")]
public class GameFlowGraph : BaseGraph
{

    public void Initialize()
    {
        
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