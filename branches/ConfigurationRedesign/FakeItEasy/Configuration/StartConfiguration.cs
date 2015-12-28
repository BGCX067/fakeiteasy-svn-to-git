namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using FakeItEasy.Expressions;

    public class StartConfiguration<TFake>
        : IFakeConfiguration<TFake>, IHideObjectMembers
    {
        private FakeObject fakeObject;
        private ExpressionCallRule.Factory callRuleFactory;
        private IConfigurationFactory configurationFactory;

        internal StartConfiguration(FakeObject fakeObject, ExpressionCallRule.Factory callRuleFactory, IConfigurationFactory configurationFactory)
        {
            this.fakeObject = fakeObject;
            this.callRuleFactory = callRuleFactory;
            this.configurationFactory = configurationFactory;
        }

        public IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");

            var rule = this.callRuleFactory(callSpecification);
            this.fakeObject.AddRule(rule);
            return this.configurationFactory.CreateConfiguration<TMember>(this.fakeObject, rule);
        }

        public IVoidArgumentValidationConfiguration CallsTo(Expression<Action<TFake>> callSpecification)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");

            var rule = this.callRuleFactory(callSpecification);
            rule.Applicator = x => { };
            this.fakeObject.AddRule(rule);
            return this.configurationFactory.CreateConfiguration(this.fakeObject, rule);
        }

        public IVoidConfiguration AnyCall()
        {
            var rule = new AnyCallCallRule();
            this.fakeObject.AddRule(rule);
            return this.configurationFactory.CreateConfiguration(this.fakeObject, rule);
        }
    }
}