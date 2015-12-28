using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy
{
    class SyntaxTests
    {
        public void Test()
        {
            var foo = A.Fake<Foo>();

            
            
            Fake.Configure(foo).AnyCall().Throws(new ArgumentNullException());

            Fake.Configure(foo)
                .CallsTo(x => x.Bar())
                .Returns("1");

            Fake.Configure(foo)
                .CallsTo(x => x.Bar())
                .Returns("foo");

            Fake.Configure(foo)
                .CallsTo(x => x.Baz())
                .Throws(new Exception());

            Fake.Configure(foo)
                .CallsTo(x => x.Bar())
                .Returns("test")
                .Twice();

            
            Fake.Configure(foo)
                .CallsTo(x => x.Baz())
                .Throws(new ArgumentNullException("test"));

            
            Fake.Configure(foo)
                .CallsTo(x => x.Bar())
                .Returns(() => "test");

            //Fake.Configure(foo).CallsTo(x => x.Baz()).RemoveCallConfigurations();
            //Fake.Configure(foo).RemoveFakeObjectConfigurations();

            //// Expectations
            Fake.Configure(foo)
                .CallsTo(x => x.Bar())
                .Returns("test")
                .Once();

            Fake.Configure(foo)
                .CallsTo(x => x.Baz())
                .Throws(new ArgumentNullException())
                .Twice();

            

            Fake.Configure(foo).CallsTo(x => x.Bar()).Throws(new ArgumentException()).NumberOfTimes(2);

            Fake.Configure(foo)
                .CallsTo(x => x.Bar(Argument.Is.Any<string>(), Argument.Is.Matching<int>(p => p > 10)))
                .Returns("foo");
            

            foo = A.Fake<Foo>();
            //Fake.Configure(foo).Event(x => x.SomethingHappened += null)
            //foo.SomethingHappened += new EventHandler(foo_SomethingHappened);
            //Fake.VerifySetExpectations(foo);
            
            //Fake.Configure(foo).Event(x => x.SomethingHappened += null).Throws(new ArgumentNullException());
            //Fake.Configure(foo).Event(x => x.SomethingHappened += null).Raise(sender, EventArgs.Empty);

            //var t = foo.SomethingHappened;
            //foo.SomethingHappened += new EventManager<EventArgs>().Raise;
            //var raiser = Raise.With(foo, EventArgs.Empty);
            //foo.SomethingHappened += raiser.Now;
            //var f = foo.SomethingHappened;
            //foo.SomethingHappened += Raise.With(foo, EventArgs.Empty).Now;
            //foo.SomethingHappened += Raise.With(EventArgs.Empty).Now;
        }
    }

    public class Foo
    {
        public IServiceProvider ServiceProvider;

        public Foo(IServiceProvider provider)
        {
            this.ServiceProvider = provider;
        }

        public Foo()
        { }

        public virtual void Baz()
        {
            throw new NotImplementedException();
        }

        public virtual string Bar()
        {
            throw new NotImplementedException();
        }

        public virtual string Bar(string baz, int lorem)
        {
            throw new NotImplementedException();
        }

        public virtual event EventHandler SomethingHappened;
    }
}
