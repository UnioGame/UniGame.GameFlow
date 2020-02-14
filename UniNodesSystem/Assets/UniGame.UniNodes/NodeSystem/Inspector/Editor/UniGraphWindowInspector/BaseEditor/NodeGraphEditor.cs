﻿namespace UniGreenModules.UniNodeSystem.Inspector.Editor.BaseEditor {
    using System;
    using Runtime.Core;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary> Base class to derive custom Node Graph editors from. Use this to override how graphs are drawn in the editor. </summary>
    [CustomNodeGraphEditor(typeof(NodeGraph))]
    public partial class NodeGraphEditor : NodeEditorBase<NodeGraphEditor, NodeGraphEditor.CustomNodeGraphEditorAttribute, NodeGraph> {
        /// <summary> The position of the window in screen space. </summary>
        public Rect position;
        /// <summary> Are we currently renaming a node? </summary>
        protected bool isRenaming;

        public virtual void OnGUI() { }

        public virtual Texture2D GetGridTexture() 
        {
            return this.GetSettings().gridTexture;
        }

        public virtual Texture2D GetSecondaryGridTexture() {
            return this.GetSettings().crossTexture;
        }

        /// <summary> Return default settings for this graph type. This is the settings the user will load if no previous settings have been saved. </summary>
        public virtual NodeEditorSettings GetDefaultPreferences() 
        {
            return new NodeEditorSettings();
        }

        /// <summary> Returns context node menu path. Null or empty strings for hidden nodes. </summary>
        public virtual string GetNodeMenuName(Type type) {
            //Check if type has the CreateNodeMenuAttribute
            CreateNodeMenuAttribute attrib;
            return NodeEditorUtilities.GetAttrib(type, out attrib) ? 
                attrib.menuName : 
                ObjectNames.NicifyVariableName(type.ToString().Replace('.', '/'));
        }

        /// <summary> Creates a copy of the original node in the graph </summary>
        public Node CopyNode(Node original) 
        {
            
            var node = target.CopyNode(original);
            node.name = original.name;
            node.transform.parent = original.Graph.transform;
            
            if (this.GetSettings().autoSave)
            {
                AssetDatabase.SaveAssets();
            }
            
            return node;
        }

        /// <summary> Safely remove a node and all its connections. </summary>
        public void RemoveNode(Node node)
        {
            var graph = node.Graph;

            var sourceGraph = PrefabUtility.GetCorrespondingObjectFromSource(target);
            var sourceNode = PrefabUtility.GetCorrespondingObjectFromSource(node);

            var removedAsset = sourceNode != null ? (Object)sourceNode : node;
            var targetGraph = sourceGraph ? sourceGraph : target;

            var targetNode = sourceNode ? sourceNode : node;
            targetGraph.RemoveNode(targetNode);

            Object.DestroyImmediate(removedAsset,true);

            AssetDatabase.SaveAssets();

            if (sourceGraph && targetGraph)
            {
                PrefabUtility.ApplyPrefabInstance(targetGraph.gameObject,InteractionMode.AutomatedAction);
            }
            
        }
    }
}