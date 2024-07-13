namespace Unibrics.Configuration.Expressions
{
    using System;
    using System.Collections.Generic;
    using API;
    using Tokens;
    using Utils;

    class ExpressionEvaluator : IExpressionEvaluator
    {
        private readonly ITokenizer tokenizer;

        private readonly ITokensParser tokensParser;

        public ExpressionEvaluator(ITokenizer tokenizer, ITokensParser tokensParser)
        {
            this.tokenizer = tokenizer;
            this.tokensParser = tokensParser;
        }

        public EvaluationResult Evaluate(string expression, Dictionary<string, object> variables)
        {
            try
            {
                var tokens = tokenizer.Tokenize(expression);
                var ast = tokensParser.Parse(tokens);
                var expressionResult = ast.Evaluate(variables);
                return new EvaluationResult { Result = expressionResult.AsBool };
            }
            catch (ExpressionException e)
            {
                return new EvaluationResult { Exception = e };
            }
            catch (Exception e)
            {
                return new EvaluationResult { Exception = new ExpressionEvaluationException("Exception during expression evaluation", e) };
            }
        }
    }
}