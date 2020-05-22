namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor {
    using Runtime.Core;
    using UnityEditor;
    using UnityEngine;

    /// <summary> Deals with modified assets </summary>
    class NodeEditorAssetModProcessor : UnityEditor.AssetModificationProcessor {

        /// <summary> Automatically delete Node sub-assets before deleting their script.
        /// <para/> This is important to do, because you can't delete null sub assets. </summary> 
        private static AssetDeleteResult OnWillDeleteAsset (string path, RemoveAssetOptions options) {
            // Get the object that is requested for deletion
            var obj = AssetDatabase.LoadAssetAtPath<Object> (path);

            // If we aren't deleting a script, return
            if (!(obj is MonoScript)) return AssetDeleteResult.DidNotDelete;

            // Check script type. Return if deleting a non-node script
            var script = obj as MonoScript;
            var scriptType = script.GetClass ();
            if (scriptType == null || (scriptType != typeof (Node) && !scriptType.IsSubclassOf (typeof (Node)))) return AssetDeleteResult.DidNotDelete;

            // Find all ScriptableObjects using this script
            var guids = AssetDatabase.FindAssets ("t:" + scriptType);
            for (var i = 0; i < guids.Length; i++) {
                var assetpath = AssetDatabase.GUIDToAssetPath (guids[i]);
                var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath (assetpath);
                for (var k = 0; k < objs.Length; k++) {
                    var node = objs[k] as Node;
                    if (node.GetType () == scriptType) {
                        if (node != null && node.GraphData != null) {
                            // Delete the node and notify the user
                            Debug.LogWarning (node.name + " of " + node.GraphData + " depended on deleted script and has been removed automatically.", node);
                            node.GraphData.RemoveNode (node);
                        }
                    }
                }
            }
            // We didn't actually delete the script. Tell the internal system to carry on with normal deletion procedure
            return AssetDeleteResult.DidNotDelete;
        }

    }
}