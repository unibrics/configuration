namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Utils.Json;

    interface IConfigsFactory
    {
        List<ConfigFile> PrepareConfigs(IConfigsFetcher configsFetcher, List<ConfigMeta> configMetas);
    }

}