using UniModules.GameFlow.Runtime.Interfaces;
using UnityEditor;

namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    public struct EditorNodeData
    {
        public INode              Node;
        public SerializedProperty Property;
        public SerializedProperty Parent;
        public SerializedObject   Source;
    }
}