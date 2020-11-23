namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.Context.Runtime.Context;
    using UniGame.Context.SerializableContext.Runtime.Abstract;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;

    [Serializable]
    public class SingleAsyncStateToken : IAsyncStateToken
    {
        private EntityContext      _context;
        private IAsyncContextState _state;

        #region constructor

        public SingleAsyncStateToken()
        {
            _context  = new EntityContext();
        }

        #endregion

        public IContext Context => _context;

        public int Id => _context.Id;
        
        public ILifeTime LifeTime => _context.LifeTime;

        public async UniTask<bool> TakeOwnership(IAsyncContextState state)
        {
            await StopCurrent();
            
            _state = state;
            _state.ExecuteAsync(_context);
            return true;
        }

        public async UniTask<bool> StopAfter(IAsyncContextState node)
        {
            return await StopCurrent();
        }

        public async UniTask<bool> StopSince(IAsyncContextState node)
        {
            return await StopCurrent();
        }

        public void Dispose()
        {
            _context.Release();
            _state.ExitAsync();
            _state = null;
        }

        /// <summary>
        /// always unequals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => false;

        private async UniTask<bool> StopCurrent()
        {
            if (_state == null) return false;
            await _state.ExitAsync();
            _state = null;
            return true;
        }
    }
}