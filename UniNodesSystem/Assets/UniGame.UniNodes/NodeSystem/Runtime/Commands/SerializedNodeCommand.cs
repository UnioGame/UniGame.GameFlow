namespace UniGame.UniNodes.NodeSystem.Runtime.Commands
{
    using System;
    using Core.Interfaces;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    [Serializable]
    public class SerializedNodeCommand : ILifeTimeCommandSource, IValidator
    {
        public virtual bool IsUpdatable { get; }

        public virtual ILifeTimeCommand Create(IUniNode node) => null;

        public virtual bool Validate() => false;

    }
}
