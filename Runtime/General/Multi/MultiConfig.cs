namespace Unibrics.Configuration.General.Multi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    abstract class MultiConfig
    {
        internal abstract void Add(string key, Func<ConfigFile> config, bool lazy);
    }

    class MultiConfig<TConfig> : MultiConfig, IMultiConfig<TConfig> where TConfig : class
    {
        private readonly Dictionary<string, TConfig> configs = new();
        
        private readonly Dictionary<string, Func<TConfig>> lazyGetters = new();

        public TConfig GetBuyId(string id)
        {
            if (configs.TryGetValue(id, out var config))
            {
                return config;
            }

            if (lazyGetters.TryGetValue(id, out var getter))
            {
                var createdConfig = getter();
                configs[id] = createdConfig;
                return createdConfig;
            }

            return default;
        }

        public IEnumerable<TConfig> GetAll() => configs.Values;
        
        public IEnumerable<(string key, TConfig value)> GetAllWithKeys()
        {
            return configs.Select(config => (config.Key, config.Value));
        }

        internal override void Add(string key, Func<ConfigFile> config, bool lazy)
        {
            if (config is not Func<TConfig> typed)
            {
                throw new Exception($"Config type {config.GetType()} must implement interface {typeof(TConfig)} " +
                    $"to be used in MultiConfig");
            }

            if (lazy)
            {
                lazyGetters.Add(key, typed);
            }
            else
            {
                configs.Add(key, typed());
            }
        }
    }
}