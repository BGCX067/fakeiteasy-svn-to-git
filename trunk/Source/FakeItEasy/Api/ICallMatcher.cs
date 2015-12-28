namespace FakeItEasy.Api
{
    /// <summary>
    /// Represents a predicate that matches a fake object call.
    /// </summary>
    public interface ICallMatcher
    {
        /// <summary>
        /// Gets a value indicating whether the call matches the predicate.
        /// </summary>
        /// <param name="call">The call to match.</param>
        /// <returns>True if the call matches the predicate.</returns>
        bool Matches(IFakeObjectCall call);
    }
}
