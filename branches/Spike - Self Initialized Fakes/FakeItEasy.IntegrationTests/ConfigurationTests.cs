using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.VisualBasic;
using FakeItEasy.Api;
using System.Xml.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void Faked_object_configured_to_call_back_performs_callback_when_called()
        {
            var foo = A.Fake<IFoo>();

            bool wasCalled = false;

            foo.Configure().CallsTo(x => x.Bar()).Invokes(x => wasCalled = true);
            
            foo.Bar();

            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void Faked_object_configured_to_perform_several_call_backs_and_return_value_does_all()
        {
            var foo = A.Fake<IFoo>();
            
            bool firstWasCalled = false;
            bool secondWasCalled = false;

            foo.Configure()
                .CallsTo(x => x.Baz())
                .Invokes(x => firstWasCalled = true)
                .Invokes(x => secondWasCalled = true)
                .Returns(10);

            var result = foo.Baz();

            Assert.That(firstWasCalled);
            Assert.That(secondWasCalled);
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_should_call_base_method()
        {
            var fake = A.Fake<BaseClass>();

            fake.DoSomething();

            Assert.That(fake.WasCalled, Is.False);

            fake.Configure().CallsTo(x => x.DoSomething()).CallsBaseMethod();
            fake.DoSomething();

            Assert.That(fake.WasCalled, Is.True);
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_should_return_value_from_base_method()
        {
            var fake = A.Fake<BaseClass>();

            fake.Configure().CallsTo(x => x.ReturnSomething()).CallsBaseMethod();
            
            Assert.That(fake.ReturnSomething(), Is.EqualTo(10));
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_invokes_configured_invokations_also()
        {
            var fake = A.Fake<BaseClass>();

            bool wasCalled = false;

            fake.Configure().CallsTo(x => x.ReturnSomething()).Invokes(x => wasCalled = true).CallsBaseMethod();

            Assert.That(wasCalled, Is.True);
        }

        public class BaseClass
        {
            public bool WasCalled;

            public virtual void DoSomething()
            {
                WasCalled = true;
            }

            public virtual int ReturnSomething()
            {
                return 10;
            }
        }
    }

    [TestFixture]
    public class AutoInitializedFakeTests
    {
        [Test]
        public void test_name()
        {
            var realService = new SomeService();
            realService.Responses.Add("foo", "one");
            realService.Responses.Add("bar", "two");

            using (var recorder = new XmlRecorder(XDocument.Parse("<calls>" +
  "<call type=\"System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\">one</call>" +
  "<call type=\"System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\">two</call>" +
"</calls>")))
            {
                var fakeWrapper = A.Fake(realService, recorder);
                var foo = fakeWrapper.MakeCall("foo");
                var bar = fakeWrapper.MakeCall("bar");

                Assert.That(foo, Is.EqualTo("one"));
                Assert.That(bar, Is.EqualTo("two"));
            }
                
        }

        public class SomeService
        {
            public bool Frozen = false;
            public Dictionary<string, string> Responses = new Dictionary<string,string>();

            public virtual string MakeCall(string request)
            {
                if (Frozen) throw new Exception();

                return Responses[request];
            }
        }

        private class InMemoryRecorder
            : IFakeCallRecorder
        {
            private List<ICompletedFakeObjectCall> recordedCalls = new List<ICompletedFakeObjectCall>();
            private IEnumerator<ICompletedFakeObjectCall> enumerator;

            public InMemoryRecorder()
            {
                this.IsRecording = true;
            }

            public bool IsRecording
            {
                get;
                private set;
            }

            public void RecordCall(FakeItEasy.Api.ICompletedFakeObjectCall call)
            {
                this.recordedCalls.Add(call);
            }

            public void ApplyNext(FakeItEasy.Api.IWritableFakeObjectCall call)
            {
                var recordedCall = this.GetNext();
                call.SetReturnValue(recordedCall.ReturnValue);
            }

            private ICompletedFakeObjectCall GetNext()
            {
                if (this.enumerator == null || !this.enumerator.MoveNext())
                {
                    this.enumerator = this.recordedCalls.GetEnumerator();
                    this.enumerator.MoveNext();
                }

                return this.enumerator.Current;
            }

            public void Dispose()
            {
                this.IsRecording = false;
            }
        }

        private class XmlRecorder
            : IFakeCallRecorder
        {
            public XDocument Xml;
            private IEnumerator<XElement> enumerator;

            public XmlRecorder(XDocument document)
            {
                this.Xml = document;
                this.IsRecording = false;
            }

            public XmlRecorder()
            {
                this.Xml = new XDocument();
                this.Xml.Add(new XElement("calls"));
                this.Xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
                this.IsRecording = true;
            }

            public bool IsRecording
            {
                get;
                private set;
            }

            public void RecordCall(ICompletedFakeObjectCall call)
            {
                var attribute = new XAttribute("type", call.ReturnValue.GetType().AssemblyQualifiedName);
                var element = new XElement("call", attribute, Serialize(call.ReturnValue));
                
                this.Xml.Root.Add(element);
            }

            private string Serialize(object value)
            {
                if (value == null) return "";

                var converter = TypeDescriptor.GetConverter(value.GetType());
                return converter.ConvertToInvariantString(value);
            }

            private object Deserialize(XElement element)
            {
                var typeAttribute = element.FirstAttribute;
                var type = Type.GetType(typeAttribute.Value);

                var converter = TypeDescriptor.GetConverter(type);

                return converter.ConvertFromInvariantString(element.Value);
            }

            public void ApplyNext(IWritableFakeObjectCall call)
            {
                if (this.enumerator == null || !this.enumerator.MoveNext())
                {
                    this.enumerator = this.Xml.Root.Elements().GetEnumerator();
                    this.enumerator.MoveNext();
                }

                call.SetReturnValue(Deserialize(this.enumerator.Current));
            }

            public void Dispose()
            {
                Console.WriteLine(Xml.ToString());
            }
        }
    }
}
