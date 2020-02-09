
namespace UniGreenModules.UniUiNodes.Runtime.UiData
{
    using System;
    using Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Connections;
    using UniNodeSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Ui module container
    /// </summary>
    public class UiModuleSlot : 
        UIBehaviour, 
        IUiModuleSlot,
        IDisposable
    {
        private TypeDataBrodcaster value = new TypeDataBrodcaster();

        public string SlotName => name;

        public IConnector<IMessagePublisher> Value => value;
     
        public void Dispose()
        {
            value.Release();
        }

        public virtual void Insert(RectTransform targetTransfom)
        {
            targetTransfom.SetParent(transform,false);
        }
    }
    
}
