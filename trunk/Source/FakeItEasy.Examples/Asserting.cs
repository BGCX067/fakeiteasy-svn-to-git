namespace FakeItEasy.Examples
{
    public class Asserting
    {
        public void Asserting_on_calls()
        {
            var factory = A.Fake<IWidgetFactory>();

            // This would throw an exception since no call has been made.
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Once);
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Twice);
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Times(10));

            // This on the other hand would not throw.
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Never);

            // The number of times the call has been made can be restricted so that it must have happened
            // exactly the number of times specified
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Once.Exactly);
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Times(4).Exactly);

            // Then number of times can be specified so that it must not have happened more
            // than the specified number of times.
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Twice.OrLess);
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Times(5).OrLess);
        }
    }
}