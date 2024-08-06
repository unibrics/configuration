namespace Unibrics.Configuration.Expressions.Types
{

    using System.Collections.Generic;

    public class LiteralExpression : Expression
    {
        public object Value { get; }

        public LiteralExpression(object value)
        {
            Value = value;
        }

        public override ExpressionResult Evaluate(Dictionary<string, object> variables)
        {
            return new ExpressionResult(Value);
        }
    }
}