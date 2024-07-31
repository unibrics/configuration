namespace Unibrics.Configuration.Expressions.Tokens
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    interface ITokenizer
    {
        List<Token> Tokenize(string expression);
    }
    
    class Tokenizer : ITokenizer
    {
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            {"and", TokenType.LogicalOperator},
            {"or", TokenType.LogicalOperator},
            {"not", TokenType.LogicalOperator},
            {"is", TokenType.Comparison},
            {"in", TokenType.Comparison},
            {"notin", TokenType.Comparison},
            {"isnot", TokenType.Comparison},
        };

        private static readonly Dictionary<string, TokenType> Comparisons = new()
        {
            {"<", TokenType.Comparison},
            {"<=", TokenType.Comparison},
            {">", TokenType.Comparison},
            {">=", TokenType.Comparison},
            {"==", TokenType.Comparison},
            {"!=", TokenType.Comparison}
        };

        private static readonly HashSet<char> Operators = new("()");

        public List<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            var i = 0;

            while (i < expression.Length)
            {
                if (char.IsWhiteSpace(expression[i]))
                {
                    i++;
                    continue;
                }

                if (char.IsLetter(expression[i]))
                {
                    var start = i;
                    while (i < expression.Length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                        i++;

                    var value = expression.Substring(start, i - start);

                    if (Keywords.TryGetValue(value, out var keyword))
                    {
                        tokens.Add(new Token(keyword, value));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Variable, value));
                    }
                }
                else if (expression[i] == '[')
                {
                    Debug.Log($"Parsing set");
                    var start = i + 1;
                    i++;
                    while (i < expression.Length && expression[i] != ']')
                    {
                        i++;
                    }
                    var value = expression.Substring(start, i - start);
                    i++; // move one further to skip [
                    tokens.Add(new Token(TokenType.Set, value));
                }
                else if (expression[i] == '\'')
                {
                    var start = i + 1;
                    i++;
                    while (i < expression.Length && expression[i] != '\'')
                    {
                        i++;
                    }
                    var value = expression.Substring(start, i - start);
                    i++; // move one further to skip '
                    tokens.Add(new Token(TokenType.String, value));
                }
                else if (char.IsDigit(expression[i]))
                {
                    i = ParseNumber();
                }
                else if (expression[i] == '-')
                {
                    // negative number case
                    i++;
                    i = ParseNumber();
                }
                else if (Operators.Contains(expression[i]))
                {
                    tokens.Add(new Token(expression[i] == '(' ? TokenType.OpenParen : TokenType.CloseParen, expression[i].ToString()));
                    i++;
                }
                else
                {
                    var start = i;
                    while (i < expression.Length && !char.IsWhiteSpace(expression[i]) && !char.IsLetterOrDigit(expression[i]) && !Operators.Contains(expression[i]))
                        i++;

                    var value = expression.Substring(start, i - start);

                    if (Comparisons.ContainsKey(value))
                    {
                        tokens.Add(new Token(Comparisons[value], value));
                    }
                    else
                    {
                        throw new Exception($"Unexpected token: {value}");
                    }
                }
            }

            return tokens;

            int ParseNumber()
            {
                var start = i;
                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    i++;

                var value = expression.Substring(start, i - start);
                tokens.Add(new Token(TokenType.Number, value));
                return i;
            }
        }
    }
}