namespace Unibrics.Configuration.Expressions.Types
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Utils;

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
                throw new ExpressionException($"Unknown variable: {Name}");
            }

            if (variable is int intVariable)
            {
                // to exclude possible comparisons between different types, all numbers are converted to doubles
                return new ExpressionResult((double)intVariable);    
            }
            return new ExpressionResult(variable);
        }
    }
}