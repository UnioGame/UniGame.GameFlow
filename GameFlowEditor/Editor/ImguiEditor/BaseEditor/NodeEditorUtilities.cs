namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Runtime.Attributes;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ReflectionUtils;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary> A set of editor-only utilities and extensions for UnityNodeEditorBase </summary>
    public static class NodeEditorUtilities
    {
        /// <summary>C#'s Script Icon [The one MonoBhevaiour Scripts have].</summary>
        private static Texture2D scriptIcon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
        
        [NonSerialized] private static List<Type> _nodeTypes = null;
        
        public static List<Type> NodeTypes
        {
            get => _nodeTypes = (_nodeTypes == null || _nodeTypes.Count == 0) ?
                NodeEditorUtilities.GetVisibleNodeTypes() :
                _nodeTypes;
        }
        
        public static List<Type> GetNodeTypes()
        {
            //Get all classes deriving from Node via reflection
            return typeof(INode).GetAssignableTypes();
        }

        public static List<Type> GetVisibleNodeTypes()
        {
            var nodeTypes = GetNodeTypes();
            nodeTypes.RemoveAll(x => !x.IsVisible);
            //Get all classes deriving from Node via reflection
            return nodeTypes;
        }

        public static bool IsValidNode(this Type nodeType)
        {
            return !NodeEditorUtilities.GetAttrib<HideNodeAttribute>(nodeType, out var hideNodeAttribute);
        }
        
        /// <summary> Returns context node menu path. Null or empty strings for hidden nodes. </summary>
        public static string GetNodeMenuName(this Type type) {
            //Check if type has the CreateNodeMenuAttribute
            CreateNodeMenuAttribute attrib;
            return NodeEditorUtilities.GetAttrib(type, out attrib) ? 
                attrib.menuName : 
                ObjectNames.NicifyVariableName(type.ToString().Replace('.', '/'));
        }
        
        public static bool GetAttrib<T>(Type classType, out T attribOut) where T : Attribute
        {
            var attribs = classType.GetCustomAttributes(typeof(T), false);
            return GetAttrib(attribs, out attribOut);
        }

        public static bool GetAttrib<T>(object[] attribs, out T attribOut) where T : Attribute
        {
            for (var i = 0; i < attribs.Length; i++) {
                if (attribs[i].GetType() != typeof(T)) continue;

                attribOut = attribs[i] as T;
                return true;
            }

            attribOut = null;
            return false;
        }

        public static bool GetAttrib<T>(Type classType, string fieldName, out T attribOut) where T : Attribute
        {
            var field = classType.GetField(fieldName);
            if (field == null) {
                attribOut = null;
                return false;
            }

            var attribs = field.GetCustomAttributes(typeof(T), false);
            return GetAttrib(attribs, out attribOut);
        }

        public static bool HasAttrib<T>(object[] attribs) where T : Attribute
        {
            for (var i = 0; i < attribs.Length; i++) {
                if (attribs[i].GetType() == typeof(T)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary> Returns true if this can be casted to <see cref="Type"/></summary>
        public static bool IsCastableTo(this Type from, Type to)
        {
            if (to.IsAssignableFrom(from)) return true;
            var methods = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(
                    m => m.ReturnType == to &&
                         (m.Name == "op_Implicit" ||
                          m.Name == "op_Explicit")
                );
            return methods.Count() > 0;
        }

        /// <summary> Return a prettiefied type name. </summary>
        public static string PrettyName(this Type type)
        {
            if (type == null) return "no filter";
            if (type == typeof(System.Object)) return "object";
            if (type == typeof(float)) return "float";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(double)) return "double";
            if (type == typeof(string)) return "string";
            if (type == typeof(bool)) return "bool";
            if (type.IsGenericType) {
                var s           = "";
                var   genericType = type.GetGenericTypeDefinition();
                s = genericType == typeof(List<>) ? "List" : type.GetGenericTypeDefinition().ToString();
                var   types  = type.GetGenericArguments();
                var stypes = new string[types.Length];
                for (var i = 0; i < types.Length; i++) {
                    stypes[i] = types[i].PrettyName();
                }

                return s + "<" + string.Join(", ", stypes) + ">";
            }
            if (type.IsArray) {
                var rank = "";
                for (var i = 1; i < type.GetArrayRank(); i++) {
                    rank += ",";
                }

                var elementType = type.GetElementType();
                if (!elementType.IsArray) return elementType.PrettyName() + "[" + rank + "]";
                {
                    var s = elementType.PrettyName();
                    var    i = s.IndexOf('[');
                    return s.Substring(0, i) + "[" + rank + "]" + s.Substring(i);
                }
            }

            return type.ToString();
        }

        /// <summary>Creates a new C# Class.</summary>
        [MenuItem("Assets/Create/UniNodeSystem/Node C# Script", false, 89)]
        private static void CreateNode()
        {
            var guids = AssetDatabase.FindAssets("Node_NodeTemplate.cs");
            if (guids.Length == 0) {
                Debug.LogWarning("UniNodeSystem_NodeTemplate.cs.txt not found in asset database");
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            CreateFromTemplate("NewNode.cs", path);
        }

        /// <summary>Creates a new C# Class.</summary>
        [MenuItem("Assets/Create/UniNodeSystem/NodeGraph C# Script", false, 89)]
        private static void CreateGraph()
        {
            var guids = AssetDatabase.FindAssets("Node_NodeGraphTemplate.cs");
            if (guids.Length == 0) {
                Debug.LogWarning("UniNodeSystem_NodeGraphTemplate.cs.txt not found in asset database");
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            CreateFromTemplate(
                "NewNodeGraph.cs",
                path
            );
        }

        public static void CreateFromTemplate(string initialName, string templatePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateCodeFile>(),
                initialName,
                scriptIcon,
                templatePath
            );
        }

        /// Inherits from EndNameAction, must override EndNameAction.Action
        public class DoCreateCodeFile : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var o = CreateScript(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }
        }

        /// <summary>Creates Script from Template's path.</summary>
        internal static Object CreateScript(string pathName, string templatePath)
        {
            var className    = Path.GetFileNameWithoutExtension(pathName).Replace(" ", string.Empty);
            var templateText = string.Empty;

            var encoding = new UTF8Encoding(true, false);

            if (File.Exists(templatePath)) {
                /// Read procedures.
                var reader = new StreamReader(templatePath);
                templateText = reader.ReadToEnd();
                reader.Close();

                templateText = templateText.Replace("#SCRIPTNAME#", className);
                templateText = templateText.Replace("#NOTRIM#", string.Empty);
                /// You can replace as many tags you make on your templates, just repeat Replace function
                /// e.g.:
                /// templateText = templateText.Replace("#NEWTAG#", "MyText");

                /// Write procedures.

                var writer = new StreamWriter(Path.GetFullPath(pathName), false, encoding);
                writer.Write(templateText);
                writer.Close();

                AssetDatabase.ImportAsset(pathName);
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            }
            else {
                Debug.LogError(string.Format("The template file was not found: {0}", templatePath));
                return null;
            }
        }
    }
}