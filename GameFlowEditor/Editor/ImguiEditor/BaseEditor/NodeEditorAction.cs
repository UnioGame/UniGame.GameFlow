namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.EditorTools.Editor.Utility;
    using UniModules.UniGame.Core.Runtime.Extension;
    using UnityEditor;
    using UnityEngine;

    public partial class NodeEditorWindow
    {
        public static NodeActivity currentActivity = NodeActivity.Idle;
        public static bool isPanning { get; private set; }
        public static Vector2[] dragOffset;

        private bool IsDraggingPort => draggedOutput != null;

        private bool IsHoveringPort => hoveredPort != null;

        private bool IsHoveringNode => hoveredNode != null;

        private bool IsHoveringReroute => hoveredReroute.port != null;

        private INode hoveredNode;
        
        [NonSerialized] private INodePort hoveredPort;
        [NonSerialized] private INodePort draggedOutput;
        [NonSerialized] private INodePort draggedOutputTarget;
        [NonSerialized] private List<Vector2> draggedOutputReroutes = new List<Vector2>();
        
        private RerouteReference hoveredReroute;
        private List<RerouteReference> selectedReroutes = new List<RerouteReference>();
        private Rect nodeRects;
        private Vector2 dragBoxStart;
        private int[] preBoxSelection;
        private RerouteReference[] preBoxSelectionReroute;
        private Rect selectionBox;

        private struct RerouteReference
        {
            public INodePort port;
            public int connectionIndex;
            public int pointIndex;

            public RerouteReference(INodePort port, int connectionIndex, int pointIndex)
            {
                this.port = port;
                this.connectionIndex = connectionIndex;
                this.pointIndex = pointIndex;
            }

            public void InsertPoint(Vector2 pos)
            {
                port.GetReroutePoints(connectionIndex).Insert(pointIndex, pos);
            }

            public void SetPoint(Vector2 pos)
            {
                port.GetReroutePoints(connectionIndex)[pointIndex] = pos;
            }

            public void RemovePoint()
            {
                port.GetReroutePoints(connectionIndex).RemoveAt(pointIndex);
            }

            public Vector2 GetPoint()
            {
                return port.GetReroutePoints(connectionIndex)[pointIndex];
            }
        }

        public void Controls()
        {
            wantsMouseMove = true;
            var e = Event.current;
            switch (e.type)
            {
                case EventType.MouseMove:
                    break;
                case EventType.ScrollWheel:
                    if (e.delta.y > 0) Zoom += 0.1f * Zoom;
                    else Zoom -= 0.1f * Zoom;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (IsDraggingPort)
                        {
                            if (IsHoveringPort && hoveredPort.Direction == PortIO.Input)
                            {
                                if (!draggedOutput.IsConnectedTo(hoveredPort))
                                {
                                    draggedOutputTarget = hoveredPort;
                                }
                            }
                            else
                            {
                                draggedOutputTarget = null;
                            }

                            Repaint();
                        }
                        else if (currentActivity == NodeActivity.HoldNode)
                        {
                            RecalculateDragOffsets(e);
                            currentActivity = NodeActivity.DragNode;
                            Repaint();
                        }

                        if (currentActivity == NodeActivity.DragNode)
                        {
                            // Holding ctrl inverts grid snap
                            var gridSnap = this.GetSettings().gridSnap;
                            if (e.control) gridSnap = !gridSnap;

                            var mousePos = WindowToGridPosition(e.mousePosition);
                            // Move selected nodes with offset
                            for (var i = 0; i < selection.Count; i++) {
                                var node    = selection[i];
                                var initial = node.Position;
                                node.Position = mousePos + dragOffset[i];
                                if (gridSnap) {
                                    var newPosition = new Vector2(
                                        (Mathf.Round((node.Position.x + 8) / 16) * 16) - 8,
                                        (Mathf.Round((node.Position.y + 8) / 16) * 16) - 8);
                                    node.Position = newPosition;
                                }

                                // Offset portConnectionPoints instantly if a node is dragged so they aren't delayed by a frame.
                                var offset = node.Position - initial;
                                if (!(offset.sqrMagnitude > 0)) {
                                    continue;
                                }

                                // foreach (var output in node.Outputs) {
                                //     if (PortConnectionPoints.TryGetValue(output, out var rect)) {
                                //         rect.position                   += offset;
                                //         PortConnectionPoints[output] =  rect;
                                //     }
                                // }
                                //
                                // foreach (var input in node.Inputs) {
                                //     if (!PortConnectionPoints.TryGetValue(input, out var rect)) {
                                //         continue;
                                //     }
                                //
                                //     rect.position                  += offset;
                                //     PortConnectionPoints[input] =  rect;
                                // }
                            }

                            // Move selected reroutes with offset
                            for (var i = 0; i < selectedReroutes.Count; i++)
                            {
                                var pos = mousePos + dragOffset[selection.Count + i];
                                if (gridSnap)
                                {
                                    pos.x = (Mathf.Round(pos.x / 16) * 16);
                                    pos.y = (Mathf.Round(pos.y / 16) * 16);
                                }

                                selectedReroutes[i].SetPoint(pos);
                            }

                            Repaint();
                        }
                        else if (currentActivity == NodeActivity.HoldGrid)
                        {
                            currentActivity = NodeActivity.DragGrid;
                            preBoxSelection = selection.
                                Select(x => x.Id).
                                ToArray();
                            
                            preBoxSelectionReroute = selectedReroutes.ToArray();
                            dragBoxStart = WindowToGridPosition(e.mousePosition);
                            Repaint();
                        }
                        else if (currentActivity == NodeActivity.DragGrid)
                        {
                            var boxStartPos = GridToWindowPosition(dragBoxStart);
                            var boxSize = e.mousePosition - boxStartPos;
                            if (boxSize.x < 0)
                            {
                                boxStartPos.x += boxSize.x;
                                boxSize.x = Mathf.Abs(boxSize.x);
                            }

                            if (boxSize.y < 0)
                            {
                                boxStartPos.y += boxSize.y;
                                boxSize.y = Mathf.Abs(boxSize.y);
                            }

                            selectionBox = new Rect(boxStartPos, boxSize);
                            Repaint();
                        }
                    }
                    else if (e.button == 1 || e.button == 2)
                    {
                        var tempOffset = PanOffset;
                        tempOffset += e.delta * Zoom;
                        // Round value to increase crispyness of UI text
                        tempOffset.x = Mathf.Round(tempOffset.x);
                        tempOffset.y = Mathf.Round(tempOffset.y);
                        PanOffset = tempOffset;
                        isPanning = true;
                    }

                    break;
                case EventType.MouseDown:
                    Repaint();
                    if (e.button == 0)
                    {
                        draggedOutputReroutes.Clear();

                        if (IsHoveringPort)
                        {
                            if (hoveredPort.IsOutput)
                            {
                                draggedOutput = hoveredPort;
                            }
                            else
                            {
                                hoveredPort.VerifyConnections();
                                if (hoveredPort.IsConnected)
                                {
                                    var node = hoveredPort.Node;
                                    var output = hoveredPort.Connection as NodePort;
                                    var outputConnectionIndex = output.GetConnectionIndex(hoveredPort);
                                    draggedOutputReroutes = output.GetReroutePoints(outputConnectionIndex);
                                    hoveredPort.Disconnect(output);
                                    draggedOutput = output;
                                    draggedOutputTarget = hoveredPort;
                                    if (NodeEditor.OnUpdateNode != null) NodeEditor.OnUpdateNode(node);
                                }
                            }
                        }
                        else if (IsHoveringNode && IsHoveringTitle(hoveredNode))
                        {
                            // If mousedown on node header, select or deselect
                            if (!IsSelected(hoveredNode))
                            {
                                Select(hoveredNode, e.control || e.shift);
                                if (!e.control && !e.shift) {
                                    selectedReroutes.Clear();
                                }
                            }
                            else if (e.control || e.shift) {
                                Deselect(hoveredNode);
                            }

                            e.Use();
                            currentActivity = NodeActivity.HoldNode;
                        }
                        else if (IsHoveringReroute)
                        {
                            // If reroute isn't selected
                            if (!selectedReroutes.Contains(hoveredReroute))
                            {
                                // Add it
                                if (e.control || e.shift) selectedReroutes.Add(hoveredReroute);
                                // Select it
                                else
                                {
                                    selectedReroutes = new List<RerouteReference> {hoveredReroute};
                                    DeselectAll();
                                }
                            }
                            // Deselect
                            else if (e.control || e.shift) selectedReroutes.Remove(hoveredReroute);

                            e.Use();
                            currentActivity = NodeActivity.HoldNode;
                        }
                        // If mousedown on grid background, deselect all
                        else if (!IsHoveringNode)
                        {
                            currentActivity = NodeActivity.HoldGrid;
                            if (!e.control && !e.shift)
                            {
                                selectedReroutes.Clear();
                                DeselectAll();
                            }
                        }
                    }

                    break;
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        //Port drag release
                        if (IsDraggingPort)
                        {
                            //If connection is valid, save it
                            if (draggedOutputTarget != null)
                            {
                                var node = draggedOutputTarget.Node;
                                if (ActiveGraph.Nodes.Count != 0) draggedOutput.Connect(draggedOutputTarget);

                                // ConnectionIndex can be -1 if the connection is removed instantly after creation
                                var connectionIndex = draggedOutput.GetConnectionIndex(draggedOutputTarget);
                                if (connectionIndex != -1)
                                {
                                    draggedOutput.GetReroutePoints(connectionIndex).AddRange(draggedOutputReroutes);
                                    if (NodeEditor.OnUpdateNode != null) NodeEditor.OnUpdateNode(node);
                                    EditorUtility.SetDirty(ActiveGraph);
                                }
                            }

                            //Release dragged connection
                            draggedOutput = null;
                            draggedOutputTarget = null;
                            EditorUtility.SetDirty(ActiveGraph);
                            if (this.GetSettings().autoSave) AssetDatabase.SaveAssets();
                        }
                        else if (currentActivity == NodeActivity.DragNode) {
                            var nodes = selection;
                            foreach (var node in nodes) 
                                node.SetDirty();
                            if (this.GetSettings().autoSave) 
                                AssetDatabase.SaveAssets();
                        }
                        else if (!IsHoveringNode)
                        {
                            // If click outside node, release field focus
                            if (!isPanning)
                            {
                                EditorGUI.FocusTextInControl(null);
                            }

                            if (this.GetSettings().autoSave) AssetDatabase.SaveAssets();
                        }

                        // If click node header, select it.
                        if (currentActivity == NodeActivity.HoldNode && !(e.control || e.shift))
                        {
                            selectedReroutes.Clear();
                            Select(hoveredNode, false);
                        }

                        // If click reroute, select it.
                        if (IsHoveringReroute && !(e.control || e.shift))
                        {
                            selectedReroutes = new List<RerouteReference> {hoveredReroute};
                        }

                        Repaint();
                        currentActivity = NodeActivity.Idle;
                    }
                    else if (e.button == 1 || e.button == 2)
                    {
                        if (!isPanning)
                        {
                            if (IsDraggingPort)
                            {
                                draggedOutputReroutes.Add(WindowToGridPosition(e.mousePosition));
                            }
                            else if (currentActivity == NodeActivity.DragNode && 
                                     selection.Count > 0 &&
                                     selectedReroutes.Count == 1)
                            {
                                selectedReroutes[0].InsertPoint(selectedReroutes[0].GetPoint());
                                selectedReroutes[0] = new RerouteReference(selectedReroutes[0].port,
                                    selectedReroutes[0].connectionIndex, selectedReroutes[0].pointIndex + 1);
                            }
                            else if (IsHoveringReroute)
                            {
                                ShowRerouteContextMenu(hoveredReroute);
                            }
                            else if (IsHoveringPort)
                            {
                                ShowPortContextMenu(hoveredPort);
                            }
                            else if (IsHoveringNode && IsHoveringTitle(hoveredNode))
                            {
                                if (!hoveredNode.IsSelected()) {
                                    Select(hoveredNode, false);
                                }
                                ShowNodeContextMenu();
                            }
                            else if (!IsHoveringNode)
                            {
                                ShowGraphContextMenu();
                            }
                        }

                        isPanning = false;
                    }

                    break;
                case EventType.KeyDown:
                    if (EditorGUIUtility.editingTextField) break;
                    else if (e.keyCode == KeyCode.F) Home();
                    if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
                    {
                        if (e.keyCode == KeyCode.Return) RenameSelectedNode();
                    }
                    else
                    {
                        if (e.keyCode == KeyCode.F2) RenameSelectedNode();
                    }

                    break;
                case EventType.ValidateCommand:
                    if (e.commandName == "SoftDelete") RemoveSelectedNodes();
                    else if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX &&
                             e.commandName == "Delete") RemoveSelectedNodes();
                    else if (e.commandName == "Duplicate") DublicateSelectedNodes();
                    Repaint();
                    break;
                case EventType.Ignore:
                    // If release mouse outside window
                    if (e.rawType == EventType.MouseUp && currentActivity == NodeActivity.DragGrid)
                    {
                        Repaint();
                        currentActivity = NodeActivity.Idle;
                    }

                    break;
            }
        }

        private void RecalculateDragOffsets(Event current)
        {
            dragOffset = new Vector2[selection.Count + selectedReroutes.Count];
            // Selected nodes
            for (var i = 0; i < selection.Count; i++)
            {
                var node = selection[i];
                dragOffset[i] = node.Position - WindowToGridPosition(current.mousePosition);
            }

            // Selected reroutes
            for (var i = 0; i < selectedReroutes.Count; i++)
            {
                dragOffset[selection.Count + i] =
                    selectedReroutes[i].GetPoint() - WindowToGridPosition(current.mousePosition);
            }
        }

        /// <summary> Puts all nodes in focus. If no nodes are present, resets view to  </summary>
        public void Home()
        {
            Zoom = 2;
            PanOffset = Vector2.zero;
        }

        public INode CreateNode(Type type, Vector2 position)
        {
            return CreateNode(type,ObjectNames.NicifyVariableName(type.Name), position);
        }
        
        public INode CreateNode(Type type,string nodeName, Vector2 position)
        {
            var node = ActiveGraph.AddNode(type,nodeName,position);

            Save();

            return node;
        }

        /// <summary> Remove nodes in the graph in Selection.objects</summary>
        public void RemoveSelectedNodes()
        {
            // We need to delete reroutes starting at the highest point index to avoid shifting indices
            selectedReroutes = selectedReroutes.OrderByDescending(x => x.pointIndex).ToList();
            for (var i = 0; i < selectedReroutes.Count; i++)
            {
                selectedReroutes[i].RemovePoint();
            }

            selectedReroutes.Clear();
            foreach (var item in selection)
            {
                graphEditor.RemoveNode(item);
            }
        }

        /// <summary> Initiate a rename on the currently selected node </summary>
        public void RenameSelectedNode()
        {
            if (selection.Count == 1) {
                var node = selection.FirstOrDefault();
                NodeEditor.GetEditor(node).InitiateRename();
            }
        }

        /// <summary> Dublicate selected nodes and select the dublicates </summary>
        public void DublicateSelectedNodes()
        {
            
            var substitutes = new Dictionary<INode, INode>();

            var srcNodes = selection.ToList();
            
            foreach (var srcNode in srcNodes) {
                if (srcNode.GraphData != ActiveGraph) continue; // ignore nodes selected in another graph
                var newNode = graphEditor.CopyNode(srcNode);
                substitutes.Add(srcNode, newNode);
                newNode.Position = (srcNode.Position + new Vector2(30, 30));
            }

            // Walk through the selected nodes again, recreate connections, using the new nodes
            foreach (var srcNode in srcNodes) {

                if (srcNode.GraphData != ActiveGraph) continue; // ignore nodes selected in another graph
                
                foreach (var port in srcNode.Ports)
                {
                    for (var c = 0; c < port.ConnectionCount; c++)
                    {
                        var inputPort = port.Direction == PortIO.Input ? port : port.GetConnection(c);
                        var outputPort = port.Direction == PortIO.Output
                            ? port
                            : port.GetConnection(c);

                        if (substitutes.TryGetValue(inputPort.Node, out var newNodeIn) &&
                            substitutes.TryGetValue(outputPort.Node, out var  newNodeOut))
                        {
                            inputPort  = newNodeIn.GetInputPort(inputPort.ItemName);
                            outputPort = newNodeOut.GetOutputPort(outputPort.ItemName);
                        }

                        if (!inputPort.IsConnectedTo(outputPort as NodePort)) {
                            inputPort.Connect(outputPort as NodePort);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Draw a connection as we are dragging it
        /// </summary>
        public void DrawDraggedConnection()
        {
            if (IsDraggingPort)
            {
                var col = GameFlowPreferences.GetTypeColor(draggedOutput.ValueType);

                if (!_portConnectionPoints.TryGetValue(draggedOutput, out var fromRect)) return;
                var from = fromRect.center;
                col.a = 0.6f;
                var to = Vector2.zero;
                for (var i = 0; i < draggedOutputReroutes.Count; i++)
                {
                    to = draggedOutputReroutes[i];
                    DrawConnection(from, to, col);
                    from = to;
                }

                to = draggedOutputTarget!=null && 
                     PortConnectionPoints.TryGetValue(draggedOutputTarget, out var targetRect) ?
                     targetRect.center : 
                     WindowToGridPosition(Event.current.mousePosition);
  
                DrawConnection(from, to, col);

                var bgcol = Color.black;
                var frcol = col;
                bgcol.a = 0.6f;
                frcol.a = 0.6f;

                // Loop through reroute points again and draw the points
                for (var i = 0; i < draggedOutputReroutes.Count; i++)
                {
                    // Draw reroute point at position
                    var rect = new Rect(draggedOutputReroutes[i], new Vector2(16, 16));
                    rect.position = new Vector2(rect.position.x - 8, rect.position.y - 8);
                    rect = GridToWindowRect(rect);

                    NodeEditorGUILayout.DrawPortHandle(rect, bgcol, frcol);
                }
            }
        }

        bool IsHoveringTitle(INode node)
        {
            var mousePos = Event.current.mousePosition;
            //Get node position
            var nodePos = GridToWindowPosition(node.Position);
            float width;
            if (NodeSizes.TryGetValue(node, out var size)) width = size.x;
            else width = 200;
            var windowRect = new Rect(nodePos, new Vector2(width / Zoom, 30 / Zoom));
            return windowRect.Contains(mousePos);
        }
    }
}