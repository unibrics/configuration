namespace Unibrics.Configuration.General
{
    using Codice.Utils;
    using Core.Config;
    using Settings;

    public class LazyConfigsChecker : ILazyConfigsChecker
    {
        private readonly IAppSettings settings;

        public LazyConfigsChecker(IAppSettings settings)
        {
            this.settings = settings;
        }

        public bool AreLazyConfigEnabled() => settings.Get<ConfigurationSettings>().EnableLazyConfigs;
    }
}