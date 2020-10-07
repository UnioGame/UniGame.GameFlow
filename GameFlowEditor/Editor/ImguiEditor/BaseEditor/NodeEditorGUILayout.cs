namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Drawers;
    using Drawers.ReactivePortDrawers;
    using Extensions;
    using Interfaces;
    using Runtime.Attributes;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UniModules.UniCore.EditorTools.Editor.Utility;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniModules.UniGame.Core.Runtime.Attributes.FieldTypeDrawer;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Extensions;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Profiling;
    using Object = UnityEngine.Object;

    /// <summary> UniNodeSystem-specific version of <see cref="EditorGUILayout"/> </summary>
    public static class NodeEditorGUILayout
    {
        private static readonly Dictionary<UnityEngine.Object, Dictionary<string, ReorderableList>> reorderableListCache
            = new Dictionary<UnityEngine.Object, Dictionary<string, ReorderableList>>();

        private static int reorderableListIndex = -1;

        public static IEnumerable<PropertyEditorData> GetProperties(this INode node,SerializedObject serializedObject)
        {
            return node.GetProperties(serializedObject.GetIterator(), null);
        }

        private static List<string> excludes = new List<string>() {"m_Script"};
        
        public static void DrawNode(this INode node, Object target)
        {
            var serializedObject = new SerializedObject(target);
            
            var drawedItems      = node.GetProperties(serializedObject);

            foreach (var item in drawedItems) {
                if (!node.IsFieldVisible(item.Type, item.Name))
                    continue;
                
                node.DrawNodePropertyField(
                    item.Property,
                    new GUIContent(item.Name, item.Tooltip),
                    true);
            }
        }
        
        public static bool IsFieldVisible(this object value, Type type, string fieldName)
        {
            if (excludes.Contains(fieldName))
                return false;
            
            //is node field should be draw
            var field         = type.GetFieldInfoCached(fieldName);
            var hideInspector = field?.GetCustomAttributes(typeof(HideNodeInspectorAttribute), false).Length > 0;
            return !hideInspector;
        }

        public static IEnumerable<PropertyEditorData> GetProperties(
            this object target,
            SerializedProperty targetProperty,
            SerializedProperty parent)
        {
            var property = targetProperty.Copy();
            var type = target.GetType();
            
            var moveNext = property.NextVisible(true);
            var next     = parent?.GetNextArrayProperty(targetProperty);

            while (moveNext 
                   && !property.IsEquals(targetProperty) 
                   && (next == null || !next.IsEquals(property)))
            {
                var propertyData = new PropertyEditorData() {
                    Target   = target,
                    Name     = property.name,
                    Tooltip  = property.tooltip,
                    Source   = target,
                    Type     = type,
                    Property = property.Copy(),
                };
                yield return propertyData;
                moveNext = property.NextVisible(false);
            }
        }
        
        
        /// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
        public static void DrawNodePropertyField(
            this INode node,
            SerializedProperty property,
            bool includeChildren)
        {
            node.DrawNodePropertyField(property, GUIContent.none, includeChildren);
        }

        /// <summary> Make a field for a serialized property. Automatically displays relevant node port. </summary>
        public static void DrawNodePropertyField(
            this INode node,
            SerializedProperty property,
            GUIContent label,
            bool includeChildren,
            params GUILayoutOption[] options)
        {
            if (property == null || node == null)
                throw new NullReferenceException();

            var port = node.GetPort(property.name);

            node.DrawNodePropertyField(property, label, port, includeChildren, options);
        }

        /// <summary> Make a field for a serialized property. Manual node port override. </summary>
        public static void DrawNodePropertyField(
            this INode node,
            SerializedProperty property,
            NodePort port,
            bool includeChildren = true, params GUILayoutOption[] options)
        {
            node.DrawNodePropertyField(property, null, port, includeChildren, options);
        }

        public static void ShowBackingValueField(
            PortField data,
            SerializedProperty property,
            bool isConnected,
            bool includeChildren,
            GUIStyle style = null, params GUILayoutOption[] options)
        {
            var portData     = data.PortData;
            var labelContent = portData.ItemName != null ? new GUIContent(portData.ItemName) : new GUIContent(property.displayName);

            switch (portData.ShowBackingValue) {
                case ShowBackingValue.Unconnected:
                    // Display a label if port is connected
                    if (isConnected) {
                        EditorDrawerUtils.DrawLabelField(labelContent, style, options);
                    }
                    // Display an editable property field if port is not connected
                    else {
                        ReactivePortDrawer.DrawPropertyValue(data.Value, property, labelContent);
                    }

                    break;
                case ShowBackingValue.Never:
                    // Display a label
                    EditorDrawerUtils.DrawLabelField(labelContent, style, options);
                    break;
                case ShowBackingValue.Always:
                    // Display an editable property field
                    ReactivePortDrawer.DrawPropertyValue(data.Value, property, labelContent);
                    break;
            }
        }


        /// <summary> Make a field for a serialized property. Manual node port override. </summary>
        public static void DrawNodePropertyField(
            this INode node,
            SerializedProperty property,
            GUIContent label,
            INodePort port,
            bool includeChildren = true,
            params GUILayoutOption[] options)
        {
            if (property == null) return;

            label = label ?? new GUIContent(property.name);

            // If property is not a port, display a regular property field
            if (port == null) {
                EditorGUILayout.PropertyField(property, label, includeChildren, GUILayout.MinWidth(30));
            }
            else {
                DrawFieldPort(property, label, node, port, includeChildren, options);
            }
        }

        private static void DrawFieldPort(
            SerializedProperty property,
            GUIContent label,
            INode node,
            INodePort port,
            bool includeChildren = true,
            params GUILayoutOption[] options)
        {
            var rect = new Rect();

            var data     = node.GetPortData(node.GetType(), property.name);
            var portData = data.PortData;

            if (portData.InstancePortList) {
                InstancePortList(property.name,
                    port.ValueType,
                    property.serializedObject,
                    portData.Direction,
                    portData.ConnectionType);
                return;
            }

            ShowBackingValueField(data, property, port.IsConnected, includeChildren, null, GUILayout.MinWidth(30));

            rect          = GUILayoutUtility.GetLastRect();
            rect.position = port.Direction == PortIO.Input ? rect.position - new Vector2(16, 0) : rect.position + new Vector2(rect.width, 0);
            rect.size     = new Vector2(16, 16);

            Color backgroundColor = new Color32(90, 97, 105, 255);
            if (NodeEditorWindow.nodeTint.TryGetValue(port.Node.GetType(), out var tint)) {
                backgroundColor *= tint;
            }

            Profiler.BeginSample("DrawPortHandle");
            var col = GameFlowPreferences.GetTypeColor(port.ValueType);
            DrawPortHandle(rect, backgroundColor, col);
            Profiler.EndSample();

            // Register the handle position
            var portPos = rect.center;

            //TODO REMOTE
            NodeEditor.PortPositions[port] = portPos;

            portData.Despawn();
        }

        private static Type GetType(SerializedProperty property)
        {
            var parentType = property.serializedObject.targetObject.GetType();
            var fi         = parentType.GetField(property.propertyPath);
            return fi.FieldType;
        }

        /// <summary> Make a simple port field. </summary>
        public static void PortField(INodePort port, params GUILayoutOption[] options)
        {
            PortField(null, port, options);
        }

        /// <summary> Make a simple port field. </summary>
        public static void PortField(GUIContent label, 
            INodePort port,
            params GUILayoutOption[] layoutOptions)
        {
            if (port == null) return;

            var defaultStyle = GetDefaultPortStyle(port);
            defaultStyle.Label = label;

            if (layoutOptions != null)
                defaultStyle.Options = layoutOptions;

            PortField(port, defaultStyle);
        }

        public static void PortField(INodePort port, NodeGuiLayoutStyle portStyle)
        {
            if (port == null) return;

            if (portStyle.Options == null)
                portStyle.Options = new GUILayoutOption[] {GUILayout.MinWidth(30)};

            Vector2 position = Vector3.zero;
            portStyle.Label = portStyle.Label != null ? portStyle.Label
                : string.IsNullOrEmpty(portStyle.Name) ? new GUIContent(ObjectNames.NicifyVariableName(port.ItemName))
                : new GUIContent(portStyle.Name);

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.Direction == PortIO.Input) {
                // Display a label
                EditorGUILayout.LabelField(portStyle.Label, portStyle.Options);

                var rect = GUILayoutUtility.GetLastRect();
                position = rect.position - new Vector2(16, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.Direction == PortIO.Output) {
                // Display a label
                EditorGUILayout.LabelField(portStyle.Label, NodeEditorResources.OutputPort, portStyle.Options);
                var rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }

            PortField(position, port, portStyle.Color, portStyle.Background);
        }


        /// <summary> Make a simple port field. </summary>
        public static void PortField(Vector2 position, INodePort port)
        {
            if (port == null) return;

            var col = GetMainPortColor(port);

            PortField(position, port, col);
        }

        public static NodeGuiLayoutStyle GetDefaultPortStyle(INodePort port)
        {
            var name  = port == null ? string.Empty : port.ItemName;
            var label = port == null ? new GUIContent(string.Empty) : new GUIContent(port.ItemName);

            var style = new NodeGuiLayoutStyle() {
                Color      = GetMainPortColor(port),
                Background = GetBackgroundPortColor(port),
                Options    = new GUILayoutOption[] {GUILayout.MinWidth(30)},
                Label      = label,
                Name       = name,
            };

            return style;
        }

        public static Color GetMainPortColor(INodePort port)
        {
            if (port == null)
                return Color.magenta;

            return GameFlowPreferences.GetTypeColor(port.ValueType);
        }

        public static Color GetBackgroundPortColor(INodePort port)
        {
            Color backgroundColor = new Color32(90, 97, 105, 255);
            if (port == null)
                return backgroundColor;

            if (NodeEditorWindow.nodeTint.TryGetValue(port.Node.GetType(), out var tint)) backgroundColor *= tint;
            return backgroundColor;
        }

        public static void PortField(Vector2 position, INodePort port, Color color)
        {
            if (port == null) return;

            var backgroundColor = GetBackgroundPortColor(port);

            PortField(position, port, color, backgroundColor);
        }

        public static void PortField(Vector2 position, INodePort port, Color color, Color backgroundColor)
        {
            if (port == null) return;

            var rect = new Rect(position, new Vector2(16, 16));

            DrawPortHandle(rect, backgroundColor, color);

            // Register the handle position
            var portPos = rect.center;

            NodeEditor.PortPositions[port] = portPos;
        }

        /// <summary> Add a port field to previous layout element. </summary>
        public static void AddPortField(NodePort port)
        {
            if (port == null) return;
            var rect = new Rect();

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.Direction == PortIO.Input) {
                rect          = GUILayoutUtility.GetLastRect();
                rect.position = rect.position - new Vector2(16, 0);
                // If property is an output, display a text label and put a port handle on the right side
            }
            else if (port.Direction == PortIO.Output) {
                rect          = GUILayoutUtility.GetLastRect();
                rect.position = rect.position + new Vector2(rect.width, 0);
            }

            rect.size = new Vector2(16, 16);

            Color backgroundColor = new Color32(90, 97, 105, 255);
            Color tint;
            if (NodeEditorWindow.nodeTint.TryGetValue(port.Node.GetType(), out tint)) backgroundColor *= tint;
            var col                                                                                   = GameFlowPreferences.GetTypeColor(port.ValueType);
            DrawPortHandle(rect, backgroundColor, col);

            // Register the handle position
            var portPos = rect.center;
            if (NodeEditor.PortPositions.ContainsKey(port)) NodeEditor.PortPositions[port] = portPos;
            else NodeEditor.PortPositions.Add(port, portPos);
        }

        /// <summary> Draws an input and an output port on the same line </summary>
        public static void PortPair(NodePort input, NodePort output)
        {
            GUILayout.BeginHorizontal();
            PortField(input, GUILayout.MinWidth(0));
            PortField(output, GUILayout.MinWidth(0));
            GUILayout.EndHorizontal();
        }

        public static void PortPair(INodePort input, INodePort output,
            NodeGuiLayoutStyle intputStyle, NodeGuiLayoutStyle outputStyle)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            PortField(input, intputStyle);
            PortField(output, outputStyle);

            GUILayout.EndHorizontal();
        }

        public static void DrawPortHandle(Rect rect, Color backgroundColor, Color typeColor)
        {
            var col = GUI.color;
            GUI.color = backgroundColor;
            GUI.DrawTexture(rect, NodeEditorResources.dotOuter);
            GUI.color = typeColor;
            GUI.DrawTexture(rect, NodeEditorResources.dot);
            GUI.color = col;
        }

        [Obsolete("Use InstancePortList(string, Type, SerializedObject, NodePort.IO, ConnectionType) instead")]
        public static void InstancePortList(string fieldName, Type type, SerializedObject serializedObject,
            ConnectionType connectionType = ConnectionType.Multiple)
        {
            InstancePortList(fieldName, type, serializedObject, PortIO.Output, connectionType);
        }


        /// <summary> Draw an editable list of instance ports. Port names are named as "[fieldName] [index]" </summary>
        /// <param name="fieldName">Supply a list for editable values</param>
        /// <param name="type">Value type of added instance ports</param>
        /// <param name="serializedObject">The serializedObject of the node</param>
        /// <param name="connectionType">Connection type of added instance ports</param>
        public static void InstancePortList(string fieldName, Type type, SerializedObject serializedObject,
            PortIO io,
            ConnectionType connectionType = ConnectionType.Multiple)
        {
            var node = serializedObject.targetObject as Node;

            var arrayData = serializedObject.FindProperty(fieldName);

            bool IsMatchingInstancePort(string x)
            {
                var split = x.Split(' ');
                return split.Length == 2 && split[0] == fieldName;
            }

            var instancePorts = node.Ports.Where(x => IsMatchingInstancePort(x.ItemName)).OrderBy(x => x.ItemName).ToList();

            ReorderableList                     list = null;
            Dictionary<string, ReorderableList> rlc;
            if (reorderableListCache.TryGetValue(serializedObject.targetObject, out rlc)) {
                if (!rlc.TryGetValue(fieldName, out list)) list = null;
            }

            // If a ReorderableList isn't cached for this array, do so.
            if (list == null) {
                var label = serializedObject.FindProperty(fieldName).displayName;
                list = CreateReorderableList(instancePorts, arrayData, type, serializedObject, io, label,
                    connectionType);
                if (reorderableListCache.TryGetValue(serializedObject.targetObject, out rlc)) rlc.Add(fieldName, list);
                else
                    reorderableListCache.Add(serializedObject.targetObject,
                        new Dictionary<string, ReorderableList>() {{fieldName, list}});
            }

            list.list = instancePorts;
            list.DoLayoutList();
        }

        private static ReorderableList CreateReorderableList(
            List<INodePort> instancePorts,
            SerializedProperty arrayData, 
            Type type, 
            SerializedObject serializedObject, 
            PortIO io,
            string label, 
            ConnectionType connectionType = ConnectionType.Multiple)
        {
            var hasArrayData = arrayData != null && arrayData.isArray;
            var arraySize    = hasArrayData ? arrayData.arraySize : 0;
            var node         = serializedObject.targetObject as Node;
            var portConnections =
                NodeEditorWindow.ActiveWindows.FirstOrDefault(x => x.ActiveGraph == node.GraphData).PortConnectionPoints;

            var list = new ReorderableList(instancePorts, null, true, true, true, true);

            list.drawElementCallback =
                (rect, index, isActive, isFocused) => {
                    var port = node.GetPort(arrayData.name + " " + index);
                    if (hasArrayData) {
                        var itemData = arrayData.GetArrayElementAtIndex(index);
                        EditorGUI.PropertyField(rect, itemData);
                    }
                    else EditorGUI.LabelField(rect, port.ItemName);

                    var pos = rect.position + (port.Direction == PortIO.Output ? 
                                  new Vector2(rect.width + 6, 0) : 
                                  new Vector2(-36, 0));
                    
                    PortField(pos, port);
                };
            list.elementHeightCallback =
                index => {
                    if (hasArrayData) {
                        var itemData = arrayData.GetArrayElementAtIndex(index);
                        return EditorGUI.GetPropertyHeight(itemData);
                    }
                    else return EditorGUIUtility.singleLineHeight;
                };
            list.drawHeaderCallback =
                rect => { EditorGUI.LabelField(rect, label); };
            list.onSelectCallback =
                rl => { reorderableListIndex = rl.index; };
            list.onReorderCallback = rl => {
                // Move up
                if (rl.index > reorderableListIndex) {
                    for (var i = reorderableListIndex; i < rl.index; ++i) {
                        var port     = node.GetPort(arrayData.name + " " + i);
                        var nextPort = node.GetPort(arrayData.name + " " + (i + 1));
  
                        port.SwapConnections(nextPort);

                        // Swap cached positions to mitigate twitching
                        var rect = portConnections[port];
                        portConnections[port] = portConnections[port];
                        portConnections[port] = rect;
                    }
                }
                // Move down
                else {
                    for (var i = reorderableListIndex; i > rl.index; --i) {
                        var port     = node.GetPort(arrayData.name + " " + i);
                        var nextPort = node.GetPort(arrayData.name + " " + (i - 1));
                        
                        port.SwapConnections(nextPort);

                        // Swap cached positions to mitigate twitching
                        var rect = portConnections[port];
                        portConnections[port] = portConnections[port];
                        portConnections[port] = rect;
                    }
                }

                // Apply changes
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                // Move array data if there is any
                if (hasArrayData) {
                    var arrayDataOriginal = arrayData.Copy();
                    arrayData.MoveArrayElement(reorderableListIndex, rl.index);
                }

                // Apply changes
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                var window = NodeEditorWindow.ActiveWindows.FirstOrDefault(x => x.ActiveGraph == node.GraphData);
                window.Repaint();
                //                    EditorApplication.delayCall += window.Repaint;
            };
            list.onAddCallback = rl => {
                // Add instance port postfixed with an index number
                var newName = arrayData.name + " 0";
                var i       = 0;
                while (node.HasPort(newName))
                    newName = arrayData.name + " " + (++i);

                var types = new List<Type>();
                node.AddPort(newName, types, io, connectionType);

                serializedObject.Update();
                EditorUtility.SetDirty(node);
                if (hasArrayData) arrayData.InsertArrayElementAtIndex(arraySize);
                serializedObject.ApplyModifiedProperties();
            };
            list.onRemoveCallback =
                rl => {
                    var index = rl.index;
                    // Clear the removed ports connections
                    instancePorts[index].ClearConnections();
                    // Move following connections one step up to replace the missing connection
                    for (var k = index + 1; k < instancePorts.Count(); k++) {
                        for (var j = 0; j < instancePorts[k].ConnectionCount; j++) {
                            var other = instancePorts[k].GetConnection(j);
                            instancePorts[k].Disconnect(other as NodePort);
                            instancePorts[k - 1].Connect(other as NodePort);
                        }
                    }

                    // Remove the last instance port, to avoid messing up the indexing
                    node.RemovePort(instancePorts[instancePorts.Count() - 1].ItemName);
                    serializedObject.Update();
                    EditorUtility.SetDirty(node);
                    if (hasArrayData) {
                        arrayData.DeleteArrayElementAtIndex(index);
                        arraySize--;
                        // Error handling. If the following happens too often, file a bug report at https://github.com/Siccity/UniNodeSystem/issues
                        if (instancePorts.Count <= arraySize) {
                            while (instancePorts.Count <= arraySize) {
                                arrayData.DeleteArrayElementAtIndex(--arraySize);
                            }

                            Debug.LogWarning("Array size exceeded instance ports size. Excess items removed.");
                        }

                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    }
                };

            if (hasArrayData) {
                var instancePortCount = instancePorts.Count;
                while (instancePortCount < arraySize) {
                    // Add instance port postfixed with an index number
                    var newName = arrayData.name + " 0";
                    var i       = 0;
                    while (node.HasPort(newName))
                        newName = arrayData.name + " " + (++i);

                    var types = new List<Type>();
                    node.AddPort(newName, types, io, connectionType, ShowBackingValue.Always);

                    EditorUtility.SetDirty(node);

                    instancePortCount++;
                }

                while (arraySize < instancePortCount) {
                    arrayData.InsertArrayElementAtIndex(arraySize);
                    arraySize++;
                }

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            return list;
        }
    }
}