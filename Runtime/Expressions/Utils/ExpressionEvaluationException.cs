namespace Unibrics.Configuration.Expressions.Utils
{
    using System;

    public class ExpressionEvaluationException : Exception
    {
        public ExpressionEvaluationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ExpressionException : Exception
    {
        public ExpressionException(string message) : base(message)
        {
        }

        public override string ToString()
        {
            return $"ExpressionException : {Message}";
        }
    }
}