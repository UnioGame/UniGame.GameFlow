using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
using UniGreenModules.UniNodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ReactivePortDemo
{
    using System.Collections;
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core.Commands;
    using NodeSystem.Runtime.ReactivePorts;
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

        private IEnumerator Generate(float delay, Vector2Int range, IMessagePublisher messagePublisher)
        {
            while (isActiveAndEnabled) {

                var value = Random.Range(range.x, range.y);
                messagePublisher.Publish(value);
                
                yield return this.WaitForSecond(delay);

            }
        }
    }
}
