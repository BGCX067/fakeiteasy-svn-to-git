using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using System.IO;
using System.Globalization;

namespace FakeItEasy.Assertion
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

            if (this.Fake.RecordedCallsInScope.Any(x => rule.IsApplicableTo(x)))
            {
                throw new ExpectationException("Expected call not to be found but found it.");
            }  
        }

        private void OnWasCalled(LambdaExpression expression)
        {
            var rule = new ExpressionCallRule(expression);

            if (!this.Fake.RecordedCallsInScope.Any(x => rule.IsApplicableTo(x)))
            {
                var writer = new StringWriter(CultureInfo.InvariantCulture);
                this.Fake.RecordedCallsInScope.Cast<IFakeObjectCall>().WriteCalls(writer);

                var messageTemplate = "Expected to find call {0} but could not find it among the calls: \r\n {1}";
                throw new ExpectationException(messageTemplate.FormatInvariant(rule.ToString(), writer.ToString()));
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
            var rule = new ExpressionCallRule(expression);
            var repeat = this.Fake.RecordedCallsInScope.Where(x => rule.IsApplicableTo(x)).Count();

            if (!repeatValidation.Compile().Invoke(repeat))
            {
                ThrowWhenNotCalledWithCorrectRepeat(rule, repeatValidation, repeat);
            }
        }

        private static void ThrowWhenNotCalledWithCorrectRepeat(ExpressionCallRule rule, Expression<Func<int, bool>> repeatValidation, int repeat)
        {
            var messageTemplate = ExceptionMessages.WasCalledWrongNumberOfTimes.FormatInvariant(rule.ToString(), repeatValidation.ToString(), repeat);
            throw new ExpectationException(messageTemplate);
        }

        private CallCollection<TFake> GetCalls()
        {
            return new CallCollection<TFake>(this.Fake);
        }


        public void WasCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall, Expression<Func<int, bool>> repeatValidation)
        {
            Guard.IsNotNull(returnValueCall, "returnValueCall");
            Guard.IsNotNull(repeatValidation, "repeatValidation");

            this.OnWasCalled(returnValueCall, repeatValidation);
        }
    }
}