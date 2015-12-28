using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Legend.Fakes.VisualBasic;

namespace Legend.Fakes.Tests.VisualBasic
{
    [TestFixture]
    public class VisualBasicHelpersTests
    {
        [Test]
        public void With_any_arguments_should_configure_call_so_that_any_arguments_matches()
        {
            var fake = A.Fake<IFoo>();

            ThisCall.To(fake).WithAnyArguments().Throws(new ApplicationException());
            fake.Baz(null, null);

            Assert.Throws<ApplicationException>(() =>
                fake.Baz("foo", "bar"));
        }
    }
}
