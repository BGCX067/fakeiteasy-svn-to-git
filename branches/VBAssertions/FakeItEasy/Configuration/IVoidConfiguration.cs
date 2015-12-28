namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for methods that does not have a return value.
    /// </summary>
    /// <typeparam name="TFake">The type of the fake.</typeparam>
    public interface IVoidConfiguration<TFake>
        : IExceptionThrowerConfiguration<TFake>, 
          ICallbackConfiguration<IVoidConfiguration<TFake>>, 
          ICallBaseConfiguration<TFake>,
          IHideObjectMembers
    {
        /// <summary>
        /// Configures the specified call to do nothing when called.
        /// </summary>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration<TFake> DoesNothing();
    }
}