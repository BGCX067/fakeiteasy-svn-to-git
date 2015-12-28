namespace FakeItEasy
{
    using FakeItEasy.Api;

    /// <summary>
    /// Creates CallCollection(TFake) instances.
    /// </summary>
    public interface ICallCollectionFactory
    {
        /// <summary>
        /// Creates the call collection.
        /// </summary>
        /// <typeparam name="TFake">The type of the faked object.</typeparam>
        /// <param name="fake">The fake object.</param>
        /// <returns>A new call collection.</returns>
        CallCollection<TFake> CreateCallCollection<TFake>(FakeObject fake);
    }
}
