namespace UniGame.GameFlowEditor.Runtime
{
    public interface IUniExposedParameter
    {
        string DisplayName { get; set; }
        
        string Info { get; }

        void Apply(UniGraphAsset asset);
    }
}