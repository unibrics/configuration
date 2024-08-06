namespace Unibrics.Configuration.Settings
{
    using Core;
    using Core.Config;

    [InstallWithId("configuration")]
    public class ConfigurationSettings : IAppSettingsSection, IConfigurationFetchingTimeoutProvider
    {
        public bool EnableLazyConfigs { get; set; }
        
        public float TimeoutSeconds { get; set; }

        public string LogColor { get; set; } = "white";
    }

    internal interface IConfigurationFetchingTimeoutProvider
    {
        float TimeoutSeconds { get; }
    }
}