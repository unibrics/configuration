﻿namespace Unibrics.Configuration.General
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using UnityEngine;
    using Utils.Json;

    interface IConfigsFactory
    {
        List<ConfigFile> PrepareConfigs(IConfigsFetcher configsFetcher, List<ConfigMeta> configMetas);
    }

    class ConfigsFactory : IConfigsFactory
    {
        private readonly IConfigObjectCreator configObjectCreator;

        private readonly IConfigValuesInjector valuesInjector;

        private readonly ILazyConfigsChecker lazyConfigsChecker;

        private const string Delimiter = "<br>";

        public ConfigsFactory(IConfigObjectCreator configObjectCreator, IConfigValuesInjector valuesInjector,
            ILazyConfigsChecker lazyConfigsChecker)
        {
            this.lazyConfigsChecker = lazyConfigsChecker;
            this.configObjectCreator = configObjectCreator;
            this.valuesInjector = valuesInjector;
        }

        public List<ConfigFile> PrepareConfigs(IConfigsFetcher configsFetcher,
            List<ConfigMeta> configMetas)
        {
            var keys = configsFetcher.GetKeys().ToList();
            var patcher = new ConfigsPatcher(configsFetcher);
            var validator = new ConfigsValidator(configMetas);

            var result = new List<ConfigFile>();
            foreach (var configMeta in configMetas)
            {
                if (configMeta.IsMultiConfig)
                {
                    var prefix = configMeta.Key;
                    var prefixedKeys = keys.Where(key => key.StartsWith(prefix)).ToList();
                    if (!prefixedKeys.Any() && !configMeta.IsOptional)
                    {
                        throw new Exception($"Can not find any config prefixed with '{prefix}'" +
                            $" to create MultiConfig of {configMeta.InterfaceType}. Add config file or mark it with IsOptional" +
                            $"property");
                    }

                    var multiObject = configObjectCreator.CreateMultiConfigFor(configMeta);
                    var lazy = lazyConfigsChecker.AreLazyConfigEnabled();
                    foreach (var key in prefixedKeys)
                    {
                        multiObject.Add(key[prefix.Length..], () => Process(key), lazy);
                    }
                }
                else
                {
                    var key = configMeta.Key;
                    if (!keys.Contains(key))
                    {
                        throw new Exception(
                            $"Can not find requested config '{key}' for type {configMeta.InterfaceType}");
                    }

                    Process(key);
                }

                ConfigFile Process(string key)
                {
                    var value = configsFetcher.GetValue(key);
                    try
                    {
                        value = value.Replace(Delimiter, "\n");
                        var configObject = configObjectCreator.CreateObject(configMeta);
                        value = patcher.TryPatch(key, value);
                        valuesInjector.InjectTo(configObject, value);

                        validator.OnConfigPrepared(configMeta, configObject);

                        result.Add(configObject);
                        return configObject;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error while processing {key}:");
                        Debug.LogError(value);
                        Debug.LogException(e);
                    }

                    return null;
                }
            }

            validator.Validate();
            return result;
        }
    }
}