namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Context.Runtime.Context;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.SerializableType;
    using UniRx;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    [Serializable]
    public class PortValue : IPortValue, ISerializationCallbackReceiver
    {
        #region static data
        
        private static List<Type> ignoreFilterTypes = new List<Type>(){typeof(object)};
        
        #endregion
        
        #region serialized data

        /// <summary>
        /// port value Name
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// allowed port value types
        /// </summary>
        [SerializeField]
        protected List<SType> serializedValueTypes = new List<SType>();

        [SerializeField] 
        [ReadOnlyValue] protected int broadcastersCount;

        #endregion

        #region private property

        private EntityContext _data;

        private ReactiveCommand _portValueChanged = new ReactiveCommand();

        private ILifeTime _lifeTime;

        private LifeTimeDefinition _lifeTimeDefeDefinition = new LifeTimeDefinition();

        private List<Type> _valueTypeFilter;

        #endregion

        #region constructor

        public PortValue()
        {
            Initialize();
        }

        #endregion

        #region public properties

        public IReadOnlyList<Type> ValueTypes => _valueTypeFilter = _valueTypeFilter ?? new List<Type>();

        public ILifeTime LifeTime => _lifeTimeDefeDefinition.LifeTime;

        public string ItemName => name;

        public int RuntimeId => _data.Id;

        public bool HasValue => _data.HasValue;

        public IObservable<Unit> PortValueChanged => _portValueChanged;

        public bool IsValidPortValueType(Type type)
        {
            if (_valueTypeFilter == null || _valueTypeFilter.Count == 0)
                return true;
            return _valueTypeFilter.Contains(type);
        }

        #endregion
        
        #region connection api

        public int ConnectionsCount => _data.ConnectionsCount;

        public void Disconnect(IMessagePublisher connection) {
            _data.Disconnect(connection);
        }

        public IDisposable Bind(IMessagePublisher contextData)
        {
            var disposable = _data.Bind(contextData);
            broadcastersCount = _data.ConnectionsCount;
            return disposable;
        }

        #endregion
        
        public void Initialize(string portName)
        {
            name = portName;
            Initialize();
        }


        public void SetValueTypeFilter(IReadOnlyList<Type> types)
        {
            _valueTypeFilter = _valueTypeFilter ?? new List<Type>();
            _valueTypeFilter.Clear();
            _valueTypeFilter.AddRange(types);

            UpdateSerializedFilter(_valueTypeFilter);
        }

        public void Dispose() => Release();

        public void Release() => _lifeTimeDefeDefinition.Terminate();

        #region type data container

        public bool Remove<TData>()
        {
            var result = _data.Remove<TData>();
            if (result) {
                _portValueChanged.Execute(Unit.Default);
            }

            return result;
        }

        public void Publish<TData>(TData value)
        {
            if (_valueTypeFilter != null &&
                _valueTypeFilter.Count != 0 &&
                !_valueTypeFilter.Contains(typeof(TData))) {
                return;
            }

            _data.Publish(value);
            _portValueChanged.Execute(Unit.Default);
        }

        public void RemoveAllConnections()
        {
            _data.Release();
        }

        public TData Get<TData>() => _data.Get<TData>();

        public bool Contains<TData>() => _data.Contains<TData>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObservable<TValue> Receive<TValue>() => _data.Receive<TValue>();

        #endregion

        private bool DefaultFilter(Type type) => true;

        private void Initialize()
        {
            _lifeTimeDefeDefinition = _lifeTimeDefeDefinition ?? new LifeTimeDefinition();
            _lifeTime               = _lifeTimeDefeDefinition.LifeTime;
            _lifeTimeDefeDefinition.Release();

            _data        = _data ?? new EntityContext();

            _lifeTime.AddCleanUpAction(_data.Release);
            _lifeTime.AddCleanUpAction(RemoveAllConnections);
        }

        #region serialization rules

        public void OnBeforeSerialize()
        {
            UpdateSerializedFilter(_valueTypeFilter);
        }

        public void OnAfterDeserialize()
        {
            _valueTypeFilter = _valueTypeFilter ?? new List<Type>();
            _valueTypeFilter.Clear();

            for (var i = 0; i < serializedValueTypes.Count; i++) {
                var typeFilter = serializedValueTypes[i];
                if (typeFilter != null)
                    _valueTypeFilter.Add(typeFilter);
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void UpdateSerializedFilter(IReadOnlyList<Type> filter)
        {
            serializedValueTypes.Clear();
            if (filter == null) return;

            foreach (var type in filter)
            {
                if(ignoreFilterTypes.Contains(type))
                    continue;
                serializedValueTypes.Add(type);
            }

        }

        #endregion


        #region Unity Editor Api

#if UNITY_EDITOR

        public IReadOnlyDictionary<Type, IValueContainerStatus> Values => _data.EditorValues;

#endif

        #endregion

    }
}