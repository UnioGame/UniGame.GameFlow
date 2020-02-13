using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
using UniGreenModules.UniNodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ReactivePortDemo
{
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
        private Vector2Int range = new Vector2Int();
        
        #endregion
        
        [Space]
        [ReactivePort(PortIO.Output)]
        public IntReactivePort IntOut = new IntReactivePort();

        protected override void OnExecute()
        {
            Generate(delay,range,IntOut).
                Execute().
                AddTo(LifeTime);
        }

        private IEnumerator Generate(float delayTime, Vector2Int rangeValue, IReactivePortValue<int> reactivePort)
        {
            while (isActiveAndEnabled) {

                var value = Random.Range(rangeValue.x, rangeValue.y);
                reactivePort.SetValue(value);
                
                yield return this.WaitForSecond(delayTime);

            }
        }
    }
}
