namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;

    public class GameServiceCommand : ILifeTimeCommand
    {
        public GameServiceCommand(Func<IGameService> service)
        {
        
        }
    
        public void Execute(ILifeTime lifeTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
