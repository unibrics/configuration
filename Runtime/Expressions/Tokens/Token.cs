namespace Unibrics.Configuration.Expressions.Tokens
{

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString() => $"{Type}: {Value}";
    }

    public enum TokenType
    {
        Variable,
        Keyword,
        Comparison,
        LogicalOperator,
        String,
        Set,
        Number,
        OpenParen,
        CloseParen
    }
}