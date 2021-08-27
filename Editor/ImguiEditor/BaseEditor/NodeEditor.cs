namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using Drawers;
    using Drawers.Interfaces;
    using Interfaces;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEngine;

    /// <summary> Base class to derive custom Node editors from. Use this to create your own custom inspectors and editors for your nodes. </summary>
    [CustomNodeEditor(typeof(INode))]
    public class NodeEditor :
        NodeEditorBase<NodeEditor, CustomNodeEditorAttribute, INode>,
        INodeEditorData
    {
        
        /// <summary> Fires every whenever a node was modified through the editor </summary>
        public static Action<INode> OnUpdateNode;

        /// <summary>
        /// nodes port positions
        /// </summary>
        public static Dictionary<INodePort, Vector2> PortPositions = new Dictionary<INodePort, Vector2>();

        public static int Renaming;

        protected List<INodeEditorHandler> _bodyDrawers = new List<INodeEditorHandler>();

        protected List<INodeEditorHandler> _headerDrawers = new List<INodeEditorHandler>();

        public INode Target => Node;

        public IReadOnlyDictionary<INodePort, Vector2> Ports => PortPositions;

        public EditorNode EditorNode => EditorData;
        
        public sealed override void OnEnable()
        {
            _bodyDrawers   = new List<INodeEditorHandler>();
            _headerDrawers = new List<INodeEditorHandler>();

            _bodyDrawers   = InitializedBodyDrawers();
            _headerDrawers = InitializeHeaderDrawers();

            OnEditorEnabled();
        }

        public bool IsSelected { get; set; }

        public virtual void OnHeaderGUI() => Draw(_headerDrawers);

        /// <summary> Draws standard field editors for all public fields </summary>
        public virtual void OnBodyGUI()
        {
            PortPositions = PortPositions ?? new Dictionary<INodePort, Vector2>();
            PortPositions.Clear();

            SerializedObject?.Update();

            Draw(_bodyDrawers);

            SerializedObject?.ApplyModifiedProperties();
        }

        public virtual int GetWidth()
        {
            var type = Node.GetType();

            return NodeEditorWindow.nodeWidth.TryGetValue(type, out var width) ? width : Node.Width;
        }

        public virtual Color GetTint()
        {
            var type = Node.GetType();
            return NodeEditorWindow.nodeTint.TryGetValue(type, out var color) ? color : Color.white;
        }

        public virtual GUIStyle GetBodyStyle()
        {
            return NodeEditorResources.styles.nodeBody;
        }

        public void InitiateRename()
        {
            Renaming = 1;
        }

        public void Rename(string newName)
        {
            Node.SetName(newName);
        }

        #region private methods

        protected virtual List<INodeEditorHandler> InitializeHeaderDrawers()
        {
            _headerDrawers.Add(new BaseHeaderDrawer());
            return _headerDrawers;
        }

        protected virtual List<INodeEditorHandler> InitializedBodyDrawers()
        {
            _bodyDrawers.Add(new BaseBodyDrawer());
            return _bodyDrawers;
        }

        protected virtual void OnEditorEnabled()
        {
        }

        protected void Draw(List<INodeEditorHandler> drawers)
        {
            for (var i = 0; i < drawers.Count; i++) {
                var drawer = drawers[i];
                if (!drawer.Update(this, Node))
                    break;
            }
        }

        #endregion
    }
}