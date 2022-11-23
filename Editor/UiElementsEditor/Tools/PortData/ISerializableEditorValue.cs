using UniGame.Core.Runtime;

namespace UniModules.GameFlow.Editor.Tools.PortData
{
    public interface ISerializableEditorValue<TValue> : ISerializableObject
    {
        TValue Value { get; }

        void Apply(object value);
    }
}