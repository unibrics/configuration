namespace Unibrics.Configuration.Expressions
{
    using System.Collections.Generic;

    public abstract class Expression
    {
        public abstract ExpressionResult Evaluate(Dictionary<string, object> variables);
    }

}