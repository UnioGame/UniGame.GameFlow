using UniGame.Core.Runtime.Rx;

namespace UniModules.GameFlow.Editor.Tools.PortData
{
    using System;
    using System.Collections.Generic;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UnityEngine;

    [Serializable]
    public class PortViewerEditor
    {
        #region inspector

        public string portName = string.Empty;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.ListDrawerSettings(Expanded = true,DraggableItems = false)]
#endif
        [SerializeField]
        public List<ContextValueInfo> values = new List<ContextValueInfo>();
        
        #endregion
        
        private INodePort    _port;
        private List<string> _genericsNames = new List<string>();

        public PortViewerEditor Initialize(INodePort port)
        {
            _port    = port;
            portName = port.ItemName;
            
            UpdateValues(_port.Value as PortValue);
            return this;
        }

        private void UpdateValues(PortValue source)
        {
            values.Clear();
            if (source == null)
                return;
            
            foreach (var valuePair in source.Values)
            {
                _genericsNames.Clear();
                var type     = valuePair.Key;
                var value    = valuePair.Value;

                var containerValue = value as IReadonlyObjectValue;
                var resultValue    = containerValue != null ? containerValue.GetValue() : value;
                

                values.Add(new ContextValueInfo().Update(resultValue,type));
            }
        }
        
    }
}