namespace UniGreenModules.UniNodeSystem.Runtime
{
    using System;
    using Interfaces;
    using UniCore.Runtime.Common;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Connections;
    using UniRx;

    [Serializable]
    public class UniPortValue : IPortValue, IPoolable
    {
        #region serialized data

        /// <summary>
        /// port value Name
        /// </summary>
        public string name = string.Empty;

        
        #endregion

        #region private property

        [NonSerialized] private TypeData context;

        [NonSerialized] private TypeDataBrodcaster broadcaster;

        [NonSerialized] private bool initialized = false;
        
        [NonSerialized] private ReactiveCommand portValueChanged = new ReactiveCommand();

#endregion

        #region public properties

        public string ItemName => name;

        public bool HasValue => context.HasValue;

        public IObservable<Unit> PortValueChanged => portValueChanged;

        #endregion

        #region constructor
        
        public UniPortValue()
        {
            Initialize();
        }

        #endregion
        
        public void Initialize(string portName, ILifeTime lifeTime)
        {
            lifeTime.AddCleanUpAction(Release);
            
            name = portName;
            
            Initialize();
        }

        public void Dispose()
        {
            Release();
        }
        
        #region type data container

        public bool Remove<TData>()
        {
            var result = context.Remove<TData>();
            if (result) {
                broadcaster.Remove<TData>();
                portValueChanged.Execute(Unit.Default);
            }

            return result;
        }

        public void Publish<TData>(TData value)
        {
            
            context.Publish(value);
            broadcaster.Publish(value);

            portValueChanged.Execute(Unit.Default);
            
        }

        public void CleanUp()
        {
            context.CleanUp();
            broadcaster.CleanUp();
        }

        public void RemoveAllConnections()
        {
            broadcaster.CleanUp();
        }

        public TData Get<TData>()
        {
            return context.Get<TData>();
        }

        public bool Contains<TData>()
        {
            return context.Contains<TData>();
        }

        #endregion

        #region connector

        public IConnector<IContextWriter> Connect(IContextWriter contextData)
        {
            broadcaster.Connect(contextData);
            return this;
        }

        public void Disconnect(IContextWriter contextData)
        {
            broadcaster.Disconnect(contextData);
        }

        public void Release()
        {
            CleanUp();
            RemoveAllConnections();
        }

        public IObservable<T> Receive<T>()
        {
            return context.Receive<T>();
        }
        
        #endregion

        private void Initialize()
        {
            if (initialized)
                return;
            
            context    = new TypeData();
            broadcaster = new TypeDataBrodcaster();
            //mark as initialized
            initialized = true;
        }


    }
}