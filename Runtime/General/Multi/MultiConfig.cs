namespace Unibrics.Configuration.General.Multi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    abstract class MultiConfig
    {
        internal abstract void Add(string key, Func<ConfigFile> getter, bool lazy);
    }

    class MultiConfig<TConfig> : MultiConfig, IMultiConfig<TConfig> where TConfig : class
    {
        private readonly Dictionary<string, TConfig> configs = new();
        
        private readonly Dictionary<string, Func<ConfigFile>> lazyGetters = new();

        public TConfig GetByKey(string key)
        {
            if (configs.TryGetValue(key, out var config))
            {
                return config;
            }

            if (lazyGetters.TryGetValue(key, out var getter))
            {
                var rawConfig = getter();
                if (rawConfig is not TConfig typedConfig)
                {
                    throw new Exception($"Config type {rawConfig.GetType()} must implement interface {typeof(TConfig)} " +
                        $"to be used in MultiConfig");
                }
                configs[key] = typedConfig;
                return typedConfig;
            }

            return default;
        }

        public IEnumerable<TConfig> GetAll() => configs.Values;
        
        public IEnumerable<string> GetAllKeys()
        {
            return configs.Select(config => config.Key).Concat(lazyGetters.Select(pair => pair.Key));
        }

        internal override void Add(string key, Func<ConfigFile> getter, bool lazy)
        {
            if (lazy)
            {
                lazyGetters.Add(key, getter);
            }
            else
            {
                var value = getter();
                if (value is not TConfig typed)
                {
                    throw new Exception($"Config type {getter.GetType()} must implement interface {typeof(TConfig)} " +
                        $"to be used in MultiConfig");
                }
                configs.Add(key, typed);
            }
        }
    }
}