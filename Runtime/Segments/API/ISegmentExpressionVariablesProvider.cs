namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;

    public interface ISegmentExpressionVariablesProvider
    {
        Dictionary<string, object> GetVariables();
    }

    public class EmptySegmentExpressionVariablesProvider : ISegmentExpressionVariablesProvider
    {
        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>()
            {
                
            };
        }
    }
}