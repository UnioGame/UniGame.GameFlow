using System.Collections;
using System.Collections.Generic;
using UniGame.UniNodes.Nodes.Runtime.Common;
using UniGame.UniNodes.NodeSystem.Runtime.Core;
using UniGreenModules.UniCore.Runtime.Interfaces;
using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateNodeMenu("Common/Scenes/LoadScene",nodeName = "LoadScene")]
public class LoadSceneNode : ContextNode
{
    [SerializeField]
    private int _sceneIndex;

    [SerializeField]
    private LoadSceneMode _loadMode = LoadSceneMode.Single;

    protected override void OnContextActivate(IContext context)
    {
        SceneManager.LoadScene(_sceneIndex, _loadMode);
    }

}
