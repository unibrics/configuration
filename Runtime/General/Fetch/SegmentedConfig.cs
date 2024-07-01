namespace Unibrics.Configuration.General
{
    public class SegmentedConfig
    {
        public string DefaultValue { get; private set; }
        
        public void AddDefaultValue(string value)
        {
            DefaultValue = value;
        }

        public void AddSegmentValue(string segment, string value)
        {
            
        }
    }
}