namespace Unibrics.Configuration.Tests.Mocks
{
    using General;

    public class LazyConfigsTester : ILazyConfigsChecker
    {
        private bool value;

        public LazyConfigsTester(bool value)
        {
            this.value = value;
        }

        public bool AreLazyConfigEnabled() => value;
    }
}