namespace FakeItEasy
{
    using FakeItEasy.Api;
    using FakeItEasy.Assertion;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
using FakeItEasy.Configuration;
using FakeItEasy.VisualBasic;
    
    /// <summary>
    /// Handles the registration of root dependencies in an IoC-container.
    /// </summary>
    internal class RootModule 
        : Module
    {
        /// <summary>
        /// Registers the dependencies.
        /// </summary>
        /// <param name="container">The container to register the dependencies in.</param>
        public override void RegisterDependencies(DictionaryContainer container)
        {
            container.Register<FakeScope>(c => 
                FakeScope.Current);

            container.RegisterSingleton<IFakeProxyGenerator>(c => 
                new FakeItEasy.DynamicProxy.DynamicProxyGenerator());

            container.RegisterSingleton<FakeObjectFactory.Creator>(c =>
                () => new FakeObjectFactory(c.Resolve<IFakeObjectContainer>()));

            container.RegisterSingleton<ExpressionCallMatcher.Factory>(c =>
                expression => new ExpressionCallMatcher(expression, c.Resolve<ArgumentValidatorFactory>(), c.Resolve<MethodInfoManager>()));

            container.Register<IFakeObjectContainer>(c => 
                c.Resolve<FakeScope>().FakeObjectContainer);

            container.RegisterSingleton<ArgumentValidatorFactory>(c => 
                new ArgumentValidatorFactory());

            container.RegisterSingleton<ExpressionCallRule.Factory>(c =>
                callSpecification => new ExpressionCallRule(c.Resolve<ExpressionCallMatcher.Factory>().Invoke(callSpecification)));

            container.RegisterSingleton<ICallCollectionFactory>(c =>
                new CallCollectionFactory { Container = c });

            container.RegisterSingleton<IFakeAssertionsFactory>(c =>
                new FakeAssertionsFactory { Container = c });

            container.RegisterSingleton<IFakeConfigurationFactory>(c =>
                new FakeConfigurationFactory { Container = c });

            container.RegisterSingleton<MethodInfoManager>(c =>
                new MethodInfoManager());

            container.RegisterSingleton<RecordedCallRule.Factory>(c =>
                () => new RecordedCallRule(c.Resolve<MethodInfoManager>()));

            container.RegisterSingleton<IRecordingCallRuleFactory>(c =>
                new RecordingCallRuleFactory() { Container = c });

            container.RegisterSingleton<FakeAsserter.Factory>(c =>
                x => new FakeAsserter(x));
        }

        #region FactoryImplementations
        private class CallCollectionFactory : ICallCollectionFactory
        {
            public DictionaryContainer Container;

            public ICallCollection<TFake> CreateCallCollection<TFake>(FakeObject fake)
            {
                return new CallCollection<TFake>(fake, this.Container.Resolve<ExpressionCallMatcher.Factory>());
            }
        }

        private class FakeAssertionsFactory : IFakeAssertionsFactory
        {
            public DictionaryContainer Container;

            public IFakeAssertions<TFake> CreateAsserter<TFake>(FakeObject fake)
            {
                return new FakeAssertions<TFake>(
                    fake, 
                    this.Container.Resolve<ExpressionCallMatcher.Factory>(),
                    this.Container.Resolve<FakeAsserter.Factory>());
            }
        }

        private class FakeConfigurationFactory : IFakeConfigurationFactory
        {
            public DictionaryContainer Container;

            public IFakeConfiguration<TFake> Create<TFake>(FakeObject fakeObject)
            {
                return new FakeConfiguration<TFake>(
                    fakeObject,
                    this.Container.Resolve<ExpressionCallRule.Factory>());
            }
        }

        private class RecordingCallRuleFactory : IRecordingCallRuleFactory
        {
            public DictionaryContainer Container;

            public RecordingCallRule<TFake> Create<TFake>(FakeObject fakeObject, RecordedCallRule recordedRule)
            {
                return new RecordingCallRule<TFake>(fakeObject, recordedRule, this.Container.Resolve<FakeAsserter.Factory>());
            }
        }
        #endregion
    }
}
