namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;

    public class SegmentedConfigsResolver : IConfigValueResolver
    {
        private readonly IDictionary<string, SegmentedConfig> values = new Dictionary<string, SegmentedConfig>();
        
        private const string SegmentDelimiter = "__";

        public void PutValue(string key, string value)
        {
            var configKey = key;
            string segment = null;
            if (key.Contains(SegmentDelimiter))
            {
                var split = key.Split(SegmentDelimiter);
                configKey = split[0];
                segment = split[1];
            }
            
            if (!values.TryGetValue(key, out var segmentedConfig))
            {
                segmentedConfig = new SegmentedConfig();
                values[configKey] = segmentedConfig;
            }

            if (segment != null)
            {
                segmentedConfig.AddSegmentValue(segment, value);
            }
            else
            {
                segmentedConfig.AddDefaultValue(value);
            }
        }

        public string GetValue(string key)
        {
            return values[key].DefaultValue;
        }

        public IEnumerable<string> GetKeys() => values.Keys;
    }
}