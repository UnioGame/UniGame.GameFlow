namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Attributes;
    using Interfaces;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UnityEngine;

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

        [NonSerialized] 
        private bool isInitialized = false;

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

        public void Initialize(NodeGraph graphData)
        {
            if (Application.isEditor && Application.isPlaying == false) {
                lifeTimeDefinition.Terminate();
            }
            
            if (Application.isPlaying && isInitialized)
                return;

            isInitialized = true;
            
            graph = graphData;
            //restart lifetime
            lifeTimeDefinition = lifeTimeDefinition ?? new LifeTimeDefinition();
            lifeTime = lifeTimeDefinition.LifeTime;
            portValues = portValues ?? new HashSet<INodePort>();

            //initialize ports
            InitializePorts();
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
        private void InitializePorts()
        {
            //initialize ports
            for (var i = 0; i < Ports.Count; i++) {
                var nodePort = Ports[i];
                nodePort.Initialize(this);
                lifeTime.AddCleanUpAction(nodePort.Release);
                
                if(Application.isPlaying)
                    AddPortValue(nodePort);
            }
        }
        
        /// <summary>
        /// finish node life time
        /// </summary>
        private void OnDisable() => Exit();

#region inspector call

        public override void Validate()
        {
            var removedPorts = ClassPool.Spawn<List<NodePort>>();
            foreach (var portPair in ports) {
                var port = portPair.Value;
                if (port == null || string.IsNullOrEmpty(port.fieldName) || port.nodeId == 0) {
                    removedPorts.Add(port);
                    continue;
                }
                var value = PortValues.FirstOrDefault(x => x.ItemName == port.ItemName && 
                                        x.Direction == port.Direction);
                if (value == null || string.IsNullOrEmpty(value.ItemName)) {
                    removedPorts.Add(port);
                }
            }

            removedPorts.ForEach(RemovePort);
            removedPorts.DespawnCollection();

            for (int i = 0; i < Ports.Count; i++) {
                var port = Ports[i];
                port.nodeId = id;
                port.node = this;
                port.Validate();
            }
            
            CleanUpSerializableCommands();
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