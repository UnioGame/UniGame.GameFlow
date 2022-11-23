namespace UniModules.GameFlow.Runtime.Commands
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using global::UniGame.Core.Runtime;

    public class DummyPortsCommand : ILifeTimeCommand
    {
        public UniTask Execute(ILifeTime lifeTime)
        {
            return UniTask.CompletedTask;
        }
    }
}