﻿using UniGreenModules.UniGame.SerializableContext.Runtime.Abstract;

namespace UniGreenModules.UniGameSystems.Examples.SimpleSystem.Context
{
    using Runtime.Context;
    using UnityEngine;
    
    [CreateAssetMenu(menuName = "UniGame/GameSystem/Examples/DemoGameContext")]
    public class DemoGameStatusData : TypeValueSource<DemoGameData,IDemoGameContext>{}
    
}
