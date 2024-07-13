namespace Unibrics.Configuration.Expressions.API
{
    using System;
    using System.Collections.Generic;

    public interface IExpressionEvaluator
    {
        EvaluationResult Evaluate(string expression, Dictionary<string, object> variables);
    }

    public struct EvaluationResult
    {
        public bool HasErrors => Exception != null;
        
        public Exception Exception { get; set; }
        
        public bool? Result { get; set; }
    }
}