using System;
using UniModules.GameFlow.Runtime.Interfaces;
using UnityEngine.UIElements;

namespace UniGame.GameFlowEditor.Runtime
{
    [Serializable]
    public class UniExposedParameter<TNode> : IUniExposedParameter
        where TNode : INode
    {
        public string parameter;

        public UniExposedParameter()
        {
            parameter = UniExposedParametersTool.GetNiceNameFromType(typeof(TNode).Name);
        }

        public string DisplayName
        {
            get => parameter;
            set => parameter = value;
        }
 
        public string Info => UniExposedParametersTool.GetNiceNameFromType(typeof(TNode).Name);
        
        public void Apply(UniGraphAsset asset)
        {
            var mousePos     = ContextClickEvent.GetPooled().mousePosition;
            asset.CreateNode(typeof(TNode), mousePos);
        }
    }
}