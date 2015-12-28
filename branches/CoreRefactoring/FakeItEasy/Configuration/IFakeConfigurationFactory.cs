namespace FakeItEasy.Configuration
{
    using FakeItEasy.Api;

    /// <summary>
    /// Responsible for creating IFakeConfiguration(TFake) instances.
    /// </summary>
    public interface IFakeConfigurationFactory
    {
        /// <summary>
        /// Creates a new configuration for the specified object.
        /// </summary>
        /// <typeparam name="TFake">The type of the faked object to configure.</typeparam>
        /// <param name="fakeObject">The fake object to configure.</param>
        /// <returns>A configuration object.</returns>
        IFakeConfiguration<TFake> Create<TFake>(FakeObject fakeObject);
    }
}
