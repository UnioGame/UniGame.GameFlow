namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime
{
    using System.Collections;
    using GameFlow.Runtime;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniRoutine.Runtime;
    using UniModules.UniRoutine.Runtime.Extension;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public class SimpleSystem3 : GameService
    {

        private IEnumerator ReadyDelay(float delay)
        {
            yield return this.WaitForSeconds(delay);
            isReady.Value = true;
        }
        
    }
}
