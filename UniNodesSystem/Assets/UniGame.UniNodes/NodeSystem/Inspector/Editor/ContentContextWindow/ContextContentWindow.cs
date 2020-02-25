namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.ContentContextWindow
{
    using UniGreenModules.UniCore.Runtime.Common;
    using UniGreenModules.UniCore.Runtime.Interfaces.Rx;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ContextContentWindow : EditorWindow
    {
        private const string stylePath = "";
        private const string uiViewPath = "";
        private const string uiIconViewPath = "";
        
        private TypeData data;
        
        private string portName;
        
        private ScrollView scrollView;
        private Button refreshButton;
        
        public static void Open(TypeData data)
        {
            var window = GetWindow<ContextContentWindow>(); 
            window.Initialize(data);
            window.minSize = new Vector2(400, 200);
            window.titleContent = new GUIContent("Context Data");
            window.Show();
        }

        public void Initialize(TypeData contextData)
        {
            this.data = contextData;
            Refresh();
        }

        public void Refresh()
        {
            CreateContent(scrollView,data);
        }
        
        public void OnEnable()
        {
           rootVisualElement.style.flexDirection = FlexDirection.Column;
           
           refreshButton = new Button(Refresh) {
               text = "refresh"
           };
           rootVisualElement.Add(refreshButton);
           
           scrollView = new ScrollView() {
               style = {
                   marginTop = 20
               },
               showVertical = true,
           };
           rootVisualElement.Add(scrollView);
           
        }

        public void CreateContent(VisualElement container , TypeData data)
        {
            container.Clear();
            
            if (data == null) return;
            
            foreach (var pair in data.EditorValues) {
                var valueContainer = pair.Value;
                var objectValue = valueContainer as IObjectValue;
                var type = pair.Key;
                
                if(valueContainer.HasValue == false || objectValue == null)
                    continue;
                
                var value = objectValue.GetValue();
                
                var foldout = new Foldout() {
                    text = $"{type.Name} : {value?.GetType().Name}",
                };
                                         
                container.Add(foldout);

                if(value == null) 
                    continue;

                var valueType = value.GetType();
                
                VisualElement element = null;
                
                switch (value) {
                    case GameObject asset:
                        element = new IMGUIContainer(() => asset.DrawOdinPropertyInspector());
                        break;
                    case Sprite asset:
                        element = new Image() {
                            image = asset.texture,
                            name = asset.name,
                            scaleMode = ScaleMode.ScaleToFit,
                            style = {
                                width  = 128,
                                height = 128,
                            }
                        };
                        
                        break;
                    case Texture asset:
                        element = new Image() {
                            image = asset,
                            name = asset.name,
                            scaleMode = ScaleMode.ScaleToFit,
                            style = {
                                width = 128,
                                height = 128,
                            }
                        };
                        break;
                    case Object asset:
                        element = new IMGUIContainer(() => asset.DrawOdinPropertyInspector());
                        break;
                    default:
                        element = new TextElement() {
                            text = value.ToString()
                        };
                        break;
                }
                
                if (element != null) {
                    foldout.Add(element);
                }
  
            }
        }
        

    }
}
