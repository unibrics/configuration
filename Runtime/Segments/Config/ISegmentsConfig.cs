namespace Unibrics.Configuration.General.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;

    [Config("universal_segments", typeof(SegmentsConfig), Priority = Priority.Highest)]
    public interface ISegmentsConfig
    {
        SegmentDescription GetSegmentExpression(string segment);

        IEnumerable<(string segment, string expression)> GetAllSegments();
    }

    public class SegmentsConfig : ConfigFile, ISegmentsConfig
    {
        public Dictionary<string, string> Segments { get; set; }

        private List<string> keys;

        public SegmentDescription GetSegmentExpression(string segment)
        {
            if (keys == null)
            {
                keys = Segments.Keys.ToList();
            }

            if (!keys.Contains(segment))
            {
                return new SegmentDescription()
                {
                    Name = segment
                }; // empty segment
            }

            return new SegmentDescription
            {
                Order = keys.IndexOf(segment),
                Name = segment,
                Value = Segments[segment]
            };
        }

        public IEnumerable<(string segment, string expression)> GetAllSegments()
        {
            foreach (var pair in Segments)
            {
                yield return (pair.Key, pair.Value);
            }
        }
    }

    public struct SegmentDescription
    {
        public int Order { get; set; }
        
        public string Name { get; set; }
        
        public string Value { get; set; }

        public bool IsDefined => Value != null;
    }
}