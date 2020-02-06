namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.DemoGame.Runtime.Models
{
    using System;
    using UniRx;

    [Serializable]
    public class DemoGameStatusData 
    {
    
        public BoolReactiveProperty IsReady = new BoolReactiveProperty();
    
        public ReactiveProperty<DemoGameState> State = new ReactiveProperty<DemoGameState>(DemoGameState.Initialize);
    
    }
}