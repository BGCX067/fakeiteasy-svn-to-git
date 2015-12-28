using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Legend.Fakes.Configuration;
using Legend.Fakes.Api;
using System.IO;
using System.Globalization;

namespace Legend.Fakes.Assertion
{
    internal class FakeAsserter<TFake> : IFakeAssertions<TFake> where TFake : class
    {
        public FakeAsserter(FakeObject fake)
        {
            Guard.IsNotNull(fake, "fake");

            this.Fake = fake;
        }

        public FakeObject Fake
        {
            get;
            private set;
        }

        public void WasCalled(Expression<Action<TFake>> voidCall)
        {
            this.OnWasCalled(voidCall);           
        }

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
            var rule = new ExpressionCallRule(expression);

            if (this.Fake.RecordedCalls.Any(x => rule.IsApplicableTo(x)))
            {
                throw new ExpectationException("Expected call not to be found but found it.");
            }  
        }

        private void OnWasCalled(LambdaExpression expression)
        {
            var rule = new ExpressionCallRule(expression);

            if (!this.Fake.RecordedCalls.Any(x => rule.IsApplicableTo(x)))
            {
                var writer = new StringWriter(CultureInfo.InvariantCulture);
                this.Fake.RecordedCalls.WriteCalls(writer);

                var messageTemplate = "Expected to find call {0} but could not find it among the calls: \r\n {1}";
                throw new ExpectationException(messageTemplate.FormatInvariant(rule.ToString(), writer.ToString()));
            }
        }
    }
}