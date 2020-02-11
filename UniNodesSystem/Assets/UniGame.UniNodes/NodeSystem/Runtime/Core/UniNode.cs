namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Attributes;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Extension;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniNodeSystem.Runtime.Commands;
    using UniNodeSystem.Runtime.Core;
    using UniNodeSystem.Runtime.Extensions;
    using UniNodeSystem.Runtime.Interfaces;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public abstract class UniNode : UniBaseNode, IUniNode
    {       
        #region inspector fields

        [SerializeField]
        public List<SerializedNodeCommand> nodeSavedCommands = new List<SerializedNodeCommand>();

        #endregion
        
        #region private fields

        [NonSerialized] protected HashSet<INodePort> portValues = 
            new HashSet<INodePort>();
        
        [NonSerialized] protected List<ILifeTimeCommand> commands = 
            new List<ILifeTimeCommand>();

        [NonSerialized] private LifeTimeDefinition lifeTimeDefinition = 
            new LifeTimeDefinition();

        [NonSerialized] private bool isInitialized;

        [NonSerialized] private bool isActive = false;

        #endregion

        #region public properties

        /// <summary>
        /// Is node currently active
        /// </summary>
        public bool IsActive => isActive;

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        public IReadOnlyCollection<INodePort> PortValues => portValues = (portValues ?? new HashSet<INodePort>());

        #endregion

        #region public methods

        public void Initialize()
        {
            //check initialization status
            if (isInitialized)
                return;

            portValues = new HashSet<INodePort>();
            
            //initialize ports
            foreach (var nodePort in Ports) {
                nodePort.Initialize();
            }
            
            isInitialized = true;
            
            //custom node initialization
            OnInitialize();
            //initialize all node commands
            InitializeCommands();
        }

        public bool AddPortValue(INodePort portValue)
        {
            if (portValue == null) {
                GameLog.LogErrorFormat("Try add NULL port value to {0}", this);
                return false;
            }

            portValues.Add(portValue);

            return true;
        }
        
        /// <summary>
        /// stop execution
        /// </summary>
        public void Exit()
        {
            isActive = false;
            lifeTimeDefinition.Terminate();
        }

        /// <summary>
        /// start node execution
        /// </summary>
        public void Execute()
        {
            //node already active
            if (isActive) {
                return;
            }

            //mark as active
            isActive = true;
            //restart lifetime
            lifeTimeDefinition.Release();
            //initialize
            Initialize();
            //execute all node commands
            commands.ForEach(x => x.Execute(LifeTime));
            //user defined logic
            OnExecute();
        }

        /// <summary>
        /// stop node execution
        /// </summary>
        public void Release() => Exit();

        #endregion

        /// <summary>
        /// Call once on node initialization
        /// </summary>
        protected virtual void OnInitialize(){}

        /// <summary>
        /// base logic realization
        /// </summary>
        protected virtual void OnExecute(){}

        /// <summary>
        /// update active list commands
        /// add all supported node commands here
        /// </summary>
        protected virtual void UpdateCommands(List<ILifeTimeCommand> nodeCommands){}
        
        private void InitializeCommands()
        {
            commands.Clear();
            
            //register all backed commands to main list
            for (var i = 0; i < nodeSavedCommands.Count; i++) {
                var command = nodeSavedCommands[i];
                if(command!=null) 
                    commands.Add(command.Create(this));
            }
            
            //register node commands
            UpdateCommands(commands);
        }

        /// <summary>
        /// finish node life time
        /// </summary>
        private void OnDisable() => Exit();

#region inspector call
        
        [Conditional("UNITY_EDITOR")]
        private void LogMessage(string message)
        {
            GameLog.Log($"{Graph.name}:{name}: {message}");
        }

        [Conditional("UNITY_EDITOR")]
        protected virtual void OnValidate()
        {
            Initialize();
            foreach (var port in Ports) {
                port.Validate();
            }
        }
        
#endregion

    }
}