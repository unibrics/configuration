namespace Unibrics.Configuration.Expressions.Types
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnaryExpression : Expression
    {
        public string Operator { get; }
        public Expression Operand { get; }

        public UnaryExpression(string op, Expression operand)
        {
            Operator = op;
            Operand = operand;
        }

        public override ExpressionResult Evaluate(Dictionary<string, object> variables)
        {
            Debug.Log($"Evaluating Unary");
            var value = Operand.Evaluate(variables);

            var result = Operator switch
            {
                "not" => !value.AsBool,
                _ => throw new Exception($"Unknown operator: {Operator}")
            };

            return new ExpressionResult(result);
        }
    }
}