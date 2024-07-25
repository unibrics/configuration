namespace Unibrics.Configuration.General
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Config;
    using Expressions.API;
    using Logs;

    class SegmentsSelector : ISegmentsSelector, IActiveSegmentsProvider
    {
        private readonly IExpressionEvaluator evaluator;

        private readonly ISegmentsConfig config;

        private readonly ISegmentExpressionVariablesProvider variablesProvider;

        private readonly Dictionary<string, bool> cachedEvaluations = new();

        public SegmentsSelector(IExpressionEvaluator evaluator, ISegmentsConfig config, ISegmentExpressionVariablesProvider variablesProvider)
        {
            this.evaluator = evaluator;
            this.config = config;
            this.variablesProvider = variablesProvider;
        }

        public string GetActiveSegment(List<string> variants)
        {
            var orderedSegments = variants
                .Select(segment => config.GetSegmentExpression(segment))
                .OrderBy(description => description.Order);

            foreach (var segment in orderedSegments)
            {
                if (!segment.IsDefined)
                {
                    Log($"Segment '{segment.Name}' is unknown");
                    continue;
                }

                if (cachedEvaluations.TryGetValue(segment.Name, out var cachedResult))
                {
                    Log($"Segment '{segment.Name}' is taken from cache - {cachedResult}");
                    if (cachedResult)
                    {
                        return segment.Name;
                    }
                }
                
                var result = evaluator.Evaluate(segment.Value, variablesProvider.GetVariables());
                if (result.HasErrors)
                {
                    Log($"Evaluating segment '{segment.Name}' ('{segment.Value}') ends with errors, skipping.");
                    Log(result.Exception.ToString());
                    continue;
                }
                
                Log($"Segment '{segment.Name}' evaluated successfully {result.Result}");
                if (result.Result.HasValue)
                {
                    cachedEvaluations[segment.Name] = result.Result.Value;
                    if (result.Result.Value)
                    {
                        return segment.Name;
                    }
                }
            }

            return null;
        }

        public IEnumerable<(string segment, string description)> GetAllActiveSegments()
        {
            foreach (var pair in config.GetAllSegments())
            {
                var result = evaluator.Evaluate(pair.expression, variablesProvider.GetVariables());
                if (result is { HasErrors: false, Result: true })
                {
                    yield return pair;
                }
            }
        }

        private void Log(string message)
        {
            Logger.Log("Config", message);
        }
    }
}