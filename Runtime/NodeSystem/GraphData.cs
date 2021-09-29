namespace UniModules.GameFlow.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    [Serializable]
    public class GraphData : IGraphData, IUniqueIdProvider
    {
        private readonly string _name;
        private readonly int _id;
        private readonly IContext _context;
        private readonly INodeGraph _graph;
        
        private Dictionary<int,IGraphItem> _graphItems = new Dictionary<int, IGraphItem>(16);

        #region constructor

        public GraphData(string name,int id, IContext context)
        {
            _name = name;
            _id = id;
            _context = context;
        }
        
        #endregion

        public IContext GraphContext => _context;

        public string ItemName => _name;

        public int Id => _id;

        public int GetNextId() => throw new NotImplementedException();

        public int UpdateId(int oldId) => throw new NotImplementedException();

        public INode GetNode(int nodeId) => throw new NotImplementedException();

        public INodePort GetPort(int id) => null;

        public IGraphData RemoveNode(INode node) => throw new NotImplementedException();

        public IGraphData AddItem(IGraphItem item)
        {
            if (_graphItems.TryGetValue(item.Id, out var graphItem))
                return this;
            
            switch (item) {
                case INode node:
                    break;
                case INodePort port:
                    break;
            }

            return this;
        }

        public IGraphItem Get(int id) => throw new NotImplementedException();

        public void Connect(int fromPort, int toPort)
        {
            
        }

    }
}
