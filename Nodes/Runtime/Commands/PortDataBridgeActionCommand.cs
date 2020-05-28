namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    [Serializable]
    public class PortDataBridgeActionCommand<TData> : ILifeTimeCommand
    {
        private readonly IContext inputPort;
        private readonly bool distinctSame;

        public IObservable<TData> Source { get; protected set; }

        public PortDataBridgeActionCommand(IContext input,bool distinctSame = true)
        {
            this.inputPort = input;
            this.distinctSame = distinctSame;
            
            var valueObservable = inputPort.Receive<TData>();

            valueObservable = distinctSame ? 
                valueObservable.DistinctUntilChanged() : 
                valueObservable;
            
            Source = valueObservable;
        }


        public void Execute(ILifeTime lifeTime) { }
    }
}
