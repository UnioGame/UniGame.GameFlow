using UniGame.Core.Runtime;

namespace UniGame.GameFlow.Runtime.Interfaces
{
    using Cysharp.Threading.Tasks;

    public interface IContextService
    {
        UniTask Bind(IContext context, ILifeTime lifeTime);
    }
}