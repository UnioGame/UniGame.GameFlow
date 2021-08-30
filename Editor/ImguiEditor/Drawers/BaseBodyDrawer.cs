namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using BaseEditor;
    using BaseEditor.Interfaces;
    using Interfaces;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniGame.Editor.DrawersTools;
    using UnityEditor;
    using UnityEngine;

    public class BaseBodyDrawer : INodeEditorHandler
    {
        private NodeFieldsContainer nodeFields = new NodeFieldsContainer();

        private int counter = 0;

        public bool Update(INodeEditorData editor, INode node)
        {
            EditorGUIUtility.labelWidth = 84;
            Draw(editor.EditorNode);
            return true;
        }

        public void Draw(EditorNode nodeData)
        {
            foreach (var item in nodeFields.GetFields(nodeData)) {
                var node = item.Target as INode;
                node.DrawNodePropertyField(
                    item.Property,
                    new GUIContent(
                        item.Name, 
                        item.Tooltip),true);
            }
        }

    }
}