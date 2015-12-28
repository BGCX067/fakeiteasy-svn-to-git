﻿namespace FakeItEasy
{
    using System.IO;
    using FakeItEasy.Api;
    using FakeItEasy.Assertion;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    using FakeItEasy.SelfInitializedFakes;
    using System.Diagnostics;
    using System;
    
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

            container.RegisterSingleton<IExpressionCallMatcherFactory>(c =>
                new ExpressionCallMatcherFactory { Container = c });

            container.RegisterSingleton<ArgumentValidatorFactory>(c => 
                new ArgumentValidatorFactory());

            container.RegisterSingleton<ExpressionCallRule.Factory>(c =>
                callSpecification => new ExpressionCallRule(new ExpressionCallMatcher(callSpecification, c.Resolve<ArgumentValidatorFactory>(), c.Resolve<MethodInfoManager>())));

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

            container.Register<IFakeObjectGeneratorFactory>(c =>
                c.Resolve<FakeObjectFactory>());
        }

        #region FactoryImplementations

        private class ExpressionCallMatcherFactory
            : IExpressionCallMatcherFactory
        {
            public DictionaryContainer Container;

            public ICallMatcher CreateCallMathcer(System.Linq.Expressions.LambdaExpression callSpecification)
            {
                return new ExpressionCallMatcher(
                    callSpecification,
                    this.Container.Resolve<ArgumentValidatorFactory>(),
                    this.Container.Resolve<MethodInfoManager>());
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
                    this.Container.Resolve<IExpressionCallMatcherFactory>(),
                    this.Container.Resolve<FakeAsserter.Factory>());
            }
        }
        #endregion
    }
}
