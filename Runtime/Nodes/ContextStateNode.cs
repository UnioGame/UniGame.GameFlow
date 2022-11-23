namespace Game.Modules.Assets.UniGame.GameFlow.Runtime.Nodes
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.Context.SerializableContext.Runtime.Abstract;
    using global::UniGame.Core.Runtime;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;

    [CreateNodeMenu("Common/States/ContextStateNode")]
    public class ContextStateNode : ContextNode
    {

        [SerializeReference]
        public IAsyncContextState contextState;

        public bool completeOnFalse = true;
        
        protected sealed override async UniTask OnContextActivate(IContext context)
        {
#if UNITY_EDITOR
            if (contextState == null)
            {
                GameLog.LogError($"NODE: {nameof(ContextStateNode)} : {name} contextState is NULL");
                return;
            }
#endif

            var result = await contextState.ExecuteAsync(context);

            switch (result)
            {
                case AsyncStatus.Succeeded:
                case {} when completeOnFalse == true:
                    Complete();
                    break;
                default:
                    GameLog.LogError($"NODE: {nameof(ContextStateNode)} : {name} STATUS {result}");
                    break;
            }
            
        }
    }
}