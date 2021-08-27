using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Interfaces
{
    using UniGame.Core.Runtime.Interfaces;

    public interface IContextService
    {
        void Bind(IContext context, ILifeTime lifeTime);
    }
}