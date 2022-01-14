using System.Linq;
using UniGame.GameFlowEditor.Runtime;
using UniModules.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniModules.UniGame.GameFlow.Editor.UiElementsEditor.Tools.ExposedParameterElement
{
    using System;
    using System.Collections.Generic;
    using GraphProcessor;
    
    public class UniExposedParameterView : PinnedElementView
    {
        protected BaseGraphView	graphView;
        protected UniGraphAsset graphAsset;

		new const string title = "Parameters";
        
        readonly string exposedParameterViewStyle = "GraphProcessorStyles/ExposedParameterView";

        List<Rect> blackboardLayouts = new List<Rect>();

        public UniExposedParameterView()
        {
            var style = Resources.Load<StyleSheet>(exposedParameterViewStyle);
            if (style != null)
                styleSheets.Add(style);
        }

        protected virtual void OnAddClicked()
        {
            var parameterType = new GenericMenu();

            foreach (var paramType in GetExposedParameterTypes())
                parameterType.AddItem(new GUIContent(UniExposedParametersTool.GetNiceNameFromType(paramType.Name)), false, () =>
                {
                    try
                    {
                        var parameter = Activator.CreateInstance(paramType) as IUniExposedParameter;
                        if (parameter == null)
                            return;
                        
                        graphAsset.uniExposedParameters.Add(parameter);
                        graphAsset.MarkDirty();

                        UpdateParameterList();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });

            parameterType.ShowAsContext();
        }

        protected virtual IEnumerable< Type > GetExposedParameterTypes()
        {
            foreach (var type in TypeCache.GetTypesDerivedFrom<IUniExposedParameter>())
            {
                if (type.IsGenericType || type.IsAbstract)
                    continue;

                yield return type;
            }
        }

        protected virtual void UpdateParameterList()
        {
            content.Clear();
            
            foreach (var param in graphAsset.uniExposedParameters)
            {
                var fieldView = new UniExposedParameterFieldView(graphAsset, param,Remove);
                var propertyView = new UniExposedParameterPropertyView(graphView, param);
                
                var row = new BlackboardRow(fieldView, propertyView);
                content.Add(row);
            }
        }

        protected void Remove(IUniExposedParameter parameter)
        {
            graphAsset.uniExposedParameters.Remove(parameter);
            UpdateParameterList();
        }
        
        protected override void Initialize(BaseGraphView graphView)
        {
			this.graphView = graphView;
            this.graphAsset = graphView.graph as UniGraphAsset;

            base.title = title;
			scrollable = true;

            graphView.onExposedParameterListChanged += UpdateParameterList;
            graphView.initialized += UpdateParameterList;
            Undo.undoRedoPerformed += UpdateParameterList;

            RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent, TrickleDown.TrickleDown);
            RegisterCallback<DetachFromPanelEvent>(OnViewClosed);

            UpdateParameterList();

            // Add exposed parameter button
            header.Add(new Button(OnAddClicked){ text = "+" });
        }

        void OnViewClosed(DetachFromPanelEvent evt)
            => Undo.undoRedoPerformed -= UpdateParameterList;

        void OnMouseDownEvent(MouseDownEvent evt)
        {
            blackboardLayouts = content.Children().Select(c => c.layout).ToList();
        }

        int GetInsertIndexFromMousePosition(Vector2 pos)
        {
            pos = content.WorldToLocal(pos);
            // We only need to look for y axis;
            var mousePos = pos.y;

            if (mousePos < 0)
                return 0;

            var index = 0;
            foreach (var layout in blackboardLayouts)
            {
                if (mousePos > layout.yMin && mousePos < layout.yMax)
                    return index + 1;
                index++;
            }

            return content.childCount;
        }

        void OnDragUpdatedEvent(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            var newIndex = GetInsertIndexFromMousePosition(evt.mousePosition);
            var graphSelectionDragData = DragAndDrop.GetGenericData("DragSelection");

            if (graphSelectionDragData == null)
                return;

            foreach (var obj in graphSelectionDragData as List<ISelectable>)
            {
                if (!(obj is UniExposedParameterFieldView view)) continue;
                
                var blackBoardRow = view.parent.parent.parent.parent.parent.parent;
                var oldIndex = content.Children().ToList().FindIndex(c => c == blackBoardRow);
                // Try to find the blackboard row
                content.Remove(blackBoardRow);

                if (newIndex > oldIndex)
                    newIndex--;

                content.Insert(newIndex, blackBoardRow);
            }
        }

        void OnDragPerformEvent(DragPerformEvent evt)
        {
            var updateList = false;

            var newIndex = GetInsertIndexFromMousePosition(evt.mousePosition);
            foreach (var obj in (List<ISelectable>)DragAndDrop.GetGenericData("DragSelection"))
            {
                if (!(obj is UniExposedParameterFieldView view)) continue;
                
                if (!updateList)
                    graphView.RegisterCompleteObjectUndo("Moved parameters");

                var parameters = graphAsset.uniExposedParameters;
                var oldIndex = parameters.FindIndex(e => e == view.parameter);
                var parameter = parameters[oldIndex];
                parameters.RemoveAt(oldIndex);

                // Patch new index after the remove operation:
                if (newIndex > oldIndex)
                    newIndex--;

                parameter.Apply(graphAsset);
                
                updateList = true;
            }

            if (!updateList) return;
            
            evt.StopImmediatePropagation();
            UpdateParameterList();
        }
    }
}
