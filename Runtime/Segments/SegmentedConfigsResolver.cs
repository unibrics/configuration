namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;

    class SegmentedConfigsResolver : IConfigValueResolver
    {
        private readonly IDictionary<string, SegmentedConfig> values = new Dictionary<string, SegmentedConfig>();

        private readonly ISegmentsSelector segmentsSelector;

        public SegmentedConfigsResolver(ISegmentsSelector segmentsSelector)
        {
            this.segmentsSelector = segmentsSelector;
        }

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

            if (!values.TryGetValue(configKey, out var segmentedConfig))
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
            var segmentedConfig = values[key];
            if (!segmentedConfig.HasSegments)
            {
                return segmentedConfig.DefaultValue;
            }

            var availableSegments = segmentedConfig.PossibleSegments;
            var selectedSegment = segmentsSelector.GetActiveSegment(availableSegments);
            if (selectedSegment == null)
            {
                return segmentedConfig.DefaultValue;
            }

            return segmentedConfig.GetSegmentValue(selectedSegment);
        }

        public IEnumerable<string> GetKeys() => values.Keys;
    }
}