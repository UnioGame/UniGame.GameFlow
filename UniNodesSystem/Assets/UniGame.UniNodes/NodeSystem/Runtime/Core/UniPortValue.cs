namespace UniGreenModules.UniNodeSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Interfaces;
    using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
    using UniCore.Runtime.Common;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Connections;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class UniPortValue : IPortValue , ISerializationCallbackReceiver
    {
        #region serialized data

        /// <summary>
        /// port value Name
        /// </summary>
        public string name = string.Empty;

                
        /// <summary>
        /// allowed port value types
        /// </summary>
        [SerializeField] protected List<string> serializedValueTypes;

        
        #endregion

        #region private property

        private TypeData context;

        private TypeDataBrodcaster broadcaster;

        private bool initialized = false;
        
        private ReactiveCommand portValueChanged = new ReactiveCommand();

        private ILifeTime lifeTime;
        
        private List<Type> valueTypeFilter;

        
#endregion

        #region public properties
        
        public IReadOnlyList<Type> ValueTypes => valueTypeFilter = valueTypeFilter ?? new List<Type>();
       
        public ILifeTime LifeTime => lifeTime;

        public string ItemName => name;

        public bool HasValue => context.HasValue;

        public IObservable<Unit> PortValueChanged => portValueChanged;

        public bool IsValidPortValueType(Type type)
        {
            if (valueTypeFilter == null || valueTypeFilter.Count == 0)
                return true;
            return valueTypeFilter.Contains(type);
        }

        #endregion

        #region constructor
        
        public UniPortValue()
        {
            Initialize();
        }

        #endregion
        
        public void Initialize(string portName, ILifeTime lifeTimeScope)
        {
            name = portName;

            this.lifeTime = lifeTimeScope;
            this.lifeTime.AddCleanUpAction(Release);

            Initialize();
        }

        public void SetValueTypeFilter(IReadOnlyList<Type> types)
        {
            valueTypeFilter = valueTypeFilter ?? new List<Type>();
            valueTypeFilter.Clear();
            valueTypeFilter.AddRange(types);
            
            UpdateSerializedFilter(valueTypeFilter);
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
            if (valueTypeFilter != null && 
                valueTypeFilter.Count != 0  && 
                !valueTypeFilter.Contains(typeof(TData))) {
                GameLog.Log($"PUBLISH: You try to Publish wrong type value {nameof(T)} into {ItemName}");
                return;
            }
            
            context.Publish(value);
            broadcaster.Publish(value);
            portValueChanged.Execute(Unit.Default);
            
        }

        public void RemoveAllConnections() => broadcaster.Release();

        public TData Get<TData>() => context.Get<TData>();

        public bool Contains<TData>() => context.Contains<TData>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObservable<TValue> Receive<TValue>() => context.Receive<TValue>();

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

        #region serialization rules
        
        public void OnBeforeSerialize()
        {
            UpdateSerializedFilter(valueTypeFilter);
        }

        public void OnAfterDeserialize()
        {
            valueTypeFilter = valueTypeFilter ?? new List<Type>();
            valueTypeFilter.Clear();

            for (var i = 0; i < serializedValueTypes.Count; i++) {
                var typeFilter = serializedValueTypes[i];
                var type       = Type.GetType(typeFilter, false, true);
                if (type != null)
                    valueTypeFilter.Add(type);
            };
        }
        
        [Conditional("UNITY_EDITOR")]
        private void UpdateSerializedFilter(IReadOnlyList<Type> filter)
        {
            serializedValueTypes = filter == null ? new List<string>() : 
                filter.Select(x => x.AssemblyQualifiedName).ToList();
        }

        #endregion
    }
}