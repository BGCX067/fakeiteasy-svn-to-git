namespace FakeItEasy.Configuration
{
    using FakeItEasy.Api;
    using FakeItEasy.Assertion;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    using FakeItEasy.VisualBasic;

    internal class ConfigurationModule
        : Module
    {
        public override void RegisterDependencies(DictionaryContainer container)
        {
            container.RegisterSingleton<RecordedCallRule.Factory>(c =>
                () => new RecordedCallRule(c.Resolve<MethodInfoManager>()));

            container.RegisterSingleton<IRecordingCallRuleFactory>(c =>
                new RecordingCallRuleFactory() { Container = c });

            container.RegisterSingleton<IConfigurationFactory>(c =>
                new ConfigurationFactory() { Container = c });

            container.RegisterSingleton<IStartConfigurationFactory>(c =>
                new StartConfigurationFactory() { Container = c });

            container.RegisterSingleton<RuleBuilder.Factory>(c =>
                rule => new RuleBuilder(rule));
        }

        private class RecordingCallRuleFactory : IRecordingCallRuleFactory
        {
            public DictionaryContainer Container;

            public RecordingCallRule<TFake> Create<TFake>(FakeObject fakeObject, RecordedCallRule recordedRule)
            {
                return new RecordingCallRule<TFake>(fakeObject, recordedRule, this.Container.Resolve<FakeAsserter.Factory>());
            }
        }

        private class ConfigurationFactory : IConfigurationFactory
        {
            public DictionaryContainer Container;

            private RuleBuilder.Factory BuilderFactory
            {
                get
                {
                    return this.Container.Resolve<RuleBuilder.Factory>();
                }
            }

            public IVoidArgumentValidationConfiguration CreateConfiguration(FakeObject fakeObject, BuildableCallRule callRule)
            {
                return this.BuilderFactory.Invoke(callRule);
            }

            public IReturnValueArgumentValidationConfiguration<TMember> CreateConfiguration<TMember>(FakeObject fakeObject, BuildableCallRule callRule)
            {
                var parent = this.BuilderFactory.Invoke(callRule);
                var configuration = new RuleBuilder.ReturnValueConfiguration<TMember>();
                configuration.ParentConfiguration = parent;
                return configuration;
            }
        }

        private class StartConfigurationFactory : IStartConfigurationFactory
        {
            public DictionaryContainer Container;

            public IFakeConfiguration<TFake> CreateConfiguration<TFake>(FakeObject fakeObject)
            {
                return new StartConfiguration<TFake>(fakeObject, this.Container.Resolve<ExpressionCallRule.Factory>(), this.Container.Resolve<IConfigurationFactory>());
            }
        }
    }
}
