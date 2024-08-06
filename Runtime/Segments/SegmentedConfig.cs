namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;
    using System.Linq;

    public class SegmentedConfig
    {
        public string DefaultValue { get; private set; }

        private readonly Dictionary<string, string> segments = new();

        public bool HasSegments => segments.Any();

        public List<string> PossibleSegments => segments.Keys.ToList();
        
        public void AddDefaultValue(string value)
        {
            DefaultValue = value;
        }

        public void AddSegmentValue(string segment, string value)
        {
            segments[segment] = value;
        }

        public string GetSegmentValue(string segment) => segments[segment];
    }
}