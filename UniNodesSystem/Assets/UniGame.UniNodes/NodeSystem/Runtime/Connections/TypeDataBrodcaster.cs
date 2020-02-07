
namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Connections
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UniNodeSystem.Runtime.Interfaces;

    public class TypeDataBrodcaster : 
        IPoolable, ITypeDataBrodcaster
    {
        private List<IContextWriter> _registeredItems = new List<IContextWriter>();
        private int count = 0;

        #region ipoolable
        
        public virtual void Release()
        {
            _registeredItems.Clear();
            UpdateCounter();
        }
        
        #endregion
        
        #region IContextData interface

        public void CleanUp()
        {
            for (var i = 0; i < count; i++)
            {
                var context = _registeredItems[i];
                context.CleanUp();
            }
        }
        
        public virtual bool Remove<TData>()
        {
            for (var i = 0; i < count; i++)
            {
                var context = _registeredItems[i];
                context.Remove<TData>();
            }
            return true;          
        }

        public void Publish<TData>(TData value)
        {
            for (var i = 0; i < count; i++)
            {
                var context = _registeredItems[i];
                context.Publish(value);
            }
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IConnector<IContextWriter> Connect(IContextWriter connection)
        {
            if (!_registeredItems.Contains(connection))
                _registeredItems.Add(connection);
            UpdateCounter();
            return this;
        }

        public void Disconnect(IContextWriter connection)
        {
            _registeredItems.Remove(connection);
            UpdateCounter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateCounter()
        {
            count = _registeredItems.Count;
        }
    }
    
}
