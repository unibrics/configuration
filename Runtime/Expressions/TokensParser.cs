namespace Unibrics.Configuration.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Tokens;
    using Types;

    interface ITokensParser
    {
        Expression Parse(List<Token> tokens);
    }
    
    class TokensParser : ITokensParser
    {
        private List<Token> tokens;
        private int position;

        public Expression Parse(List<Token> tokens)
        {
            this.tokens = tokens;
            position = 0;

            return ParseExpression();
        }

        private Expression ParseExpression()
        {
            return ParseOr();
        }

        private Expression ParseOr()
        {
            var left = ParseAnd();

            while (Match(TokenType.LogicalOperator) && Current().Value == "or")
            {
                var op = Advance();
                var right = ParseAnd();
                left = new BinaryExpression(left, op.Value, right);
            }

            return left;
        }

        private Expression ParseAnd()
        {
            var left = ParseComparison();

            while (Match(TokenType.LogicalOperator) && Current().Value == "and")
            {
                var op = Advance();
                var right = ParseComparison();
                left = new BinaryExpression(left, op.Value, right);
            }

            return left;
        }

        private Expression ParseComparison()
        {
            var left = ParsePrimary();

            while (Match(TokenType.Comparison))
            {
                var op = Advance();
                var right = ParsePrimary();
                left = new BinaryExpression(left, op.Value, right);
            }

            return left;
        }

        private Expression ParsePrimary()
        {
            if (Match(TokenType.Number))
            {
                return new LiteralExpression(double.Parse(Advance().Value, CultureInfo.InvariantCulture));
            }
            
            if (Match(TokenType.String))
            {
                return new LiteralExpression(Advance().Value);
            }
            
            if (Match(TokenType.String) || Match(TokenType.Set))
            {
                return new LiteralExpression(Advance().Value);
            }

            if (Match(TokenType.Variable))
            {
                return new VariableExpression(Advance().Value);
            }

            if (Match(TokenType.OpenParen))
            {
                Advance();
                var expr = ParseExpression();
                Consume(TokenType.CloseParen, "Expect ')' after expression.");
                return expr;
            }

            if (Match(TokenType.LogicalOperator) && Current().Value == "not")
            {
                var op = Advance();
                var operand = ParsePrimary();
                return new UnaryExpression(op.Value, operand);
            }

            throw new Exception("Unexpected token.");
        }

        private bool Match(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }
            return Current().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                position++;
            }
            return Previous();
        }

        private Token Previous()
        {
            return tokens[position - 1];
        }

        private void Consume(TokenType type, string message)
        {
            if (Current().Type != type)
            {
                throw new Exception(message);
            }
            Advance();
        }

        private Token Current()
        {
            if (IsAtEnd())
            {
                return new Token(TokenType.Variable, ""); // Return a dummy token or handle as needed
            }
            return tokens[position];
        }
        
        private bool IsAtEnd()
        {
            return position >= tokens.Count;
        }
    }
}