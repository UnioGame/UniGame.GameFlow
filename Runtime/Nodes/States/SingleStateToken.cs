namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.Context.Runtime.Connections;
    using UniGame.Context.Runtime.Context;
    using UniGame.Context.SerializableContext.Runtime.Abstract;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;

    [Serializable]
    public class SingleStateToken : IStateToken
    {
        private ContextConnection  _context;
        private IStateCancellation _state;

        #region constructor

        public SingleStateToken()
        {
            _context  = new ContextConnection();
        }

        #endregion

        public IContextConnection Context => _context;

        public ILifeTime LifeTime => _context.LifeTime;

        public bool TakeOwnership(IStateCancellation state)
        {
            StopCurrent();
            
            _state = state;
            return true;
        }

        public bool StopAfter(IStateCancellation node)
        {
            return StopCurrent();
        }

        public bool StopSince(IStateCancellation node)
        {
            return StopCurrent();
        }

        public void Dispose()
        {
            _context.Release();
            _state.StopState();
            _state = null;
        }

        /// <summary>
        /// always unequals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => false;

        private bool StopCurrent()
        {
            if (_state == null) return false;
            _state.StopState();
            _state = null;
            return true;
        }
    }
}