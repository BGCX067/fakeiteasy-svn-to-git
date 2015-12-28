namespace FakeItEasy
{
    using System.IO;
    using FakeItEasy.Api;
    using FakeItEasy.Assertion;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    using FakeItEasy.SelfInitializedFakes;
    
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

            container.Register<IFakeObjectContainer>(c =>
                c.Resolve<FakeScope>().FakeObjectContainer);

            container.RegisterSingleton<IProxyGenerator>(c =>
                new FakeItEasy.DynamicProxy.DynamicProxyProxyGenerator());
            
            container.RegisterSingleton<ExpressionCallMatcher.Factory>(c =>
                expression => new ExpressionCallMatcher(expression, c.Resolve<ArgumentValidatorFactory>(), c.Resolve<MethodInfoManager>()));

            container.RegisterSingleton<ArgumentValidatorFactory>(c => 
                new ArgumentValidatorFactory());

            container.RegisterSingleton<ExpressionCallRule.Factory>(c =>
                callSpecification => new ExpressionCallRule(c.Resolve<ExpressionCallMatcher.Factory>().Invoke(callSpecification)));

            container.RegisterSingleton<ICallCollectionFactory>(c =>
                new CallCollectionFactory { Container = c });

            container.RegisterSingleton<IFakeAssertionsFactory>(c =>
                new FakeAssertionsFactory { Container = c });

            container.RegisterSingleton<MethodInfoManager>(c =>
                new MethodInfoManager());

            container.RegisterSingleton<FakeAsserter.Factory>(c =>
                x => new FakeAsserter(x, c.Resolve<CallWriter>()));

            container.Register<FakeObjectFactory>(c =>
                new FakeObjectFactory(c.Resolve<IFakeObjectContainer>(), c.Resolve<IProxyGenerator>(), c.Resolve<FakeObject.Factory>()));

            container.RegisterSingleton<FakeObject.Factory>(c =>
                () => new FakeObject());

            container.RegisterSingleton<CallWriter>(c =>
                new CallWriter());

            container.RegisterSingleton<RecordingManager.Factory>(c =>
                x => new RecordingManager(x));

            container.RegisterSingleton<IFileSystem>(c =>
                new FileSystem());

            container.RegisterSingleton<FileStorage.Factory>(c =>
                x => new FileStorage(x, c.Resolve<IFileSystem>()));

            container.RegisterSingleton<IExpressionParser>(c =>
                new ExpressionParser());

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

        private class FileSystem : IFileSystem
        {
            public System.IO.Stream Open(string fileName, System.IO.FileMode mode)
            {
                return File.Open(fileName, mode);
            }

            public bool FileExists(string fileName)
            {
                return File.Exists(fileName);
            }

            public void Create(string fileName)
            {
                File.Create(fileName).Dispose();
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

       


        #endregion
    }
}
