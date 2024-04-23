namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;

    public interface IMultiConfig<TConfig>
    {
        TConfig GetByKey(string key);

        IEnumerable<TConfig> GetAll();

        IEnumerable<string> GetAllKeys();
    }
}