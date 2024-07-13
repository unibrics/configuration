namespace Unibrics.Configuration.Expressions.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class BinaryExpression : Expression
{
    public Expression Left { get; }
    public string Operator { get; }
    public Expression Right { get; }

    public BinaryExpression(Expression left, string op, Expression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override ExpressionResult Evaluate(Dictionary<string, object> variables)
    {
        var leftValue = Left.Evaluate(variables);
        var rightValue = Right.Evaluate(variables);
        var result = Operator switch
        {
            "and" => leftValue.AsBool && rightValue.AsBool,
            "or" => leftValue.AsBool || rightValue.AsBool,
            "is" => Regex.IsMatch(leftValue.AsString, rightValue.AsString),
            "in" => rightValue.AsSet.Contains(leftValue.RawValue),
            "notin" => !rightValue.AsSet.Contains(leftValue.RawValue),
            "isnot" => !Regex.IsMatch(leftValue.AsString, rightValue.AsString),
            "<" => Left.Evaluate(variables).AsDouble < Right.Evaluate(variables).AsDouble,
            "<=" => Left.Evaluate(variables).AsDouble <= Right.Evaluate(variables).AsDouble,
            ">" => Left.Evaluate(variables).AsDouble > Right.Evaluate(variables).AsDouble,
            ">=" => Left.Evaluate(variables).AsDouble >= Right.Evaluate(variables).AsDouble,
            "==" => Left.Evaluate(variables).Equals(Right.Evaluate(variables)),
            "!=" => !Left.Evaluate(variables).Equals(Right.Evaluate(variables)),
            _ => throw new Exception($"Unknown operator: {Operator}")
        };
        return new ExpressionResult(result);
    }
}

}