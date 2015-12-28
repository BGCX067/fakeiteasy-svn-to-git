using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
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

        public virtual string VirtualProperty
        {
            get;
            set;
        }

        internal virtual IFoo InternalVirtualFakeableProperty
        {
            get;
            set;
        }

        public virtual event EventHandler SomethingHappened;
    }
}
