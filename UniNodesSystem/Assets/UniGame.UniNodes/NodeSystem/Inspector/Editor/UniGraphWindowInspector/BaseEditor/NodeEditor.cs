namespace UniGreenModules.UniNodeSystem.Inspector.Editor.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using Drawers;
    using Drawers.Interfaces;
    using Interfaces;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UnityEditor;
    using UnityEngine;

    /// <summary> Base class to derive custom Node editors from. Use this to create your own custom inspectors and editors for your nodes. </summary>
    [CustomNodeEditor(typeof(Node))]
    public class NodeEditor : 
        NodeEditorBase<NodeEditor, CustomNodeEditorAttribute, Node>, 
        INodeEditorData
    {

        /// <summary>
        /// nodes port positions
        /// </summary>
        public static Dictionary<NodePort, Vector2> PortPositions = new Dictionary<NodePort, Vector2>();

        /// <summary> Fires every whenever a node was modified through the editor </summary>
        public static Action<Node> OnUpdateNode;
        public static int Renaming;
        
        protected List<INodeEditorHandler> _bodyDrawers = new List<INodeEditorHandler>();
        
        protected List<INodeEditorHandler> _headerDrawers = new List<INodeEditorHandler>();

        public INode Target => target;

        public IReadOnlyDictionary<NodePort, Vector2> HandledPorts => PortPositions;

        public SerializedObject SerializedObject => serializedObject;

        
        public sealed override void OnEnable()
        {
            _bodyDrawers = new List<INodeEditorHandler>();
            _headerDrawers = new List<INodeEditorHandler>();
            
            _bodyDrawers = InitializedBodyDrawers();
            _headerDrawers = InitializeHeaderDrawers();
            
            OnEditorEnabled();
            
        }

        public virtual bool IsSelected()
        {
            return Selection.Contains(target);
        }
        
        public virtual void OnHeaderGUI() => Draw(_headerDrawers);
        
        /// <summary> Draws standard field editors for all public fields </summary>
        public virtual void OnBodyGUI()
        {
            PortPositions = PortPositions ?? new Dictionary<NodePort, Vector2>();
            PortPositions.Clear();
            
            serializedObject.Update();

            Draw(_bodyDrawers);

            serializedObject.ApplyModifiedProperties();
        }

        public virtual int GetWidth()
        {
            var type = target.GetType();

            return NodeEditorWindow.nodeWidth.TryGetValue(type, out var width) ? 
                width : target.width;
            
        }

        public virtual Color GetTint()
        {
            var type = target.GetType();
            return NodeEditorWindow.nodeTint.TryGetValue(type, out var color) ? 
                color : Color.white;
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
            target.nodeName = newName;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
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

        protected virtual void OnEditorEnabled(){}

        protected void Draw(List<INodeEditorHandler> drawers)
        {
            for (var i = 0; i < drawers.Count; i++)
            {
                var drawer = drawers[i];
                if(!drawer.Update(this, target)) 
                    break;
            }
        }
        
        #endregion
        
    }
}