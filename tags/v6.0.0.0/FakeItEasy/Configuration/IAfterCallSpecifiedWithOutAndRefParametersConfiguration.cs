namespace FakeItEasy.Configuration
{
    /// <summary>
    /// A combination of the IAfterCallSpecifiedConfiguration and IOutAndRefParametersConfiguration
    /// interfaces.
    /// </summary>
    /// <typeparam name="TFake">The type of fake.</typeparam>
    public interface IAfterCallSpecifiedWithOutAndRefParametersConfiguration<TFake>
        : IAfterCallSpecifiedConfiguration<TFake>, IOutAndRefParametersConfiguration<TFake>
    { }
}
