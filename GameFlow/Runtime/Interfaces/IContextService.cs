using UniModules.UniCore.Runtime.Interfaces;
using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Interfaces
{
    public interface IContextService
    {
        void Bind(IContext context, ILifeTime lifeTime);
    }
}