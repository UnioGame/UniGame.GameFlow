namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ContentContextWindow;
    using Runtime.Attributes;
    using Runtime.Core;
    using Runtime.Core.Extensions;
    using Runtime.Core.Nodes;
    using Runtime.Interfaces;
    using Sirenix.Utilities;
    using UniGreenModules.UniCore.EditorTools.Editor.Utility;
    using UniModules.UniGameFlow.GameFlowEditor.Editor.Tools;
    using UniRx;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public struct EditorNode
    {
        public INode              Node;
        public SerializedProperty Property;
        public SerializedProperty Parent;
        public SerializedObject   Source;
    }

    /// <summary> Contains GUI methods </summary>
    public partial class NodeEditorWindow
    {
        public NodeGraphEditor graphEditor;

        private List<INode> selection   = new List<INode>();
        private List<INode> culledNodes = new List<INode>();

        private List<EditorNode> regularNodes  = new List<EditorNode>();
        private List<EditorNode> selectedNodes = new List<EditorNode>();

        private Vector2 activeGraphsScroll;

        private int topPadding => isDocked() ? 19 : 22;

        private SerializableNodeContainer nodeContainer;

        public SerializableNodeContainer Container {
            get {
                if (!nodeContainer)
                    nodeContainer = ScriptableObject.CreateInstance<SerializableNodeContainer>();
                return nodeContainer;
            }
        }

        private void OnGUI()
        {
            var e = Event.current;
            var m = GUI.matrix;
            if (ActiveGraph == null ||
                e.type == EventType.Ignore ||
                e.rawType == EventType.Ignore) {
                return;
            }

            //Initialize(ActiveGraph);

            var editorNode = new EditorNode() {
                Node     = ActiveGraph,
                Parent = null,
                Property = null,
            };

            graphEditor          = NodeGraphEditor.GetEditor(editorNode);
            graphEditor.position = position;

            if (EditorApplication.isPlayingOrWillChangePlaymode == false) {
                ActiveGraph.Initialize(ActiveGraph);
                ActiveGraph.Validate();
            }

            Controls();

            DrawGrid(position, Zoom, PanOffset);
            DrawZoomedNodes();
            DrawConnections();
            DrawDraggedConnection();
            DrawSelectionBox();
            DrawTooltip();
            DrawGraphsControlls();

            graphEditor.OnGUI();

            GUI.matrix = m;
        }


        public static void BeginZoomed(Rect rect, float zoom, float topPadding)
        {
            GUI.EndClip();

            GUIUtility.ScaleAroundPivot(Vector2.one / zoom, rect.size * 0.5f);
            var padding = new Vector4(0, topPadding, 0, 0);
            padding *= zoom;

            GUI.BeginClip(new Rect(-((rect.width * zoom) - rect.width) * 0.5f,
                -(((rect.height * zoom) - rect.height) * 0.5f) + (topPadding * zoom),
                rect.width * zoom,
                rect.height * zoom));
        }

        public static void EndZoomed(Rect rect, float zoom, float topPadding)
        {
            GUIUtility.ScaleAroundPivot(Vector2.one * zoom, rect.size * 0.5f);
            var offset = new Vector3(
                (((rect.width * zoom) - rect.width) * 0.5f),
                (((rect.height * zoom) - rect.height) * 0.5f) + (-topPadding * zoom) + topPadding,
                0);
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
        }

        public void DrawGrid(Rect rect, float zoom, Vector2 panOffset)
        {
            rect.position = Vector2.zero;

            var center   = rect.size / 2f;
            var gridTex  = graphEditor.GetGridTexture();
            var crossTex = graphEditor.GetSecondaryGridTexture();

            // Offset from origin in tile units
            var xOffset = -(center.x * zoom + panOffset.x) / gridTex.width;
            var yOffset = ((center.y - rect.size.y) * zoom + panOffset.y) / gridTex.height;

            var tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            var tileAmountX = Mathf.Round(rect.size.x * zoom) / gridTex.width;
            var tileAmountY = Mathf.Round(rect.size.y * zoom) / gridTex.height;

            var tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(rect, gridTex, new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(rect, crossTex, new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }

        public void DrawSelectionBox()
        {
            if (currentActivity != NodeActivity.DragGrid) {
                return;
            }

            var curPos = WindowToGridPosition(Event.current.mousePosition);
            var size   = curPos - dragBoxStart;
            var r      = new Rect(dragBoxStart, size);
            r.position =  GridToWindowPosition(r.position) / Zoom;
            r.size     /= Zoom;
            Handles.DrawSolidRectangleWithOutline(r, new Color(0, 0, 0, 0.1f), new Color(1, 1, 1, 0.6f));
        }

        public static bool DropdownButton(string name, float width)
        {
            return GUILayout.Button(name, EditorStyles.toolbarDropDown, GUILayout.Width(width));
        }

        /// <summary> Show right-click context menu for hovered reroute </summary>
        void ShowRerouteContextMenu(RerouteReference reroute)
        {
            var contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Remove"), false, () => reroute.RemovePoint());
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        /// <summary> Show right-click context menu for hovered port </summary>
        void ShowPortContextMenu(INodePort hoveredPort)
        {
            var contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Clear Connections"), false, hoveredPort.ClearConnections);
            contextMenu.AddItem(new GUIContent("Show Content"), false, () => ShowPortContextValues(hoveredPort.Value));
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        public void ShowPortContextValues(IPortValue port)
        {
            ContextContentWindow.Open(new ContextDescription() {
                Data = port,
                Label = port.ItemName
            });
        }

        /// <summary>
        /// Show right-click context menu for selected nodes
        /// </summary>
        public void ShowNodeContextMenu()
        {
            var contextMenu = new GenericMenu();
            // If only one node is selected
            if (selection.Count == 1) {
                var node = selection.FirstOrDefault();
                contextMenu.AddItem(new GUIContent("Rename"), false, RenameSelectedNode);
                // If only one node is selected
                AddCustomContextMenuItems(contextMenu, node);
            }

            contextMenu.AddItem(new GUIContent("Duplicate"), false, DublicateSelectedNodes);
            contextMenu.AddItem(new GUIContent("Remove"), false, RemoveSelectedNodes);

            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        /// <summary>
        /// Show right-click context menu for current graph
        /// </summary>
        void ShowGraphContextMenu()
        {
            var contextMenu = new GenericMenu();
            var pos         = WindowToGridPosition(Event.current.mousePosition);
            for (var i = 0; i < NodeTypes.Count; i++) {
                var type = NodeTypes[i];

                if (IsValidNode(type) == false)
                    continue;

                //Get node context menu path
                var path = type.GetNodeMenuName();
                if (string.IsNullOrEmpty(path)) continue;

                contextMenu.AddItem(new GUIContent(path), false, () => { CreateNode(type, pos); });
            }

            contextMenu.AddSeparator("");
            //contextMenu.AddItem(new GUIContent("Preferences"), false, () => OpenPreferences());
            AddCustomContextMenuItems(contextMenu, ActiveGraph);
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        private bool IsValidNode(Type nodeType)
        {
            return !NodeEditorUtilities.GetAttrib<HideNodeAttribute>(nodeType, out var hideNodeAttribute);
        }

        void AddCustomContextMenuItems(GenericMenu contextMenu, object obj)
        {
            var items = GetContextMenuMethods(obj);
            if (items.Length != 0) {
                contextMenu.AddSeparator("");
                for (var i = 0; i < items.Length; i++) {
                    var kvp = items[i];
                    contextMenu.AddItem(new GUIContent(kvp.Key.menuItem), false, () => kvp.Value.Invoke(obj, null));
                }
            }
        }

        /// <summary>
        /// Draw a bezier from startpoint to endpoint, both in grid coordinates
        /// </summary>
        public void DrawConnection(Vector2 startPoint, Vector2 endPoint, Color col)
        {
            startPoint = GridToWindowPosition(startPoint);
            endPoint   = GridToWindowPosition(endPoint);

            switch (this.GetSettings().noodleType) {
                case NodeEditorNoodleType.Curve:
                    var startTangent = startPoint;
                    if (startPoint.x < endPoint.x) startTangent.x = Mathf.LerpUnclamped(startPoint.x, endPoint.x, 0.7f);
                    else startTangent.x                           = Mathf.LerpUnclamped(startPoint.x, endPoint.x, -0.7f);

                    var endTangent = endPoint;
                    if (startPoint.x > endPoint.x) endTangent.x = Mathf.LerpUnclamped(endPoint.x, startPoint.x, -0.7f);
                    else endTangent.x                           = Mathf.LerpUnclamped(endPoint.x, startPoint.x, 0.7f);
                    Handles.DrawBezier(startPoint, endPoint, startTangent, endTangent, col, null, 4);
                    break;
                case NodeEditorNoodleType.Line:
                    Handles.color = col;
                    Handles.DrawAAPolyLine(5, startPoint, endPoint);
                    break;
                case NodeEditorNoodleType.Angled:
                    Handles.color = col;
                    if (startPoint.x <= endPoint.x - (50 / Zoom)) {
                        var midpoint = (startPoint.x + endPoint.x) * 0.5f;
                        var start_1  = startPoint;
                        var end_1    = endPoint;
                        start_1.x = midpoint;
                        end_1.x   = midpoint;
                        Handles.DrawAAPolyLine(5, startPoint, start_1);
                        Handles.DrawAAPolyLine(5, start_1, end_1);
                        Handles.DrawAAPolyLine(5, end_1, endPoint);
                    }
                    else {
                        var midpoint = (startPoint.y + endPoint.y) * 0.5f;
                        var start_1  = startPoint;
                        var end_1    = endPoint;
                        start_1.x += 25 / Zoom;
                        end_1.x   -= 25 / Zoom;
                        var start_2 = start_1;
                        var end_2   = end_1;
                        start_2.y = midpoint;
                        end_2.y   = midpoint;
                        Handles.DrawAAPolyLine(5, startPoint, start_1);
                        Handles.DrawAAPolyLine(5, start_1, start_2);
                        Handles.DrawAAPolyLine(5, start_2, end_2);
                        Handles.DrawAAPolyLine(5, end_2, end_1);
                        Handles.DrawAAPolyLine(5, end_1, endPoint);
                    }

                    break;
            }
        }

        /// <summary> Draws all connections </summary>
        public void DrawConnections()
        {
            var mousePos = Event.current.mousePosition;
            var preSelection = preBoxSelectionReroute != null
                ? new List<RerouteReference>(preBoxSelectionReroute)
                : new List<RerouteReference>();
            hoveredReroute = new RerouteReference();

            var col = GUI.color;
            foreach (var node in ActiveGraph.Nodes) {
                //If a null node is found, return. This can happen if the nodes associated script is deleted. It is currently not possible in Unity to delete a null asset.
                if (node == null) continue;

                // Draw full connections and output > reroute
                foreach (var output in node.Outputs) {
                    //Needs cleanup. Null checks are ugly
                    var item = _portConnectionPoints.FirstOrDefault(x => x.Key.IsEqual(output));

                    if (item.Key == null) continue;
                    if (output == null)
                        continue;

                    var types           = output.ValueTypes;
                    var fromRect        = item.Value;
                    var connectionColor = GameFlowPreferences.GetTypeColor(types.FirstOrDefault());

                    for (var k = 0; k < output.ConnectionCount; k++) {
                        var input = output.GetConnection(k);

                        // Error handling
                        if (input == null)
                            continue; //If a script has been updated and the port doesn't exist, it is removed and null is returned. If this happens, return.
                        if (!input.IsConnectedTo(output)) {
                            input.Connect(output);
                        }

                        Rect toRect;
                        if (!PortConnectionPoints.TryGetValue(input, out toRect)) continue;

                        var from          = fromRect.center;
                        var to            = Vector2.zero;
                        var reroutePoints = output.GetReroutePoints(k);
                        // Loop through reroute points and draw the path
                        for (var i = 0; i < reroutePoints.Count; i++) {
                            to = reroutePoints[i];
                            DrawConnection(from, to, connectionColor);
                            from = to;
                        }

                        to = toRect.center;

                        DrawConnection(from, to, connectionColor);

                        // Loop through reroute points again and draw the points
                        for (var i = 0; i < reroutePoints.Count; i++) {
                            var rerouteRef = new RerouteReference(output, k, i);
                            // Draw reroute point at position
                            var rect = new Rect(reroutePoints[i], new Vector2(12, 12));
                            rect.position = new Vector2(rect.position.x - 6, rect.position.y - 6);
                            rect          = GridToWindowRect(rect);

                            // Draw selected reroute points with an outline
                            if (selectedReroutes.Contains(rerouteRef)) {
                                GUI.color = this.GetSettings().highlightColor;
                                GUI.DrawTexture(rect, NodeEditorResources.dotOuter);
                            }

                            GUI.color = connectionColor;
                            GUI.DrawTexture(rect, NodeEditorResources.dot);
                            if (rect.Overlaps(selectionBox)) preSelection.Add(rerouteRef);
                            if (rect.Contains(mousePos)) hoveredReroute = rerouteRef;
                        }
                    }
                }
            }

            GUI.color = col;
            if (Event.current.type != EventType.Layout && currentActivity == NodeActivity.DragGrid)
                selectedReroutes = preSelection;
        }

        private void DrawGraphsControlls()
        {
            DrawTopButtons();
        }

        private void DrawTopButtons()
        {
            EditorDrawerUtils.DrawHorizontalLayout(() => {
                EditorDrawerUtils.DrawButton("Save", () => Open(Save(ActiveGraph)),
                    GUILayout.Height(20), GUILayout.Width(200));

                EditorDrawerUtils.DrawButton("Refresh", Refresh,
                    GUILayout.Height(20), GUILayout.Width(200));

                EditorDrawerUtils.DrawButton("Ping", PingInEditor,
                    GUILayout.Height(20), GUILayout.Width(200));
            }, GUILayout.Height(100));
        }

        private void DrawNodes(Event activeEvent)
        {
            var mousePos = Event.current.mousePosition;

            if (activeEvent.type != EventType.Layout) {
                hoveredNode = null;
                hoveredPort = null;
            }

            var preSelection = preBoxSelection != null
                ? new List<int>(preBoxSelection)
                : new List<int>();

            // Selection box stuff
            var boxStartPos = GridToWindowPositionNoClipped(dragBoxStart);

            var boxSize = mousePos - boxStartPos;
            if (boxSize.x < 0) {
                boxStartPos.x += boxSize.x;
                boxSize.x     =  Mathf.Abs(boxSize.x);
            }

            if (boxSize.y < 0) {
                boxStartPos.y += boxSize.y;
                boxSize.y     =  Mathf.Abs(boxSize.y);
            }

            var selectionBox = new Rect(boxStartPos, boxSize);

            if (activeEvent.type == EventType.Layout)
                culledNodes = new List<INode>();

            var serializableNodes = ActiveObject.FindProperty(nameof(ActiveGraph.serializableNodes));
            var assetNodes        = ActiveObject.FindProperty(nameof(ActiveGraph.nodes));

            UpdateNodes(serializableNodes, ActiveGraph.SerializableNodes);
            UpdateNodes(assetNodes, ActiveGraph.ObjectNodes);

            var eventType = activeEvent.type == EventType.Ignore ||
                            activeEvent.rawType == EventType.Ignore
                ? EventType.Ignore
                : activeEvent.type;

            var editorGuiState = new NodeEditorGuiState() {
                MousePosition = mousePos,
                PreSelection  = preSelection,
                Event         = activeEvent,
                EventType     = eventType,
            };

            EditorDrawerUtils.DrawAndRevertColor(() => {
                DrawNodes(regularNodes, editorGuiState);
                DrawNodes(selectedNodes, editorGuiState);
            });

            regularNodes.Clear();
            selectedNodes.Clear();

            if (activeEvent.type != EventType.Layout && currentActivity == NodeActivity.DragGrid)
                selection.Where(x => preSelection.Contains(x.Id)).ForEach(x => x.AddToEditorSelection(true));
        }

        private void UpdateNodes(SerializedProperty property, IReadOnlyList<INode> nodes)
        {
            for (var i = 0; i < nodes.Count; i++) {
                var node = nodes[i];

                if (node == null || !property.isArray || property.arraySize <= i) {
                    continue;
                }

                var editorNode = new EditorNode() {
                    Node     = node,
                    Parent = property,
                    Property = property.GetArrayElementAtIndex(i),
                    Source = (node is Object nodeObject) ? 
                        new SerializedObject(nodeObject) : 
                        null, 
                };

                if (IsSelected(node)) {
                    selectedNodes.Add(editorNode);
                }
                else {
                    regularNodes.Add(editorNode);
                }
            }
        }

        public bool IsSelected(INode node) => selection.Contains(node);

        public void Deselect(INode node)
        {
            selection.Remove(node);
        }

        public void DeselectAll() => selection.Clear();

        public void Select(INode node, bool add)
        {
            if (add == false)
                selection.Clear();

            if (IsSelected(node))
                return;

            selection.Add(node);

            if (node is Object asset) {
                asset.AddToEditorSelection(add);
            }
            else {
                Container.Initialize(node as SerializableNode,node.GraphData as NodeGraph);
                Container.AddToEditorSelection(add);
            }
        }

        private void DrawNodes(List<EditorNode> nodes, NodeEditorGuiState state)
        {
            for (var n = 0; n < nodes.Count; n++) {
                var editorNode = nodes[n];
                // Skip null nodes. The user could be in the process of renaming scripts, so removing them at this point is not advisable.
                var node = editorNode.Node;
                if (node == null) continue;
                //initialize with graph data
                node.Initialize(ActiveGraph);

                EditorDrawerUtils.DrawAndRevertColor(() => DrawNode(ref editorNode, state));
            }
        }

        private void DrawZoomedNodes()
        {
            var e = Event.current;
            EditorDrawerUtils.DrawZoom(() => DrawNodes(e), position, Zoom, topPadding);
        }

        private bool IsIgnoredNode(INode node, NodeEditorGuiState state)
        {
            switch (state.EventType) {
                case EventType.Ignore:
                    return true;
                // Culling
                case EventType.Layout: {
                    // Cull unselected nodes outside view
                    if (!IsSelected(node) && ShouldBeCulled(node)) {
                        culledNodes.Add(node);
                        return true;
                    }

                    break;
                }
                default: {
                    if (culledNodes.Contains(node))
                        return true;
                    break;
                }
            }

            return false;
        }

        private void DrawNode(ref EditorNode editorNode, NodeEditorGuiState state)
        {
            var node = editorNode.Node;

            if (IsIgnoredNode(node, state))
                return;

            NodeEditor.PortPositions = new Dictionary<INodePort, Vector2>();

            DrawNodeArea(ref editorNode, state);
            
            if (state.EventType == EventType.Repaint) {
                _portConnectionPoints = _portConnectionPoints
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

        }

        private void DrawNodeArea(ref EditorNode editorNode, NodeEditorGuiState state)
        {
            //create node editor
            var nodeEditor = NodeEditor.GetEditor(editorNode);

            var node = editorNode.Node;

            nodeEditor.IsSelected = IsSelected(node);

            //Get node position
            var nodePos = GridToWindowPositionNoClipped(node.Position);
            //node area
            var rectArea = new Rect(nodePos, new Vector2(nodeEditor.GetWidth(), 4000));

            DrawNodeArea(nodeEditor, editorNode, nodePos, rectArea, state);
        }

        private void DrawNodeArea(
            NodeEditor nodeEditor,
            EditorNode editorNode,
            Vector2 nodePos,
            Rect rectArea,
            NodeEditorGuiState state)
        {
            if (state.EventType == EventType.Ignore) return;

            GUILayout.BeginArea(rectArea);

            this.WrapDrawer(() => DrawNodeEditorArea(nodeEditor, editorNode, state), true);
            this.WrapDrawer(() => DrawNodePorts(editorNode.Node, nodePos, state), true);

            GUILayout.EndArea();
        }

        private void DrawNodePorts(
            INode node,
            Vector2 nodePos,
            NodeEditorGuiState state)
        {
            var eventType  = state.EventType;
            var stateEvent = state.Event;
            if (eventType == EventType.Ignore ||
                stateEvent.type == EventType.Ignore ||
                stateEvent.rawType == EventType.Ignore)
                return;

            var mousePos     = state.MousePosition;
            var preSelection = state.PreSelection;

            //Check if we are hovering this node
            var nodeSize   = GUILayoutUtility.GetLastRect().size;
            var windowRect = new Rect(nodePos, nodeSize);
            if (windowRect.Contains(mousePos))
                hoveredNode = node;

            //If dragging a selection box, add nodes inside to selection
            if (currentActivity == NodeActivity.DragGrid) {
                if (windowRect.Overlaps(selectionBox))
                    preSelection.Add(node.Id);
            }

            //Check if we are hovering any of this nodes ports
            //Check input ports
            foreach (var input in node.Inputs) {
                //Check if port rect is available
                if (!PortConnectionPoints.ContainsKey(input)) continue;
                var r                                 = GridToWindowRectNoClipped(PortConnectionPoints[input]);
                if (r.Contains(mousePos)) hoveredPort = input;
            }

            //Check all output ports
            foreach (var output in node.Outputs) {
                //Check if port rect is available
                if (!PortConnectionPoints.ContainsKey(output)) continue;
                var r = GridToWindowRectNoClipped(PortConnectionPoints[output]);
                if (r.Contains(mousePos)) hoveredPort = output;
            }
        }

        private void DrawNodeEditorArea(
            NodeEditor nodeEditor,
            EditorNode editorNode,
            NodeEditorGuiState state)
        {
            var eventType  = state.EventType;
            var stateEvent = state.Event;
            var node       = editorNode.Node;

            if (eventType == EventType.Ignore ||
                stateEvent.type == EventType.Ignore ||
                stateEvent.rawType == EventType.Ignore)
                return;

            var guiColor = GUI.color;

            var selected = IsSelected(node);

            if (selected) {
                var style          = new GUIStyle(nodeEditor.GetBodyStyle());
                var highlightStyle = new GUIStyle(NodeEditorResources.styles.nodeHighlight);
                highlightStyle.padding = style.padding;
                style.padding          = new RectOffset();
                GUI.color              = nodeEditor.GetTint();
                GUILayout.BeginVertical(style);
                GUI.color = this.GetSettings().highlightColor;

                //TODO fix style
                GUILayout.BeginVertical(new GUIStyle(highlightStyle));
            }
            else {
                var style = new GUIStyle(nodeEditor.GetBodyStyle());
                GUI.color = nodeEditor.GetTint();
                GUILayout.BeginVertical(style);
            }

            GUI.color = guiColor;

            EditorGUI.BeginChangeCheck();

            //Draw node contents
            nodeEditor.OnHeaderGUI();
            nodeEditor.OnBodyGUI();

            //If user changed a value, notify other scripts through onUpdateNode
            if (EditorGUI.EndChangeCheck()) {
                if (NodeEditor.OnUpdateNode != null) NodeEditor.OnUpdateNode(node);
                node.SetDirty();
            }

            GUILayout.EndVertical();

            //Cache data about the node for next frame
            if (state.EventType == EventType.Repaint) {
                var size = GUILayoutUtility.GetLastRect().size;
                if (NodeSizes.ContainsKey(node)) NodeSizes[node] = size;
                else NodeSizes.Add(node, size);

                foreach (var portPairs in NodeEditor.PortPositions) {
                    var id = portPairs.Key;
                    var portHandlePos = portPairs.Value;
                    portHandlePos += node.Position;
                    var rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                    PortConnectionPoints[id] = rect;
                }
            }

            if (selected) GUILayout.EndVertical();
        }

        private bool ShouldBeCulled(INode node)
        {
            var nodePos = GridToWindowPositionNoClipped(node.Position);
            if (nodePos.x / _zoom > position.width) return true;  // Right
            if (nodePos.y / _zoom > position.height) return true; // Bottom
            if (NodeSizes.ContainsKey(node)) {
                var size = NodeSizes[node];
                if (nodePos.x + size.x < 0) return true; // Left
                if (nodePos.y + size.y < 0) return true; // Top
            }

            return false;
        }

        private void DrawTooltip()
        {
            if (hoveredPort != null) {
                var type            = hoveredPort.ValueType;
                var content         = new GUIContent();
                var portTypeTooltip = type.PrettyName();
                content.text = portTypeTooltip;

                if (hoveredPort.IsOutput) {
                    //TODO DRAW ACTUAL VALUE
                    //var obj = hoveredPort.node.GetValue(hoveredPort);
                    //content.text += " = " + (obj != null ? obj.ToString() : "null");
                }

                var size = NodeEditorResources.styles.tooltip.CalcSize(content);
                var rect = new Rect(Event.current.mousePosition - (size), size);
                EditorGUI.LabelField(rect, content, NodeEditorResources.styles.tooltip);
                Repaint();
            }
        }
    }
}