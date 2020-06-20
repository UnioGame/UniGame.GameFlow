namespace UniGame.UniNodes.GameFlow.Runtime.Interfaces
{
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;

    public interface IBinder
    {

        void Bind(ILifeTime lifeTime);

    }
}
