namespace FakeItEasy.Api
{
    /// <summary>
    /// Represents a call to a fake object at interception time.
    /// </summary>
    public interface IWritableFakeObjectCall
        : IFakeObjectCall
    {
        /// <summary>
        /// Sets the return value of the call.
        /// </summary>
        /// <param name="value">The return value to set.</param>
        void SetReturnValue(object value);

        /// <summary>
        /// Calls the base method of the faked type.
        /// </summary>
        void CallBaseMethod();

        /// <summary>
        /// Freezes the call so that it can no longer be modified.
        /// </summary>
        /// <returns>A completed fake object call.</returns>
        ICompletedFakeObjectCall Freeze();
    }
}
