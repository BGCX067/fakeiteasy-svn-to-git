namespace FakeItEasy
{
    using FakeItEasy.Api;
    using FakeItEasy.Assertion;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    
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
                expression => new ExpressionCallMatcher(expression, c.Resolve<ArgumentValidatorFactory>()));

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
        }

        #region FactoryImplementations
        private class CallCollectionFactory : ICallCollectionFactory
        {
            public DictionaryContainer Container;

            public CallCollection<TFake> CreateCallCollection<TFake>(FakeObject fake)
            {
                return new CallCollection<TFake>(fake, this.Container.Resolve<ExpressionCallMatcher.Factory>());
            }
        }

        private class FakeAssertionsFactory : IFakeAssertionsFactory
        {
            public DictionaryContainer Container;

            public IFakeAssertions<TFake> CreateAsserter<TFake>(FakeObject fake)
            {
                return new FakeAsserter<TFake>(
                    fake, 
                    this.Container.Resolve<ICallCollectionFactory>(), 
                    this.Container.Resolve<ExpressionCallMatcher.Factory>());
            }
        }
        #endregion
    }
}
