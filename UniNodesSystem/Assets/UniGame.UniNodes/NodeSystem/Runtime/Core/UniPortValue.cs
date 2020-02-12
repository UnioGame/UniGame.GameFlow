namespace UniGreenModules.UniNodeSystem.Runtime
{
    using System;
    using Interfaces;
    using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
    using UniCore.Runtime.Common;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Connections;
    using UniRx;

    [Serializable]
    public class UniPortValue : IPortValue
    {
        #region serialized data

        /// <summary>
        /// port value Name
        /// </summary>
        public string name = string.Empty;

        #endregion

        #region private property

        private TypeData context;

        private TypeDataBrodcaster broadcaster;

        private bool initialized = false;
        
        private ReactiveCommand portValueChanged = new ReactiveCommand();

        private ILifeTime lifeTime;
        
        private Func<Type, bool> valueFilters;
        
#endregion

        #region public properties

        public ILifeTime LifeTime => lifeTime;

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
        
        public void Initialize(string portName, ILifeTime lifeTimeScope, Func<Type,bool> valueFilter = null)
        {
            name = portName;

            this.lifeTime = lifeTimeScope;
            this.lifeTime.AddCleanUpAction(Release);

            valueFilters = valueFilter ?? DefaultFilter;
            
            Initialize();
        }

        public void Dispose()
        {
            Release();
        }
        
        public void Release()
        {
            context.Release();
            RemoveAllConnections();
        }

        #region type data container

        public bool Remove<TData>()
        {
            var result = context.Remove<TData>();
            if (result) {
                portValueChanged.Execute(Unit.Default);
            }

            return result;
        }

        public void Publish<TData>(TData value)
        {
            if (!valueFilters(typeof(TData))) {
                GameLog.Log($"PUBLISH: You try to Publish wrong type value {nameof(T)} into {ItemName}");
                return;
            }
            
            context.Publish(value);
            broadcaster.Publish(value);
            portValueChanged.Execute(Unit.Default);
            
        }

        public void RemoveAllConnections()
        {
            broadcaster.Release();
        }

        public TData Get<TData>()
        {
            return context.Get<TData>();
        }

        public bool Contains<TData>()
        {
            return context.Contains<TData>();
        }
        
        public IObservable<T> Receive<T>()
        {
            if (valueFilters == null) {
                return Observable.Empty<T>();
            }
            return valueFilters(typeof(T)) ? context.Receive<T>() : Observable.Empty<T>();
        }
        
        #endregion

        #region connector

        public IDisposable Bind(IMessagePublisher contextData)
        {
            return broadcaster.Bind(contextData);
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

        private bool DefaultFilter(Type type) => true;
        
    }
}