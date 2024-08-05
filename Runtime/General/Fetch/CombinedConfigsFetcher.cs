namespace Unibrics.Configuration.General.Fetch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ABTests;
    using Core.Config;
    using Core.Features;
    using Core.Version;
    using Cysharp.Threading.Tasks;
    using Settings;
    using Zenject;
    using Logger = Logs.Logger;

    class CombinedConfigsFetcher : IConfigsFetcher
    {
        [Inject]
        public IConfigsConfigurator Configurator { get; set; }

        [Inject]
        public IFeatureSet FeatureSet { get; set; }

        [Inject]
        public IAppliedConfigsHolder AppliedConfigsHolder { get; set; }

        [Inject]
        public IConfigMetaProvider ConfigMetaProvider { get; set; }

        [Inject]
        public IConfigApplyCheckerFactory ConfigApplyCheckerFactory { get; set; }

        [Inject]
        public IVersionProvider VersionProvider { get; set; }

        [Inject]
        public IDefaultConfigsFetcher DefaultValuesFetcher { get; set; }

        [Inject]
        public IConfigMetadataExtractor MetadataExtractor { get; set; }

        [Inject]
        public IConfigValueResolver ConfigsResolver { get; set; }

        [Inject]
        public IAppSettings AppSettings { get; set; }

        private IConfigsFetcher remoteFetcher;
        
        public void StartFetching(TimeSpan fetchLimitTime)
        {
            if (Configurator.RemoteFetcher != null && !FeatureSet.GetFeature<RemoteConfigsFeature>().IsSuspended)
            {
                remoteFetcher = Configurator.RemoteFetcher;
            }
            else
            {
                remoteFetcher = new RemoteConfigsMockFetcher();
            }

            DefaultValuesFetcher.StartFetching(fetchLimitTime);
            remoteFetcher.StartFetching(fetchLimitTime);
        }

        private IEnumerable<string> GetAllConfigKeys(string version)
        {
            var set = new HashSet<string>();
            foreach (var key in AppliedConfigsHolder.GetKeysByVersion(version).ToList())
            {
                set.Add(key);
            }

            var remoteKeys = remoteFetcher.GetKeys().ToList();
            foreach (var key in remoteKeys.ToList())
            {
                set.Add(key);
            }

            foreach (var key in DefaultValuesFetcher.GetKeys())
            {
                set.Add(key);
            }

            foreach (var meta in ConfigMetaProvider.FetchMetas())
            {
                if (!meta.IsMultiConfig)
                {
                    set.Add(meta.Key);
                }
            }

            return set;
        }

        public async UniTask StopFetchingAndApply()
        {
            await DefaultValuesFetcher.StopFetchingAndApply();

            await remoteFetcher.StopFetchingAndApply();
            var remoteKeys = remoteFetcher.GetKeys().ToList();
            var version = VersionProvider.FullVersion;
            var keys = GetAllConfigKeys(version);
            var color = AppSettings.Get<ConfigurationSettings>().LogColor;
            var logSuffix = $"</color>\n\n<color={color}>\n";
            
            foreach (var key in keys)
            {
                if (!remoteKeys.Contains(key))
                {
                    ApplyLocalValue(key);
                    continue;
                }

                var remoteValue = remoteFetcher.GetValue(key);
                if (string.IsNullOrEmpty(remoteValue))
                {
                    ApplyLocalValue(key);
                    continue;
                }
                
                var metadata = MetadataExtractor.ExtractMetadata(remoteValue);
                var applyChecker = ConfigApplyCheckerFactory.Create(metadata.Apply, key);
               
                if (applyChecker.ShouldApply())
                {
                    Logger.Log("Config", $"Config '{key}' is applied from remote value{logSuffix}{remoteValue}");
                    PutValue(key, remoteValue);

                    if (applyChecker.ShouldCache())
                    {
                        Logger.Log("Config", $"Config '{key}' is cached for further use");

                        string cacheUntil = null;
                        if (applyChecker.IsCachePreservedBetweenVersions)
                        {
                            cacheUntil = metadata.CacheUntil;
                            Logger.Log("Config", $"Config '{key}' is cached until {cacheUntil}");
                        }
                        
                        AppliedConfigsHolder.Store(key, remoteValue, version, cacheUntil);
                    }
                }
                else
                {
                    Logger.Log("Config",
                        $"Config '{key}' is fetched from remote but skipped because of '{metadata.Apply}' mode");

                    ApplyLocalValue(key);
                }
            }

            void ApplyLocalValue(string key)
            {
                var appliedConfig = AppliedConfigsHolder.TryGetAppliedConfigFor(key, version);
                if (appliedConfig != null)
                {
                    Logger.Log("Config", $"Config '{key}' is taken from cache{logSuffix}{appliedConfig}");
                    PutValue(key, appliedConfig);
                    return;
                }

                if (!DefaultValuesFetcher.HasKey(key))
                {
                    Logger.Log("Config", $"Can not apply '{key}': no cached or local value found");
                    return;
                }
                
                appliedConfig = DefaultValuesFetcher.GetValue(key);
                Logger.Log("Config", $"Config '{key}' is taken from local value{logSuffix}{appliedConfig}");
                if (appliedConfig != null)
                {
                    PutValue(key, appliedConfig);
                }
            }
        }

        private void PutValue(string key, string value)
        {
            ConfigsResolver.PutValue(key, value);
        }

        public IEnumerable<string> GetKeys()
        {
            return ConfigsResolver.GetKeys();
        }

        public string GetValue(string key)
        {
            return ConfigsResolver.GetValue(key);
        }

    }
}