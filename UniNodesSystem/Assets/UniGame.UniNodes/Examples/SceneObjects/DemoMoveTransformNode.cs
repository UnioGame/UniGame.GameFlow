﻿namespace UniGame.UniNodes.Examples.SceneObjects
{
    using System;
    using System.Collections;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniRoutine.Runtime;
    using UniRx;
    using UnityEngine;

    [CreateNodeMenu("Examples/SceneObjects/DemoMoveTransformNode", "DemoMoveTransformNode")]
    public class DemoMoveTransformNode : UniNode
    {
        #region inspector
        
        public Transform target;

        public float limit;

        public Vector3 direction = Vector3.up;

        [ReadOnlyValue]
        public Vector3 startPosition;
        
        #endregion

        [PortValue(PortIO.Input)]
        public Transform targetSource;

        [PortValue(PortIO.Output)]
        public Transform targetOutput;
        
        private IDisposable disposable;
        
        protected override void OnExecute()
        {

            var input = GetPortValue(nameof(targetSource));
            var output = GetPortValue(nameof(targetOutput));
            
            input.Receive<Transform>().
                Where(x => x).
                Do(x => disposable.Cancel()).
                Do(x => disposable = Move(x).ExecuteRoutine()).
                DoOnCompleted(() => disposable.Cancel()).
                Subscribe().
                AddTo(LifeTime);

            if (!target) {
                return;
            }

            disposable = Move(target).ExecuteRoutine();
            output.Publish(target);

        }

        private IEnumerator Move(Transform targetTransform)
        {
            startPosition = targetTransform.position;
            var mult = 1;
            while (isActiveAndEnabled) {
                
                var targetPosition = targetTransform.position;
                var y = targetPosition.y;
                var distance = Vector3.Distance(startPosition, targetPosition);
                
                mult = distance > limit ? 1 : -1 ;
                targetTransform.position = targetPosition + direction * (mult * Time.deltaTime);
                yield return null;

            }
        }
        
    }
}
