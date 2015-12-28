using System;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using FakeItEasy.VisualBasic;
using FakeItEasy.Expressions;

namespace FakeItEasy.Configuration
{
    internal class FakeConfiguration
            : IVoidArgumentValidationConfiguration,
              IRepeatConfiguration,
              IAfterCallSpecifiedConfiguration,
              IVisualBasicConfigurationWithArgumentValidation,
              IAfterCallSpecifiedWithOutAndRefParametersConfiguration
    {
        
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

        public IAfterCallSpecifiedConfiguration Throws(Exception exception)
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
            : IReturnValueArgumentValidationConfiguration<TMember>
        {
            public FakeConfiguration ParentConfiguration;

            public IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns(TMember value)
            {
                this.ParentConfiguration.RuleBeingBuilt.Applicator = x => x.SetReturnValue(value);
                return this.ParentConfiguration;
            }

            public IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns(Func<TMember> valueProducer)
            {
                Guard.IsNotNull(valueProducer, "valueProducer");

                this.ParentConfiguration.RuleBeingBuilt.Applicator = x => x.SetReturnValue(valueProducer.Invoke());
                return this.ParentConfiguration;
            }

            public IAfterCallSpecifiedConfiguration Throws(Exception exception)
            {
                return this.ParentConfiguration.Throws(exception);
            }


            public IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns(Func<IFakeObjectCall, TMember> valueProducer)
            {
                Guard.IsNotNull(valueProducer, "valueProducer");

                this.ParentConfiguration.RuleBeingBuilt.Applicator = x => x.SetReturnValue(valueProducer(x));
                return this.ParentConfiguration;
            }

            public IReturnValueConfiguration<TMember> Invokes(Action<IFakeObjectCall> action)
            {
                Guard.IsNotNull(action, "action");

                this.ParentConfiguration.RuleBeingBuilt.Actions.Add(action);
                return this;
            }

            public IAfterCallSpecifiedConfiguration CallsBaseMethod()
            {
                return this.ParentConfiguration.CallsBaseMethod();
            }

            public IReturnValueConfiguration<TMember> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
            {
                Guard.IsNotNull(argumentsPredicate, "argumentsPredicate");

                this.ParentConfiguration.RuleBeingBuilt.UsePredicateToValidateArguments(argumentsPredicate);
                return this;
            }
        }

        public IVoidConfiguration AnyCall()
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

            public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
            {
                throw new NotSupportedException();
            }
        }

        public IVoidConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.IsNotNull(argumentsPredicate, "argumentsPredicate");

            this.RuleBeingBuilt.UsePredicateToValidateArguments(argumentsPredicate);
            return this;
        }

        public IAfterCallSpecifiedConfiguration DoesNothing()
        {
            this.RuleBeingBuilt.Applicator = x => { };
            return this;
        }

        public IVoidConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            Guard.IsNotNull(action, "action");

            this.RuleBeingBuilt.Actions.Add(action);
            return this;
        }

        public IAfterCallSpecifiedConfiguration CallsBaseMethod()
        {
            this.RuleBeingBuilt.Applicator = x => { };
            this.RuleBeingBuilt.CallBaseMethod = true;
            return this;
        }

        public void AssertWasCalled(Func<int, bool> repeatPredicate)
        {
            Guard.IsNotNull(repeatPredicate, "repeatPredicate");

            var recordedRule = this.RuleBeingBuilt as RecordedCallRule;

            if (recordedRule == null)
            {
                throw new InvalidOperationException("Only RecordedCallRules can be used for assertions.");
            }

            recordedRule.IsAssertion = true;
            recordedRule.RepeatPredicate = repeatPredicate;
            
        }


        public IAfterCallSpecifiedConfiguration AssignsOutAndRefParameters(params object[] values)
        {
            Guard.IsNotNull(values, "values");
            
            this.RuleBeingBuilt.OutAndRefParametersValues = values;

            return this;
        }



        IVisualBasicConfiguration IArgumentValidationConfiguration<IVisualBasicConfiguration>.WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.IsNotNull(argumentsPredicate, "argumentsPredicate");

            this.RuleBeingBuilt.UsePredicateToValidateArguments(argumentsPredicate);

            return this;
        }
    }
}