namespace FakeItEasy.Assertion
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides assertions for fake objects.
    /// </summary>
    /// <typeparam name="TFake">The type of the fake.</typeparam>
    internal class FakeAsserter<TFake> : IFakeAssertions<TFake>
    {
        private ICallCollectionFactory callCollectionFactory;
        private ExpressionCallMatcher.Factory callMatcherFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeAsserter&lt;TFake&gt;"/> class.
        /// </summary>
        /// <param name="fake">The fake.</param>
        public FakeAsserter(FakeObject fake, ICallCollectionFactory callCollectionFactory, ExpressionCallMatcher.Factory callMatcherFactory)
        {
            Guard.IsNotNull(fake, "fake");
            Guard.IsNotNull(callCollectionFactory, "callCollectionFactory");
            Guard.IsNotNull(callMatcherFactory, "callMatcherFactory");

            this.Fake = fake;
            this.callCollectionFactory = callCollectionFactory;
            this.callMatcherFactory = callMatcherFactory;
        }

        /// <summary>
        /// Gets or sets the fake.
        /// </summary>
        /// <value>The fake.</value>
        public FakeObject Fake
        {
            get;
            private set;
        }

        /// <summary>
        /// Throws an exception if the specified call has not been called.
        /// </summary>
        /// <param name="voidCall"></param>
        public void WasCalled(Expression<Action<TFake>> voidCall)
        {
            this.OnWasCalled(voidCall);           
        }

        /// <summary>
        /// Wases the called.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="returnValueCall">The return value call.</param>
        public void WasCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall)
        {
            this.OnWasCalled(returnValueCall);
        }

        public void WasNotCalled(Expression<Action<TFake>> voidCall)
        {
            this.OnWasNotCalled(voidCall);
        }

        public void WasNotCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall)
        {
            this.OnWasNotCalled(returnValueCall);
        }
        

        private void OnWasNotCalled(LambdaExpression expression)
        {
            var matcher = this.CreateMatcher(expression);

            if (this.Fake.RecordedCallsInScope.Any(x => matcher.Matches(x)))
            {
                throw new ExpectationException("Expected call not to be found but found it.");
            }  
        }

        private void OnWasCalled(LambdaExpression expression)
        {
            var matcher = this.CreateMatcher(expression);

            if (!this.Fake.RecordedCallsInScope.Any(x => matcher.Matches(x)))
            {
                var writer = new StringWriter(CultureInfo.InvariantCulture);
                this.Fake.RecordedCallsInScope.Cast<IFakeObjectCall>().WriteCalls(writer);

                var messageTemplate = "Expected to find call {0} but could not find it among the calls: \r\n {1}";
                throw new ExpectationException(messageTemplate.FormatInvariant(matcher.ToString(), writer.ToString()));
            }
        }


        public void WasCalled(Expression<Action<TFake>> voidCall, Expression<Func<int, bool>> repeatValidation)
        {
            Guard.IsNotNull(voidCall, "voidCall");
            Guard.IsNotNull(repeatValidation, "repeatValidation");

            this.OnWasCalled(voidCall, repeatValidation);
        }

        private void OnWasCalled(LambdaExpression expression, Expression<Func<int, bool>> repeatValidation)
        {
            var matcher = this.CreateMatcher(expression);
            var repeat = this.Fake.RecordedCallsInScope.Where(x => matcher.Matches(x)).Count();

            if (!repeatValidation.Compile().Invoke(repeat))
            {
                ThrowWhenNotCalledWithCorrectRepeat(matcher, repeatValidation, repeat);
            }
        }

        private ExpressionCallMatcher CreateMatcher(LambdaExpression callSpecification)
        {
            return this.callMatcherFactory.Invoke(callSpecification);            
        }

        private static void ThrowWhenNotCalledWithCorrectRepeat(ExpressionCallMatcher matcher, Expression<Func<int, bool>> repeatValidation, int repeat)
        {
            var messageTemplate = ExceptionMessages.WasCalledWrongNumberOfTimes.FormatInvariant(matcher.ToString(), repeatValidation.ToString(), repeat);
            throw new ExpectationException(messageTemplate);
        }

        private CallCollection<TFake> GetCalls()
        {
            return this.callCollectionFactory.CreateCallCollection<TFake>(this.Fake);
        }


        public void WasCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall, Expression<Func<int, bool>> repeatValidation)
        {
            Guard.IsNotNull(returnValueCall, "returnValueCall");
            Guard.IsNotNull(repeatValidation, "repeatValidation");

            this.OnWasCalled(returnValueCall, repeatValidation);
        }
    }
}