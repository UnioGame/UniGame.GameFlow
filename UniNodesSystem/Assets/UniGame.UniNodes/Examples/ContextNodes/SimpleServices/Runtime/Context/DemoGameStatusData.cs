namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime.Context
{
    using UniGreenModules.UniGame.SerializableContext.Runtime.Abstract;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Examples/DemoGameContext")]
    public class DemoGameStatusData : TypeValueSource<DemoGameData,IDemoGameContext>{}
    
}
