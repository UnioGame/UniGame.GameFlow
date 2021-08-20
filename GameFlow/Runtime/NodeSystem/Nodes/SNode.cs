namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes
{
    using System;
    using System.Collections.Generic;
    using Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;

    [Serializable]
    public class SNode : SerializableNode,
        IProxyNode
    {
        #region private fields

        private Action                         _onInitialize;
        private Action<List<ILifeTimeCommand>> _onCommandsInitialize;
        private Action                         _onExecute;

        [NonSerialized] private bool                   _isInitialized;
        private                 bool                   _isActive;
        private                 LifeTimeDefinition     _lifeTime = new LifeTimeDefinition();
        private                 List<ILifeTimeCommand> _commands;

        #endregion

        #region public properties

        /// <summary>
        /// Is node currently active
        /// </summary>
        public bool IsActive => _isActive;

        public ILifeTime LifeTime => _lifeTime.LifeTime;

        #endregion

        #region public methods

        public void Initialize(
            IGraphData graphData,
            Action initializeAction,
            Action<List<ILifeTimeCommand>> initializeCommands = null,
            Action executeAction = null) => InnerInitialize(graphData, initializeAction, initializeCommands, executeAction);

        public sealed override void Initialize(IGraphData graphData) => InnerInitialize(graphData);

        /// <summary>
        /// stop execution
        /// </summary>
        public void Exit()
        {
            _lifeTime.Terminate();
            _isActive = false;
        }

        /// <summary>
        /// start node execution
        /// </summary>
        public void Execute()
        {
            //node already active
            if (_isActive)
            {
                return;
            }

            //initialize
            Initialize(GraphData);
            //mark as active
            _isActive = true;
            //execute all node commands
            _commands.ForEach(x => x.Execute(LifeTime));
            //user defined logic
            OnExecute();
            //proxy outer execution
            _onExecute?.Invoke();
        }

        /// <summary>
        /// stop node execution
        /// </summary>
        public void Release()
        {
            Exit();
            _isInitialized = false;
            _isActive      = false;
            _commands.Clear();
        }

        #endregion

        /// <summary>
        /// Call once on node initialization
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// base logic realization
        /// </summary>
        protected virtual void OnExecute()
        {
        }

        /// <summary>
        /// update active list commands
        /// add all supported node commands here
        /// </summary>
        protected virtual void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
        }

        #region private methods

        private void InnerInitialize(
            IGraphData graphData,
            Action initializeAction = null,
            Action<List<ILifeTimeCommand>> initializeCommands = null,
            Action executeAction = null)
        {

            if (Application.isEditor && Application.isPlaying == false)
            {
                _lifeTime?.Release();
            }

            if (Application.isPlaying && _isInitialized)
                return;

            _onInitialize         = initializeAction;
            _onCommandsInitialize = initializeCommands;
            _onExecute            = executeAction;

            _lifeTime?.Release();

            base.Initialize(graphData);

            InitializeData(graphData);

            _isInitialized = Application.isPlaying;
            
            //initialize all node commands
            InitializeCommands();
            //initialize ports
            InitializePorts();
            //custom node initialization
            OnInitialize();
            //proxy initialization            
            _onInitialize?.Invoke();

            LifeTime.AddCleanUpAction(ResetData);
        }

        private void ResetData()
        {
            _isActive = false;
            _isInitialized = false;
        }

        /// <summary>
        /// Initialize all node commands
        /// create port and bind them
        /// </summary>
        private void InitializeCommands()
        {
            _commands.Clear();

            //register node commands
            UpdateCommands(_commands);

            //outer node commands
            _onCommandsInitialize?.Invoke(_commands);
        }

        /// <summary>
        /// initialize ports before execution
        /// </summary>
        private void InitializePorts()
        {
            //initialize ports
            foreach (var port in Ports)
            {
                port.Initialize(this);
                
                if (!Application.isPlaying) continue;
                
                LifeTime.AddCleanUpAction(port.Release);
                AddPortValue(port);
            }
        }

        private void InitializeData(IGraphData graphData)
        {
            GraphData = graphData;
            //restart lifetime
            _lifeTime = _lifeTime ?? new LifeTimeDefinition();
            _commands = _commands ?? new List<ILifeTimeCommand>();
        }

        #endregion

    }
}