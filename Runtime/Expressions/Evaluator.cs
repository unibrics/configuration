namespace Unibrics.Configuration.Expressions
{
    using System.Collections.Generic;

    public class Evaluator
    {
        public bool Evaluate(Expression expression, Dictionary<string, object> variables)
        {
            var expressionResult = expression.Evaluate(variables);
            return expressionResult.AsBool;
        }
    }
}