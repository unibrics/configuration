namespace Unibrics.Configuration.Expressions.Types
{
    using System;
    using System.Collections.Generic;

    public class VariableExpression : Expression
    {
        public string Name { get; }

        public VariableExpression(string name)
        {
            Name = name;
        }

        public override ExpressionResult Evaluate(Dictionary<string, object> variables)
        {
            if (!variables.TryGetValue(Name, out var variable))
            {
                throw new Exception($"Unknown variable: {Name}");
            }

            return new ExpressionResult(variable);
        }
    }
}