using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UniGame.UniNodes.NodeSystem.Runtime.Attributes;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces;
using UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
using UniGreenModules.UniCore.Runtime.DataFlow;
using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
using UniGreenModules.UniCore.Runtime.Interfaces;
using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
using UniGreenModules.UniCore.Runtime.ProfilerTools;
using UnityEngine;

namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes
{
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.Core.Runtime.Attributes.FieldTypeDrawer;

    [Serializable]
    public class SNode : SerializableNode, IProxyNode
    {
        #region private fields
        
        private Action                         onInitialize;
        private Action<List<ILifeTimeCommand>> onCommandsInitialize;
        private Action                         onExecute;

        [NonSerialized] private bool isInitialized = false;

        private LifeTimeDefinition lifeTimeDefinition = new LifeTimeDefinition();

        private bool isActive = false;

        private ILifeTime lifeTime;

        private List<ILifeTimeCommand> commands;

        #endregion


        #region constructor

        public SNode(){}

        public SNode(
            int id,
            string name,
            NodePortDictionary ports) : base(id, name, ports){}

        #endregion

        #region public properties

        /// <summary>
        /// Is node currently active
        /// </summary>
        public bool IsActive => isActive;

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        #endregion

        #region public methods

        public void Bind(
            Action initializeAction = null,
            Action<List<ILifeTimeCommand>> initializeCommands = null,
            Action executeAction = null)
        {
            this.onInitialize         = initializeAction;
            this.onCommandsInitialize = initializeCommands;
            this.onExecute            = executeAction;
        }
        
        public sealed override void Initialize(IGraphData graphData)
        {
            base.Initialize(graphData);
            
            InitializeData(graphData);
            
            if (Application.isEditor && Application.isPlaying == false) {
                lifeTimeDefinition.Terminate();
            }

            if (Application.isPlaying && isInitialized)
                return;

            isInitialized = true;

            //initialize ports
            InitializePorts();
            //initialize all node commands
            InitializeCommands();
            //custom node initialization
            OnInitialize();
            //proxy outer initialization
            onInitialize?.Invoke();
            
            lifeTime.AddCleanUpAction(() => {
                isActive = false;
                this.onInitialize         = null;
                this.onCommandsInitialize = null;
                this.onExecute            = null;
            });
        }

        /// <summary>
        /// stop execution
        /// </summary>
        public void Exit() => lifeTimeDefinition.Terminate();

        /// <summary>
        /// start node execution
        /// </summary>
        public void Execute()
        {
            //node already active
            if (isActive) {
                return;
            }

            //initialize
            Initialize(graph);
            //mark as active
            isActive = true;
            //execute all node commands
            commands.ForEach(x => x.Execute(LifeTime));
            //user defined logic
            OnExecute();
            //proxy outer execution
            onExecute?.Invoke();
        }

        /// <summary>
        /// stop node execution
        /// </summary>
        public void Release() => Exit();
        
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
        
        /// <summary>
        /// Initialize all node commands
        /// create port and bind them
        /// </summary>
        private void InitializeCommands()
        {
            commands.Clear();

            //register node commands
            UpdateCommands(commands);
            
            //outer node commands
            onCommandsInitialize?.Invoke(commands);
        }

        /// <summary>
        /// initialize ports before execution
        /// </summary>
        private void InitializePorts()
        {
            //initialize ports
            foreach (var port in Ports) {
                port.Initialize(this);
                if (Application.isPlaying) {
                    lifeTime.AddCleanUpAction(port.Release);
                    AddPortValue(port);
                }
            }
        }

        private void InitializeData(IGraphData graphData)
        {
            graph = graphData;
            //restart lifetime
            lifeTimeDefinition = lifeTimeDefinition ?? new LifeTimeDefinition();
            lifeTime           = lifeTimeDefinition.LifeTime;
            commands           = commands ?? new List<ILifeTimeCommand>();
        }
        
        #endregion

    }
}