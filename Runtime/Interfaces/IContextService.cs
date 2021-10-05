using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Interfaces
{
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime.Interfaces;

    public interface IContextService
    {
        UniTask Bind(IContext context, ILifeTime lifeTime);
    }
}