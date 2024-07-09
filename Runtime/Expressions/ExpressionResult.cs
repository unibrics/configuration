namespace Unibrics.Configuration.Expressions
{
    using System;
    using UnityEngine;

    public class ExpressionResult
    {
        private readonly object value;

        public ExpressionResult(object value)
        {
            this.value = value;
        }

        public bool AsBool
        {
            get
            {
                Debug.Log($"Converting to bool: {value}");
                return Convert.ToBoolean(value);
            }
        }

        public double AsDouble => Convert.ToDouble(value);

        public string AsString => Convert.ToString(value);

        public object RawValue => value;

        public object[] AsSet
        {
            get
            {
                var raw = Convert.ToString(value).Split(",");
                var result = new object[raw.Length];
                for (var i = 0; i < result.Length; i++)
                {
                    var trimmed = raw[i].TrimEnd().TrimStart();
                    if (trimmed.StartsWith("["))
                    {
                        throw new Exception("Nested sets are not supported");
                    }
                    result[i] = trimmed.StartsWith("\'") ? trimmed[1..^1] : Convert.ToDouble(trimmed);
                }

                return result;
            }
        }

        protected bool Equals(ExpressionResult other)
        {
            return Equals(value, other.value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExpressionResult)obj);
        }

        public override int GetHashCode()
        {
            return (value != null ? value.GetHashCode() : 0);
        }
    }
}