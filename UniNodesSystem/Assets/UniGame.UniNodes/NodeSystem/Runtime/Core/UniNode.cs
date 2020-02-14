namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Attributes;
    using Core.Commands;
    using Interfaces;
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
    using Object = UnityEngine.Object;

    [HideNode]
    [Serializable]
    public abstract class UniNode : Node, IUniNode
    {       
        #region inspector fields

        [HideNodeInspector]
        [SerializeReference]
        public List<ILifeTimeCommandSource> serializableCommands = new List<ILifeTimeCommandSource>();

        #endregion
        
        #region private fields

        protected HashSet<INodePort> portValues = new HashSet<INodePort>();
        
        protected List<ILifeTimeCommand> commands = 
            new List<ILifeTimeCommand>();

        private LifeTimeDefinition lifeTimeDefinition = new LifeTimeDefinition();

        private bool isActive = false;

        protected ILifeTime lifeTime;

        #endregion

        #region public properties

        /// <summary>
        /// Is node currently active
        /// </summary>
        public bool IsActive => isActive;

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        public IReadOnlyCollection<INodePort> PortValues => portValues;

        #endregion

        #region public methods

        public void Initialize()
        {
            //check initialization status
            if (lifeTime != null && lifeTime.IsTerminated == false)
                return;

            GameLog.Log($"NODE {ItemName} : Initialize");

            //restart lifetime
            lifeTimeDefinition = lifeTimeDefinition ?? new LifeTimeDefinition();
            lifeTime = lifeTimeDefinition.LifeTime;
            portValues = portValues ?? new HashSet<INodePort>();
            
            //initialize ports
            InitializePorts(lifeTime);
            //custom node initialization
            OnInitialize();
            //initialize all node commands
            InitializeCommands();

            lifeTime.AddCleanUpAction(() => isActive = false);
            lifeTime.AddCleanUpAction(() => portValues.Clear());
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
            GameLog.Log($"NODE {ItemName} : Exit");
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
            GameLog.Log($"NODE {ItemName} : Execute");
            //mark as active
            isActive = true;
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
            for (var i = 0; i < serializableCommands.Count; i++) {
                var command = serializableCommands[i];
                if(command!=null) 
                    commands.Add(command.Create(this));
            }
            
            //register node commands
            UpdateCommands(commands);
        }

        /// <summary>
        /// initialize ports before execution
        /// </summary>
        /// <param name="portsLifeTime"></param>
        private void InitializePorts(ILifeTime portsLifeTime)
        {
            //initialize ports
            for (var i = 0; i < Ports.Count; i++) {
                var nodePort = Ports[i];
                nodePort.Initialize();
                portsLifeTime.AddCleanUpAction(nodePort.Release);
                
                if(Application.isPlaying)
                    AddPortValue(nodePort);
            }
        }
        
        /// <summary>
        /// finish node life time
        /// </summary>
        private void OnDisable() => Exit();

        
#region inspector call

        [Conditional("UNITY_EDITOR")]
        public void Validate()
        { 
            foreach (var port in Ports) {
                port.Validate();
            }
            CleanUpSerializableCommands();
        }

        [Conditional("UNITY_EDITOR")]
        private void LogMessage(string message)
        {
            GameLog.Log($"{Graph.name}:{name}: {message}");
        }

        [Conditional("UNITY_EDITOR")]
        protected virtual void OnValidate()
        {
            Validate();
        }
        
        [Conditional("UNITY_EDITOR")]
        public void CleanUpSerializableCommands()
        {
            //remove all temp commands
            serializableCommands.RemoveAll(x => x == null || x.Validate() == false);

        }
        
#endregion

    }
}