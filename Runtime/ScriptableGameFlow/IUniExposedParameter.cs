namespace UniGame.GameFlowEditor.Runtime
{
    using System;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEngine.UIElements;
    
    public interface IUniExposedParameter
    {
        string DisplayName { get; set; }
        
        string Info { get; }

        void Apply(UniGraphAsset asset);
    }
    
    
    [Serializable]
    public class UniExposedParameter<TNode> : IUniExposedParameter
        where TNode : INode
    {
        public string parameter;

        public UniExposedParameter()
        {
            parameter = typeof(TNode).Name;
        }

        public string DisplayName
        {
            get => parameter;
            set => parameter = value;
        }
 
        public string Info => typeof(TNode).Name;
        
        public void Apply(UniGraphAsset asset)
        {
            var mousePos     = ContextClickEvent.GetPooled().mousePosition;
            asset.CreateNode(typeof(TNode), mousePos);
        }
    }
}