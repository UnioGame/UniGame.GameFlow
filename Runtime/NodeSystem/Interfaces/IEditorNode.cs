namespace UniModules.GameFlow.Runtime.Interfaces
{
    using UnityEngine;

    public interface IEditorNode
    {
        
        #region editor api

        int SetId(int id);
        
        /// <summary>
        /// set up graph node position
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// setup node view width
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// editor ui tollkit style
        /// </summary>
        /// <returns></returns>
        string GetStyle();

        #endregion

    }
}