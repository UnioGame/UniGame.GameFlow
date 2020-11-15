namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Tools.PortData
{
    using Core.Runtime.Interfaces;

    public interface ISerializableEditorValue<TValue> : ISerializableObject
    {
        TValue Value { get; }

        void Apply(object value);
    }
}