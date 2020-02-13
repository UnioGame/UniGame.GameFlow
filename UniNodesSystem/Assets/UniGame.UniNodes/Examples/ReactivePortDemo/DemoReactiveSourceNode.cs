using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
using UniGreenModules.UniNodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ReactivePortDemo
{
    using System;
    using System.Collections;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core.Interfaces;
    using NodeSystem.Runtime.ReactivePorts;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniRoutine.Runtime;
    using UniRoutine.Runtime.Extension;
    using UniRx;
    using UnityEngine;

    [CreateNodeMenu("Examples/ReactivePortDemo/ReactiveSource","ReactiveSource")]
    public class DemoReactiveSourceNode : UniNode
    {
        
        #region inspector

        [SerializeField]
        private float delay = 2f;
        
        [SerializeField]
        private int value = 1;
        
        #endregion
        
        [Space]
        [ReactivePort(PortIO.Output)]
        public IntReactivePort IntOut = new IntReactivePort();

        protected override void OnExecute()
        {
            Observable.Interval(TimeSpan.FromSeconds(delay)).
                Do(x => IntOut.SetValue(value)).
                Subscribe().
                AddTo(LifeTime);
        }

    }
}
