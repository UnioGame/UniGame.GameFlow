using UniGame.Core.EditorTools.Editor.UiElements;
using UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor;
using UniGame.UniNodes.NodeSystem.Runtime.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DemoNodeViewWindow : EditorWindow
{
    private const string stylePath      = "";
    private const string uiViewPath     = "";
    private const string uiIconViewPath = "";
    private const string refreshLabel   = "refresh";

    private UniGraph             containerValue;
    private string               portName;


    private ScrollView scrollView;
    private Button     refreshButton;


    public static void Open(UniGraph data)
    {
        var window = GetWindow<DemoNodeViewWindow>();
        window.Initialize(data);
        window.Show();
    }


    public void Initialize(UniGraph graph)
    {
        containerValue = graph;
        Refresh();
    }

    public void Refresh()
    {
        CreateContent(scrollView, containerValue);
    }

    public void OnEnable()
    {
        rootVisualElement.style.flexDirection = FlexDirection.Column;

        refreshButton = new Button(Refresh) {
            text = refreshLabel
        };
        rootVisualElement.Add(refreshButton);

        scrollView = new ScrollView() {
            style = {
                marginTop       = 20,
                backgroundColor = new StyleColor(new Color(0.5f, 0.5f, 0.5f)),
            },
            showVertical = true,
        };
        rootVisualElement.Add(scrollView);
    }

    public void CreateContent(VisualElement container, UniGraph data)
    {
        container.Clear();
        if (data == null) return;

        foreach (var node in data.nodes) {
            
            //var nodeView = drawer.Create(node);
            var nodeView = new IMGUIContainer(() => node.DrawNode(node));
            
            nodeView.style.backgroundColor = new StyleColor(new Color(0.8f, 0.8f, 0.8f));
            nodeView.style.marginBottom    = 8;
            nodeView.style.borderTopColor  = new StyleColor(Color.black);
            nodeView.style.borderTopWidth  = 1;

            container.Add(nodeView);
        }
    }
}