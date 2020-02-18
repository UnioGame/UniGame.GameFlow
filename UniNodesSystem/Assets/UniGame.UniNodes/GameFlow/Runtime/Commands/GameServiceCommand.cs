namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

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
