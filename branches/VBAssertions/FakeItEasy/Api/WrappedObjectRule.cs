namespace FakeItEasy.Api
{
    using System.Linq;

    /// <summary>
    /// A call rule that applies to any call and just delegates the
    /// call to the wrapped object.
    /// </summary>
    internal class WrappedObjectRule
        : IFakeObjectCallRule
    {
        private object wrappedObject;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="wrappedInstance">The object to wrap.</param>
        public WrappedObjectRule(object wrappedInstance)
        {
            this.wrappedObject = wrappedInstance;
        }

        /// <summary>
        /// Gets wether this interceptor is applicable to the specified
        /// call, if true is returned the Apply-method of the interceptor will
        /// be called.
        /// </summary>
        /// <param name="fakeObjectCall">The call to check for applicability.</param>
        /// <returns>True if the interceptor is applicable.</returns>
        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        /// <summary>
        /// Applies an action to the call, might set a return value or throw
        /// an exception.
        /// </summary>
        /// <param name="fakeObjectCall">The call to apply the interceptor to.</param>
        public void Apply(IWritableFakeObjectCall fakeObjectCall)
        {
            fakeObjectCall.SetReturnValue(fakeObjectCall.Method.Invoke(this.wrappedObject, fakeObjectCall.Arguments.AsEnumerable().ToArray()));
        }

        /// <summary>
        /// Gets the number of times this call rule is valid, if it's set
        /// to null its infinitely valid.
        /// </summary>
        /// <value></value>
        public int? NumberOfTimesToCall
        {
            get { return null; }
        }

    }
}