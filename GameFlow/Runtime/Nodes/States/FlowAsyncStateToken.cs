namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.DataFlow;
    using UniGame.Context.Runtime.Context;
    using UniGame.Context.SerializableContext.Runtime.Abstract;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;

    [Serializable]
    public class FlowAsyncStateToken : IAsyncStateToken
    {
        private EntityContext            _context;
        private LifeTimeDefinition       _lifeTime;
        private List<IAsyncContextState> _states;

        #region constructor

        public FlowAsyncStateToken()
        {
            _context  = new EntityContext();
            _lifeTime = new LifeTimeDefinition();
            _states   = new List<IAsyncContextState>();
        }

        #endregion

        public IContext Context => _context;

        public int Id => _context.Id;

        public ILifeTime LifeTime => _lifeTime;

        public async UniTask<bool> StopAfter(IAsyncContextState node)
        {
            var index = _states.IndexOf(node);
            await StopAt(++index);
            return true;
        }

        public async UniTask<bool> TakeOwnership(IAsyncContextState asyncState)
        {
            var stopResult = await StopSince(asyncState);
            _states.Add(asyncState);
            asyncState.ExecuteAsync(_context);
            return true;
        }

        public void Dispose()
        {
            StopAt(0);
            _context.Release();
            _lifeTime.Release();
        }

        public async UniTask<bool> StopSince(IAsyncContextState asyncState)
        {
            return await StopAt(_states.IndexOf(asyncState));
        }
        
        public async UniTask<bool> StopAt(int index)
        {
            if (index < 0 || index >= _states.Count)
                return false;
            
            for (var i = index; i < _states.Count; i++)
            {
                var state = _states[i];
                await state.ExitAsync();
            }
    
            _states.RemoveAt(index);
            return true;
        }
    }
}