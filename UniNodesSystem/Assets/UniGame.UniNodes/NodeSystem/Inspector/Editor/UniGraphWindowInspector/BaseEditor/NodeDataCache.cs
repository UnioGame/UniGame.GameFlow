namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary> Precaches reflection data in editor so we won't have to do it runtime </summary>
    public static class NodeDataCache
    {
        private static PortDataCache portDataCache;

        private static bool Initialized => portDataCache != null;

        /// <summary> Update static ports to reflect class fields. </summary>
        public static void UpdatePorts(UniBaseNode node, Dictionary<string, NodePort> ports)
        {
            if (!Initialized) BuildCache();

            var staticPorts = new Dictionary<string, NodePort>();
            var nodeType    = node.GetType();

            List<NodePort> typePortCache;
            if (portDataCache.TryGetValue(nodeType, out typePortCache)) {
                for (var i = 0; i < typePortCache.Count; i++) {
                    staticPorts.Add(typePortCache[i].FieldName, portDataCache[nodeType][i]);
                }
            }

            // Cleanup port dict - Remove nonexisting static ports - update static port types
            // Loop through current node ports
            foreach (var port in ports.Values.ToList()) {
                // If port still exists, check it it has been changed
                NodePort staticPort;
                if (staticPorts.TryGetValue(port.FieldName, out staticPort)) {
                    // If port exists but with wrong settings, remove it. Re-add it later.
                    if (port.ConnectionType != staticPort.ConnectionType || port.Direction != staticPort.Direction) {
                        ports.Remove(port.FieldName);
                    }
                    else {
                        port.SetValueFilter(staticPort.ValueTypes);
                    }
                }
            }

            // Add missing ports
            foreach (var staticPort in staticPorts.Values) {
                if (!ports.ContainsKey(staticPort.FieldName)) {
                    ports.Add(staticPort.FieldName, new NodePort(staticPort, node));
                }
            }
        }

        private static void BuildCache()
        {
            portDataCache = new PortDataCache();
            var baseType     = typeof(UniBaseNode);
            var nodeTypes    = new List<System.Type>();
            var assemblies   = System.AppDomain.CurrentDomain.GetAssemblies();
            var selfAssembly = Assembly.GetAssembly(baseType);
            if (selfAssembly.FullName.StartsWith("Assembly-CSharp") && !selfAssembly.FullName.Contains("-firstpass")) {
                // If UniNodeSystem is not used as a DLL, check only CSharp (fast)
                nodeTypes.AddRange(selfAssembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)));
            }
            else {
                // Else, check all relevant DDLs (slower)
                // ignore all unity related assemblies
                foreach (var assembly in assemblies) {
                    if (assembly.FullName.StartsWith("Unity")) continue;
                    // unity created assemblies always have version 0.0.0
                    if (!assembly.FullName.Contains("Version=0.0.0")) continue;
                    nodeTypes.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray());
                }
            }

            for (var i = 0; i < nodeTypes.Count; i++) {
                CachePorts(nodeTypes[i]);
            }
        }

        private static void CachePorts(System.Type nodeType)
        {
            var fieldInfo = nodeType.GetFields();
            for (var i = 0; i < fieldInfo.Length; i++) {
                
                var field = fieldInfo[i];
                var node = field.CreatePortByAttributes();
                
                if (node!=null && !portDataCache.ContainsKey(nodeType)) 
                    portDataCache.Add(nodeType, new List<NodePort>());
                
            }
        }
    }
}