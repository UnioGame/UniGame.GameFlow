namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using UniCore.Runtime.Interfaces;

    public interface IGraphItem : IUnique, INamedItem
    {
        void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem);
    }
}