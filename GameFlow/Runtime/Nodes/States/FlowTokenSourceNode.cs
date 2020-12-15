namespace UniModules.UniGame.GameFlow.GameFlow.Runtime.Nodes.States
{
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.Nodes.Runtime.States;
    using UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;

    [CreateNodeMenu("States/FlowTokenSource",nodeName = "FlowTokenSource")]
    public class FlowTokenSourceNode : UniNode
    {
        #region inspector
        
        [Port(PortIO.Output)]
        public object tokenOutput;

        [Port(PortIO.Input)]
        public object input;

        public bool fireOnStart = true;

        public bool connectWithGrphContext = true;
        
        #endregion

        private IStateToken _token;
        
        private LifeTimeDefinition _tokenLifeTime;
        private LifeTimeDefinition TokenLifeTime => _tokenLifeTime = _tokenLifeTime ?? new LifeTimeDefinition();
        
        protected override void OnExecute()
        {
            var inputValue  = GetPortValue(nameof(input));

            inputValue.PortValueChanged
                .Where(x => inputValue.HasValue)
                .Do(x => FireToken())
                .Subscribe()
                .AddTo(LifeTime);

            if (fireOnStart)
            {
                FireToken();
            }

        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void FireToken()
        {
            var port        = GetPortValue(nameof(tokenOutput));
            port.Publish(CreateToken());
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void TerminateToken()
        {
            TokenLifeTime.Release();
        }

        private IStateToken CreateToken()
        {
            if (_token != null)
                return _token;
                        
            _token = new FlowStateToken()
                .AddTo(TokenLifeTime);

            if (connectWithGrphContext)
            {
                _token.Context
                    .Connect(this.Context)
                    .AddTo(_token.LifeTime);
            }
            
            LifeTime.AddDispose(_token);
            LifeTime.AddCleanUpAction(() => _token = null);

            return _token;
        }
        
    }
}
