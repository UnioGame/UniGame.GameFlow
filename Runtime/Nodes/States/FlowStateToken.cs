namespace UniModules.UniGame.GameFlow.GameFlow.Runtime.Nodes.States
{
    using System;
    using System.Collections.Generic;
    using Context.Runtime.Connections;
    using Core.Runtime.DataFlow.Interfaces;
    using UniGameFlow.Nodes.Runtime.States;
    using UnityEngine;

    [Serializable]
    public class FlowStateToken : IStateToken
    {
      
        private readonly IContextConnection       _context;
        private readonly List<IStateCancellation> _states;

        #region constructor
        
        public FlowStateToken()
        {
            _context  = new ContextConnection();
            _states   = new List<IStateCancellation>();
        }

        #endregion

        public IContextConnection Context => _context;

        public ILifeTime LifeTime => _context.LifeTime;

        public bool StopAfter(IStateCancellation node)
        {
            var index = _states.IndexOf(node);
            return StopAt(++index);
        }

        public bool TakeOwnership(IStateCancellation asyncState)
        {
            var stopResult = StopSince(asyncState);
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

        public bool StopSince(IStateCancellation asyncState)
        {
            return StopAt(_states.IndexOf(asyncState));
        }

        public bool StopAt(int index)
        {
            if (index < 0 || index >= _states.Count)
                return true;

            for (var i = index; i < _states.Count; i++)
            {
                var state = _states[i];
                state.StopState();
            }

            _states.RemoveRange(index,_states.Count - index);
            return true;
        }


        public sealed override bool Equals(object obj) => false;

    }
}