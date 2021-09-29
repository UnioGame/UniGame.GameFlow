namespace UniModules.GameFlow.Runtime.Commands
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public class DummyPortsCommand : ILifeTimeCommand
    {
        public UniTask Execute(ILifeTime lifeTime)
        {
            return UniTask.CompletedTask;
        }
    }
}