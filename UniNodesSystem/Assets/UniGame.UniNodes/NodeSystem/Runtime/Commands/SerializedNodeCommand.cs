namespace UniGreenModules.UniNodeSystem.Runtime.Commands
{
    using System;
    using System.Diagnostics;
    using Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Commands;
    using UnityEngine;


    [Serializable]
    public class SerializedNodeCommand : ILifeTimeCommandSource, IValidator
    {
        public virtual bool IsUpdatable { get; }

        public virtual ILifeTimeCommand Create(IUniNode node) => null;

        public virtual bool Validate() => false;

    }
}
