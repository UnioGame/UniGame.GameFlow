namespace UniGreenModules.UniNodeSystem.Runtime.Commands
{
    using System;
    using Interfaces;
    using UniCore.Runtime.Interfaces;

    
    [Serializable]
    public class SerializedNodeCommand
    {
        public bool isUpdatable = false;

        public bool IsUpdatable => isUpdatable;

        public virtual ILifeTimeCommand Create(IUniNode node) => null;

    }
}
