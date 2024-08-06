namespace Unibrics.Configuration.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Execution;
    using Cysharp.Threading.Tasks;
    using General;
    using General.ABTests;
    using Zenject;

    public abstract class ApplyConfigsCommand : ExecutableCommand
    {
        [Inject]
        IConfigsRegistry ConfigsRegistry { get; set; }

        [Inject]
        public IConfigMetaProvider ConfigMetaProvider { get; set; }

        [Inject]
        IConfigsFactory ConfigsFactory { get; set; }

        [Inject]
        public List<IABTestsReporter> AbTestsReporters { get; set; }

        protected async UniTask Process(IConfigsFetcher configsFetcher, List<ConfigMeta> configMetas)
        {
            await configsFetcher.StopFetchingAndApply();
            var configs = ConfigsFactory.PrepareConfigs(configsFetcher, configMetas);

            var reportedTests = new HashSet<string>();
            foreach (var config in configs.Where(file => file.HasActivationEvent))
            {
                if (!reportedTests.Add(config.ActivationEvent))
                {
                    continue;
                }

                AbTestsReporters.ForEach(reporter => reporter.ReportTestActivation(config));
            }
            
            foreach (var config in configs)
            {
                ConfigsRegistry.Register(config);
            }
        }
    }
}