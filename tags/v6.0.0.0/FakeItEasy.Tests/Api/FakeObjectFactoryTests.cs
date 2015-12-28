using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.Api;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;
using FakeItEasy.Tests.TestHelpers;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class FakeObjectFactoryTests
    {
        IFakeObjectContainer container;
        IProxyGenerator proxyGenerator;
        FakeObject.Factory fakeObjectFactory;
        FakeObject fakeObject;

        [SetUp]
        public void SetUp()
        {
            this.container = A.Fake<IFakeObjectContainer>();
            this.proxyGenerator = A.Fake<IProxyGenerator>();
            this.fakeObject = A.Fake<FakeObject>();
            this.fakeObjectFactory = () => this.fakeObject;
        }

        private FakeObjectFactory CreateFactory()
        {
            return new FakeObjectFactory(this.container, this.proxyGenerator, this.fakeObjectFactory);
        }

        [Test]
        public void CreateFake_should_return_fake_from_container_when_non_proxyied_objects_are_allowed_and_container_contains_type()
        {
            var factory = this.CreateFactory();

            object result = null;
            Fake.Configure(this.container)
                .CallsTo(x => x.TryCreateFakeObject(typeof(int), Argument.Is.SameSequence<object>("foo"), out result))
                .Returns(true)
                .AssignsOutAndRefParameters(1);

            Assert.That(factory.CreateFake(typeof(int), new object[] { "foo" }, true), Is.EqualTo(1));
        }

        [Test]
        public void CreateFake_should_return_fake_from_proxy_generator_when_container_contains_type_but_non_proxied_objects_are_not_allowed()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            Fake.Configure(this.container)
                .CallsTo(x => x.TryCreateFakeObject(typeof(IFoo), null, out Null<object>.Out))
                .Returns(true)
                .AssignsOutAndRefParameters(A.Fake<IFoo>());

            Fake.Configure(this.proxyGenerator)
                .CallsTo(x => x.TryGenerateProxy(typeof(IFoo), Argument.Is.Any<FakeObject>(), this.container, out Null<ProxyResult>.Out))
                .Returns(true)
                .AssignsOutAndRefParameters(returned);


            Assert.That(factory.CreateFake(typeof(IFoo), null, false), Is.SameAs(returned.Proxy));
        }

        [Test]
        public void CreateFake_should_return_fake_from_proxy_generator_when_container_does_not_contain_type()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            Fake.Configure(this.proxyGenerator)
                .CallsTo(x => x.TryGenerateProxy(typeof(IFoo), Argument.Is.Any<FakeObject>(), this.container, out Null<ProxyResult>.Out))
                .Returns(true)
                .AssignsOutAndRefParameters(returned);


            Assert.That(factory.CreateFake(typeof(IFoo), null, true), Is.SameAs(returned.Proxy));
        }

        [Test]
        public void CreateFake_should_set_generated_proxy_to_fake_object()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            Fake.Configure(this.proxyGenerator)
                .CallsTo(x => x.TryGenerateProxy(typeof(IFoo), Argument.Is.Any<FakeObject>(), Argument.Is.SameSequence<object>("foo"), out Null<ProxyResult>.Out))
                .Returns(true)
                .AssignsOutAndRefParameters(returned);

            factory.CreateFake(typeof(IFoo), new object[] { "foo" }, false);

            Fake.Assert(this.fakeObject)
                .WasCalled(x => x.SetProxy(returned));
        }

        [Test]
        public void CreateFake_should_throw_exception_when_fake_cant_be_resolved_from_container_or_generated()
        {
            var factory = this.CreateFactory();

            var thrown = Assert.Throws<ArgumentException>(() =>
                factory.CreateFake(typeof(IFoo), null, true));

            Assert.That(thrown.Message, Is.EqualTo("Can not create fake of the type 'FakeItEasy.Tests.IFoo', it's not registered in the current container and the current IProxyGenerator can not generate the fake."));
        }
    }
}
