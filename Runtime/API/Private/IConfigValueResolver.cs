namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;

    public interface IConfigValueResolver
    {
        void PutValue(string key, string value);

        string GetValue(string key);

        IEnumerable<string> GetKeys();
    }
}