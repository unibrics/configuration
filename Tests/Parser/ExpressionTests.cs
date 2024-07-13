namespace Unibrics.Configuration.Tests.Parser
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Expressions;
    using Expressions.Tokens;
    using NUnit.Framework;
    using UnityEngine;
    using ExpressionEvaluator = Expressions.ExpressionEvaluator;

    [TestFixture]
    public class ExpressionTests
    {
        public object Evaluate(string expression, Dictionary<string, object> variables = null)
        {
            var tokenizer = new Tokenizer();
            var parser = new TokensParser();
            var evaluator = new ExpressionEvaluator(tokenizer, parser);

            var tokens = tokenizer.Tokenize(expression);
            var ast = parser.Parse(tokens);
            return evaluator.Evaluate(expression, variables ?? new Dictionary<string, object>());
        }

        [Test]
        public void _01Examples()
        {
            var result = Evaluate("c or (a and b) and k >= 4 and word is '^a*$'", new Dictionary<string, object>
            {
                { "a", true },
                { "b", true },
                { "c", false },
                { "k", 5 },
                { "word", "aa" }
            });

            Assert.That(result, Is.True);
        }

        [Test]
        public void _02Logicals()
        {
            var result = Evaluate("a and b and c", new Dictionary<string, object>
            {
                { "a", false },
                { "b", true },
                { "c", false }
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void _03Comparisons()
        {
            Assert.That(Evaluate("3 < 5"), Is.True);
            Assert.That(Evaluate("b == 'hello'", new Dictionary<string, object>()
            {
                ["b"] = "hello"
            }), Is.True);
            Assert.That(Evaluate("7 >= 5"), Is.True);
            Assert.That(Evaluate("0 > 0"), Is.False);
            Assert.That(Evaluate("a == a", new Dictionary<string, object>()
            {
                ["a"] = 15
            }), Is.True);
        }

        [Test]
        public void _04Regexp()
        {
            Assert.That(Regex.IsMatch("aa", "^a*$"), Is.True);
            Assert.That(Evaluate("word is '^a*$'", new Dictionary<string, object>()
            {
                ["word"] = "aa"
            }), Is.True);

            Assert.That(Evaluate("word isnot '^a*$'", new Dictionary<string, object>()
            {
                ["word"] = "bb"
            }), Is.True);
        }

        [Test]
        public void _05Parenthesis()
        {
            Assert.That(Evaluate("((a and b) or c)", new Dictionary<string, object>()
            {
                ["a"] = false,
                ["b"] = true,
                ["c"] = true,
            }), Is.True);
        }
        
        [Test]
        public void _05Sets()
        {
            Assert.That(Evaluate("5 in [1,  2, 4, 5]"), Is.True);
            Assert.That(Evaluate("'ab' in [1,2, '4', 5]"), Is.False);
            Assert.That(Evaluate("'ab' in [1,'ab', '4', 5]"), Is.True);
            Assert.That(Evaluate("'ab' notin [1,2, '4', 5]"), Is.True);
        }
    }
}