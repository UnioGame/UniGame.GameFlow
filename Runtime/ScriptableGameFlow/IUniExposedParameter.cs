namespace UniGame.GameFlowEditor.Runtime
{
    public interface IUniExposedParameter
    {
        string DisplayName { get; set; }
        
        string Info { get; }

        void Apply(UniGraphAsset asset);
    }


    public static class UniExposedParametersTool
    {
        public static string GetNiceNameFromType(string type)
        {
            if(string.IsNullOrEmpty(type))
                return string.Empty;
            
            var name = type;

            // Remove parameter in the name of the type if it exists
            name = name.Replace("Parameter", "");
            name = name.Replace("Node", "");
            name = name.Replace("Output", "");

            return name;
        }

    }
}