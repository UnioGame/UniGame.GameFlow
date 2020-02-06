using UniRx;
using UnityEngine;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.DemoGame.Runtime.Models.Ui
{
    using UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Abstract;
    using UniRx.Async;

    [CreateAssetMenu(
        fileName = nameof(MenuModelData),
        menuName = "UniGame/NodeSystem/Examples/DemoGame/MenuUiData")]
    public class MenuModelData : AssetValueSource
    {
    
        public BoolReactiveProperty isShown = new BoolReactiveProperty(false);
    
        public IntReactiveProperty counter = new IntReactiveProperty();
    
        public ReactiveCommand play = new ReactiveCommand();

        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            context.Publish(this);
            return context;
        }
    }
}
