﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.Api;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;
using System.Diagnostics;
using FakeItEasy.Expressions;
using FakeItEasy.Tests.TestHelpers;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class FakeObjectTests
    {
        private static readonly IFakeObjectCallRule NonApplicableInterception = new FakeCallRule { IsApplicableTo = x => false };

        private FakeObject CreateFakeObject<T>()
        {
            var factory = ServiceLocator.Current.Resolve<FakeObjectFactory>();
            var result = factory.CreateFake(typeof(T), null, false);

            return Fake.GetFakeObject(result);
        }

        [Test, Ignore]
        public void Calls_configured_in_a_child_context_does_not_exist_outside_that_context()
        {
            //var fake = this.CreateFakeObject<IFoo>();
            //var config = new FakeConfiguration<IFoo>(fake);
            
            //using (Fake.CreateScope())
            //{
            //    config.CallsTo(x => x.Baz()).Returns(10);
            //}

            //Assert.That(((IFoo)fake.Object).Baz(), Is.Not.EqualTo(10));
            throw new NotImplementedException();
        }

        [Test]
        public void Event_listeners_that_are_removed_should_not_be_invoked_when_event_is_raised()
        {
            var foo = A.Fake<IFoo>();
            var called = false;
            EventHandler listener = (s, e) => called = true;

            foo.SomethingHappened += listener;
            foo.SomethingHappened -= listener;

            foo.SomethingHappened += Raise.With(EventArgs.Empty).Now;
            
            Assert.That(called, Is.False);
        }

        [Test]
        public void Method_call_should_return_null_when_theres_no_matching_interception_and_return_type_is_reference_type()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var result = ((IFoo)fake.Object).Biz();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Method_call_should_return_default_value_when_theres_no_matching_interception_and_return_type_is_value_type()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var result = ((IFoo)fake.Object).Baz();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Method_call_should_not_set_return_value_when_theres_no_matching_interception_and_return_type_is_void()
        {
            var fake = this.CreateFakeObject<IFoo>();
            ((IFoo)fake.Object).Bar();
        }

        [Test]
        public void The_first_interceptor_should_be_applied_when_it_has_not_been_used()
        {
            var fake = this.CreateFakeObject<IFoo>();

            var interception = new FakeCallRule
            {
                IsApplicableTo = x => true
            };

            fake.AddRule(interception);

            // Act
            ((IFoo)fake.Object).Bar();

            Assert.That(interception.ApplyWasCalled, Is.True);
        }

        [Test]
        public void The_first_applicable_interceptor_should_be_called_when_it_has_not_been_used()
        {
            var fake = this.CreateFakeObject<IFoo>();

            var interception = new FakeCallRule
            {
                IsApplicableTo = x => true
            };

            fake.AddRule(NonApplicableInterception);
            fake.AddRule(interception);

            ((IFoo)fake.Object).Bar();

            Assert.That(interception.ApplyWasCalled, Is.True);
        }

        [Test]
        public void The_latest_added_rule_should_be_called_for_ever_when_no_number_of_times_is_specified()
        {
            var fake = this.CreateFakeObject<IFoo>();

            var firstRule = CreateApplicableInterception();
            var latestRule = CreateApplicableInterception();

            fake.AddRule(firstRule);
            fake.AddRule(latestRule);

            var foo = (IFoo)fake.Object;
            foo.Bar();
            foo.Bar();
            foo.Bar();

            Assert.That(firstRule.ApplyWasCalled, Is.False);
        }

        [Test]
        public void An_applicable_action_should_be_called_its_specified_number_of_times_before_the_next_applicable_action_is_called()
        {

            var fake = this.CreateFakeObject<IFoo>();

            var applicableTwice = new FakeCallRule
            {
                IsApplicableTo = x => true,
                NumberOfTimesToCall = 2
            };

            var nextApplicable = CreateApplicableInterception();

            fake.AddRule(nextApplicable);
            fake.AddRule(applicableTwice);
            
            ((IFoo)fake.Object).Bar();
            ((IFoo)fake.Object).Bar();
            Assert.That(nextApplicable.ApplyWasCalled, Is.False);

            ((IFoo)fake.Object).Bar();
            Assert.That(nextApplicable.ApplyWasCalled, Is.True);
        }

        [Test]
        public void DefaultValue_should_be_returned_when_the_last_applicable_action_has_been_used_its_specified_number_of_times()
        {
            var fake = this.CreateFakeObject<IFoo>();

            var applicableTwice = CreateApplicableInterception();
            applicableTwice.NumberOfTimesToCall = 2;
            applicableTwice.Apply = x => x.SetReturnValue(10);

            fake.AddRule(applicableTwice);

            ((IFoo)fake.Object).Baz();
            ((IFoo)fake.Object).Baz();

            var result = ((IFoo)fake.Object).Baz();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Interceptions_should_return_interceptions_that_are_added()
        {
            var fake = this.CreateFakeObject<IFoo>();

            var one = CreateApplicableInterception();
            var two = CreateApplicableInterception();

            fake.AddRule(one);
            fake.AddRule(two);

            Assert.That(fake.Rules, Is.EquivalentTo(new[] { one, two }));
        }

        [Test]
        public void Configure_should_return_fake_configuration_object()
        {
            var fake = A.Fake<IFoo>();

            var configuration = (FakeConfiguration<IFoo>)Fake.Configure(fake);

            Assert.That(configuration.Fake.Object, Is.SameAs(fake));
        }

        [Test]
        public void Configure_should_throw_when_object_is_not_a_fake_object()
        {
            Assert.Throws<ArgumentException>(() =>
                Fake.Configure("non fake object"));
        }

        [Test]
        public void RecordedCalls_contains_all_calls_made_on_the_fake()
        {
            var fake = this.CreateFakeObject<IFoo>();

            ((IFoo)fake.Object).Bar();
            var i = ((IFoo)fake.Object)[1];

            Assert.That(fake.RecordedCallsInScope, Has.Some.Matches<IWritableFakeObjectCall>(x => x.Method.Name == "Bar"));
            Assert.That(fake.RecordedCallsInScope, Has.Some.Matches<IWritableFakeObjectCall>(x => x.Method.Name == "get_Item"));
        }

        [Test]
        public void RecordedCalls_only_returns_calls_made_within_the_scope()
        {
            var foo = A.Fake<IFoo>();

            foo.Baz();

            using (Fake.CreateScope())
            {
                foo.Baz();

                Assert.That(Fake.GetCalls(foo).Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void RecordedCalls_returns_calls_made_in_scope_and_any_inner_scopes()
        {
            var foo = A.Fake<IFoo>();
            
            foo.Baz();

            using (Fake.CreateScope())
            {
                foo.Baz();

                using (Fake.CreateScope())
                {
                    foo.Baz();
                }
            }

            Assert.That(Fake.GetCalls(foo).Count(), Is.EqualTo(3));
        }

        [Test]
        public void Rules_should_only_be_valid_within_the_current_scope()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var rule = A.Fake<IFakeObjectCallRule>();
            Fake.Configure(rule)
                .CallsTo(x => x.IsApplicableTo(Argument.Is.Any<IFakeObjectCall>()))
                .Returns(true);

            using (Fake.CreateScope())
            {
                fake.AddRule(rule);
            }

            (fake.Object as IFoo).Bar();

            Fake.Assert(rule).WasNotCalled(x => x.Apply(Argument.Is.Any<IWritableFakeObjectCall>()));
        }

        public class TypeWithNoDefaultConstructorButAllArgumentsFakeable
        {
            public IFoo Foo;
            public IFormatProvider FormatProvider;

            public TypeWithNoDefaultConstructorButAllArgumentsFakeable(IFoo foo, IFormatProvider formatProvider)
            {
                this.Foo = foo;
                this.FormatProvider = formatProvider;
            }
        }

        private static void AddFakeRule<T>(T fakedObject, Action<FakeCallRule> ruleConfiguration) where T : class
        {
            var rule = new FakeCallRule();
            ruleConfiguration(rule);

            AddFakeRule(fakedObject, rule);
        }

        private static void AddFakeRule<T>(T fakedObject, FakeCallRule rule) where T : class
        {
            Fake.GetFakeObject(fakedObject).AddRule(rule);
        }


        //[Test]
        //public void Assert_returns_fake_asserter_with_faked_object_set_when_called_with_faked_object()
        //{
        //    var foo = A.Fake<IFoo>();

        //    var asserter = (FakeAssertions<IFoo>)Fake.Assert(foo);

        //    Assert.That(asserter.Fake.Object, Is.SameAs(foo));
        //}

        [Test]
        public void Object_properties_has_property_behavior_when_not_configured()
        {
            var foo = A.Fake<IFoo>();

            foo.SomeProperty = 10;

            Assert.That(foo.SomeProperty, Is.EqualTo(10));
        }

        [Test]
        public void Object_properties_be_set_directly_and_configured_as_methods_interchangeably()
        {
            var foo = A.Fake<IFoo>();

            Fake.Configure(foo).CallsTo(x => x.SomeProperty).Returns(2);
            Assert.That(foo.SomeProperty, Is.EqualTo(2));

            foo.SomeProperty = 5;
            Assert.That(foo.SomeProperty, Is.EqualTo(5));

            Fake.Configure(foo).CallsTo(x => x.SomeProperty).Returns(20);
            Assert.That(foo.SomeProperty, Is.EqualTo(20));

            foo.SomeProperty = 10;
            Assert.That(foo.SomeProperty, Is.EqualTo(10));
        }

        [Test]
        public void Properties_should_be_set_to_fake_objects_when_property_type_is_fakeable_and_the_property_is_not_explicitly_set()
        {
            var foo = A.Fake<IFoo>();

            Fake.Configure(foo.ChildFoo).CallsTo(x => x.Baz()).Returns(10);

            Assert.That(foo.ChildFoo.Baz(), Is.EqualTo(10));
        }

        [Test]
        public void Return_value_can_be_read_back_from_recorded_calls()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var foo = (IFoo)fake.Object;

            var config = this.CreateConfiguration<IFoo>(fake);
            config.CallsTo(x => x.Baz()).Returns(10);

            foo.Baz();

            Assert.That(fake.RecordedCallsInScope.Single().ReturnValue, Is.EqualTo(10));
        }

        [Test]
        public void GetHashCode_should_be_configurable()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var foo = (IFoo)fake.Object;

            this.CreateConfiguration<IFoo>(fake).CallsTo(x => x.GetHashCode()).Returns(10);

            Assert.That(foo.GetHashCode(), Is.EqualTo(10));
        }

        [Test]
        public void GetHashCode_on_faked_object_should_return_hash_code_of_fake_when_not_configured()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var foo = (IFoo)fake.Object;

            Assert.That(foo.GetHashCode(), Is.EqualTo(fake.GetHashCode()));
        }

        [Test]
        public void Equals_on_faked_object_should_return_true_if_the_passed_in_object_is_the_same_and_it_is_not_configured()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var foo = (IFoo)fake.Object;

            Assert.That(foo.Equals(foo));
        }

        [Test]
        public void Equals_on_faked_object_should_return_false_when_passed_in_object_is_not_the_same_and_it_is_not_configured()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var foo = (IFoo)fake.Object;

            Assert.That(foo.Equals("Something else"), Is.False);
        }

        [Test]
        public void Equals_on_faked_object_should_be_configurable()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var foo = (IFoo)fake.Object;

            this.CreateConfiguration<IFoo>(fake).CallsTo(x => x.Equals(Argument.Is.Any<object>())).Returns(true);

            Assert.That(foo.Equals("something"), Is.True);
        }

        [Test]
        public void ToString_should_be_interceptable()
        {
            var fake = this.CreateFakeObject<IFoo>();
            var foo = (IFoo)fake.Object;

            this.CreateConfiguration<IFoo>(fake).CallsTo(x => x.ToString()).Returns("foo");

            Assert.That(foo.ToString(), Is.EqualTo("foo"));
        }

        [Test]
        public void ToString_should_return_fake_of_type_when_not_configured()
        {
            var fake = this.CreateFakeObject<IFoo>();
            
            Assert.That(fake.Object.ToString(), Is.EqualTo("Faked FakeItEasy.Tests.IFoo"));
        }

        [Test]
        public void SetProxy_should_set_proxy_from_proxy_result()
        {
            var fake = this.CreateFakeObject<IFoo>();

            var proxy = this.CreateProxyResult<IFoo>();

            fake.SetProxy(proxy);

            Assert.That(fake.Object, Is.SameAs(proxy.Proxy));
        }

        [Test]
        public void SetProxy_should_set_the_fake_type_from_the_proxy()
        {
            var fake = this.CreateFakeObject<IFoo>();

            var proxy = this.CreateProxyResult<IFoo>();

            fake.SetProxy(proxy);

            Assert.That(fake.FakeObjectType, Is.EqualTo(typeof(IFoo)));
        }

        [Test]
        public void SetProxy_should_configure_fake_object_to_intercept_calls()
        {
            bool wasCalled = false;
            
            var fake = this.CreateFakeObject<IFoo>();
            var proxy = this.CreateProxyResult<IFoo>();
            var call = A.Fake<IWritableFakeObjectCall>();
            call.Configure().CallsTo(x => x.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            fake.SetProxy(proxy);

            var rule = A.Fake<IFakeObjectCallRule>();
            rule.Configure()
                .CallsTo(x => x.IsApplicableTo(call))
                .Returns(true);

            rule.Configure()
                .CallsTo(x => x.Apply(call))
                .Invokes(x => wasCalled = true);
            
            fake.AddRule(rule);

            proxy.RaiseCallWasIntercepted(call);

           
            Assert.That(wasCalled, Is.True);
        }

        //[Test]
        //public void CreateFake_returns_serializable_object()
        //{
        //    var factory = this.CreateFactory();
        //    Assert.That(factory.CreateFake(typeof(IFoo), null, false), Is.BinarySerializable);
        //}


       
        //[Test]
        //public void CreateFake_object_is_not_serializable_once_it_has_been_configured()
        //{
        //    var fake = this.CreateFakeObject<IFoo>();
            
            
        //    var factory = this.CreateFactory();

        //    var foo = factory.CreateFake(typeof(IFoo), null, false) as IFoo;

        //    Fake.Configure(foo).CallsTo(x => x.Bar()).Returns("test");

        //    Assert.That(foo, Is.Not.BinarySerializable);
        //}

        //[Test]
        //public void CreateFake_should_return_null_when_a_reference_type_member_that_has_not_been_configured_is_called()
        //{
        //    var foo = A.Fake<IFoo>();

        //    Assert.That(foo.Bar(), Is.Null);
        //}

        //[Test]
        //public void CreateFake_should_return_default_value_when_a_value_type_member_that_has_not_been_configured_is_called()
        //{
        //    var foo = A.Fake<IFoo>();

        //    Assert.That(foo.Count, Is.EqualTo(0));
        //}

        private FakeConfiguration<TFake> CreateConfiguration<TFake>(FakeObject fake)
        {
            return new FakeConfiguration<TFake>(fake, ServiceLocator.Current.Resolve<ExpressionCallRule.Factory>());
        }

        private static FakeCallRule CreateApplicableInterception()
        {
            return new FakeCallRule
            {
                IsApplicableTo = x => true
            };
        }

        private TestableProxyResult CreateProxyResult<T>()
        {
            return new TestableProxyResult(typeof(T), (IFakedProxy)A.Fake<T>());
        }
    }
}
