namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration for method calls that has a return value.
    /// </summary>
    /// <typeparam name="TFake">The type of the faked object.</typeparam>
    public interface IRepeatConfiguration<TFake>
            : IHideObjectMembers
    {
        /// <summary>
        /// Specifies the number of times for the configured event.
        /// </summary>
        /// <param name="numberOfTimesToRepeat">The number of times to repeat.</param>
        void NumberOfTimes(int numberOfTimesToRepeat);
    }
}
