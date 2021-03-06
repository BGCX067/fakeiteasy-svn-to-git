using System;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using FakeItEasy.VisualBasic;

namespace FakeItEasy.Configuration
{
    internal class FakeConfiguration<TFake>
            : IFakeConfiguration<TFake>,
              IVoidConfiguration<TFake>,
              IRepeatConfiguration<TFake>,
              IAfterCallSpecifiedConfiguration<TFake>,
              IVisualBasicConfiguration<TFake> where TFake : class
    {
        internal FakeConfiguration(FakeObject fake)
        {
            this.Fake = fake;
        }

        internal FakeConfiguration(FakeObject fake, BuildableCallRule ruleBeingBuilt)
        {
            this.Fake = fake;
            this.RuleBeingBuilt = ruleBeingBuilt;
        }

        public FakeObject Fake
        {
            get; private set;
        }

        public BuildableCallRule RuleBeingBuilt { get; private set; }

        public IReturnValueConfiguration<TFake, TMember> CallsTo<TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            this.InitializeInterceptorBeingBuilt(callSpecification);

            return new ReturnValueConfiguration<TMember> { ParentConfiguration = this };
        }

        public IVoidConfiguration<TFake> CallsTo(Expression<Action<TFake>> callSpecification)
        {
            this.InitializeInterceptorBeingBuilt(callSpecification);
            this.RuleBeingBuilt.Applicator = x => { };

            return this;
        }

        private void InitializeInterceptorBeingBuilt(LambdaExpression expression)
        {
            var callRule = new ExpressionCallRule(expression);
            Fake.AddRule(callRule);
            this.RuleBeingBuilt = callRule;
        }

        public IAfterCallSpecifiedConfiguration<TFake> Throws(Exception exception)
        {
            this.RuleBeingBuilt.Applicator = x => { throw exception; };
            return this;
        }

        public void NumberOfTimes(int numberOfTimesToRepeat)
        {
            Guard.IsInRange(numberOfTimesToRepeat, 1, int.MaxValue, "numberOfTimesToRepeat");

            this.RuleBeingBuilt.NumberOfTimesToCall = numberOfTimesToRepeat;
        }

        public class ReturnValueConfiguration<TMember>
            : IReturnValueConfiguration<TFake, TMember>
        {
            public FakeConfiguration<TFake> ParentConfiguration;

            public IAfterCallSpecifiedConfiguration<TFake> Returns(TMember value)
            {
                this.ParentConfiguration.RuleBeingBuilt.Applicator = x => x.SetReturnValue(value);
                return this.ParentConfiguration;
            }

            public IAfterCallSpecifiedConfiguration<TFake> Returns(Func<TMember> valueProducer)
            {
                Guard.IsNotNull(valueProducer, "valueProducer");

                this.ParentConfiguration.RuleBeingBuilt.Applicator = x => x.SetReturnValue(valueProducer.Invoke());
                return this.ParentConfiguration;
            }

            public IAfterCallSpecifiedConfiguration<TFake> Throws(Exception exception)
            {
                return this.ParentConfiguration.Throws(exception);
            }


            public IAfterCallSpecifiedConfiguration<TFake> Returns(Func<IFakeObjectCall, TMember> valueProducer)
            {
                Guard.IsNotNull(valueProducer, "valueProducer");

                this.ParentConfiguration.RuleBeingBuilt.Applicator = x => x.SetReturnValue(valueProducer(x));
                return this.ParentConfiguration;
            }
        }

        public IVoidConfiguration<TFake> AnyCall()
        {
            this.RuleBeingBuilt = new AnyCallCallRule();
            this.Fake.AddRule(this.RuleBeingBuilt);

            return this;
        }

        private class AnyCallCallRule
            : BuildableCallRule
        {
            protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return true;
            }
        }

        public IVoidConfiguration<TFake> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            var recordedRule = this.RuleBeingBuilt as RecordedCallRule;
            
            if (recordedRule == null)
            {
                // TODO: This limitation should be removed.
                throw new NotSupportedException("Only RecordedCallRule can be configured with argument predicate.");
            }

            recordedRule.IsApplicableToArguments = argumentsPredicate;
            return this;
        }

        public IAfterCallSpecifiedConfiguration<TFake> DoesNothing()
        {
            this.RuleBeingBuilt.Applicator = x => { };
            return this;
        }
    }
}
