namespace UniModules.GameFlow.Runtime.Commands
{
    using System;
    using Core.Interfaces;
    using Interfaces;
    using global::UniGame.Core.Runtime;

    [Serializable]
    public class SerializedNodeCommand : ILifeTimeCommandSource, IValidator
    {
        public virtual bool IsUpdatable { get; protected set; } = false;

        public virtual ILifeTimeCommand Create(IUniNode node) => null;

        public virtual bool Validate() => false;

    }
}
