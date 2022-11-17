using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
using UniModules.UniGame.Core.Runtime.Interfaces;

namespace UniGame.GameFlow.Runtime.Interfaces
{
    using Cysharp.Threading.Tasks;

    public interface IContextService
    {
        UniTask Bind(IContext context, ILifeTime lifeTime);
    }
}