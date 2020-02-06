namespace UniGreenModules.UniNodeSystem.Nodes
{
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.ObjectPool;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;

    [HideNode]
    public class SyncGraphNode : UniGraphNode
    {
        public UniGraph graphAsset;

        private UniGraph graphInstance;
        
        public override UniGraph LoadOrigin() => graphAsset;

        protected override UniGraph CreateGraph(ILifeTime lifeTime)
        {
            if (graphInstance) return graphInstance;
            graphInstance = graphAsset.Spawn();

            lifeTime.AddCleanUpAction(() => graphInstance?.Despawn());
            return graphAsset;
        }
    }
}
