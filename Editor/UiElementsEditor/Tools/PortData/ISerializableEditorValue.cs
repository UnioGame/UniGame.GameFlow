using UniModules.UniGame.Core.Runtime.Interfaces;

namespace UniModules.GameFlow.Editor.Tools.PortData
{
    public interface ISerializableEditorValue<TValue> : ISerializableObject
    {
        TValue Value { get; }

        void Apply(object value);
    }
}