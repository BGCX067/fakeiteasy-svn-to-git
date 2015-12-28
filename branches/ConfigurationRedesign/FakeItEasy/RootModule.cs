namespace FakeItEasy
{
    using FakeItEasy.Api;
    using FakeItEasy.Assertion;
    using FakeItEasy.Configuration;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    using FakeItEasy.VisualBasic;
    using FakeItEasy.SelfInitializedFakes;
    using System.IO;
    
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

            container.RegisterSingleton<RecordedCallRule.Factory>(c =>
                () => new RecordedCallRule(c.Resolve<MethodInfoManager>()));

            container.RegisterSingleton<IRecordingCallRuleFactory>(c =>
                new RecordingCallRuleFactory() { Container = c });

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

            container.RegisterSingleton<IConfigurationFactory>(c =>
                new ConfigurationFactory());

            container.RegisterSingleton<IStartConfigurationFactory>(c =>
                new StartConfigurationFactory() { Container = c });
            
        }

        #region FactoryImplementations
        private class StartConfigurationFactory : IStartConfigurationFactory
        {
            public DictionaryContainer Container;

            public IFakeConfiguration<TFake> CreateConfiguration<TFake>(FakeObject fakeObject)
            {
                return new StartConfiguration<TFake>(fakeObject, this.Container.Resolve<ExpressionCallRule.Factory>(), this.Container.Resolve<IConfigurationFactory>()); 
            }
        }

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
            public IVoidArgumentValidationConfiguration CreateConfiguration(FakeObject fakeObject, BuildableCallRule callRule)
            {
                return new FakeConfiguration(fakeObject, callRule);
            }

            public IReturnValueArgumentValidationConfiguration<TMember> CreateConfiguration<TMember>(FakeObject fakeObject, BuildableCallRule callRule)
            {
                var parent = new FakeConfiguration(fakeObject, callRule);
                var configuration = new FakeConfiguration.ReturnValueConfiguration<TMember>();
                configuration.ParentConfiguration = parent;
                return configuration;
            }
        }

        #endregion
    }
}
