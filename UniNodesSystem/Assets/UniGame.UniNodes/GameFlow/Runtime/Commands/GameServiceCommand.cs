using System;
using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
using UniGreenModules.UniCore.Runtime.Interfaces;
using UniGreenModules.UniGameSystem.Runtime.Interfaces;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.GameFlow.Runtime.Commands
{
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
