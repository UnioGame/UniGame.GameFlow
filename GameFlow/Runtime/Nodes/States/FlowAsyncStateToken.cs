namespace UniModules.UniGame.GameFlow.GameFlow.Runtime.Nodes.States
{
    using System;
    using System.Collections.Generic;
    using Context.Runtime.Connections;
    using Context.SerializableContext.Runtime.Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniGameFlow.Nodes.Runtime.States;
    using UnityEngine;

    [Serializable]
    public class FlowAsyncStateToken : IAsyncStateToken
    {
      
        private readonly IContextConnection       _context;
        private readonly List<IAsyncContextState> _states;

        #region constructor
        
        public FlowAsyncStateToken()
        {
            _context  = new ContextConnection();
            _states   = new List<IAsyncContextState>();
        }

        #endregion

        public IContextConnection Context => _context;

        public ILifeTime LifeTime => _context.LifeTime;

        public async UniTask<bool> StopAfter(IAsyncContextState node)
        {
            var index = _states.IndexOf(node);
            await StopAt(++index);
            return true;
        }

        public async UniTask<bool> TakeOwnership(IAsyncContextState asyncState)
        {
            var stopResult = await StopSince(asyncState);
            if (!stopResult)
            {
                Debug.LogError($"CAN't STOP State's for token {GetType().Name}");
                return false;
            }
            _states.Add(asyncState);
            return true;
        }

        public void Dispose()
        {
            StopAt(0);
            
            _context.Dispose();
            _states.Clear();
        }

        public async UniTask<bool> StopSince(IAsyncContextState asyncState)
        {
            return await StopAt(_states.IndexOf(asyncState));
        }

        public async UniTask<bool> StopAt(int index)
        {
            if (index < 0 || index >= _states.Count)
                return true;

            for (var i = index; i < _states.Count; i++)
            {
                var state = _states[i];
                await state.ExitAsync();
            }

            _states.RemoveRange(index,_states.Count - index);
            return true;
        }


        public sealed override bool Equals(object obj) => false;

    }
}