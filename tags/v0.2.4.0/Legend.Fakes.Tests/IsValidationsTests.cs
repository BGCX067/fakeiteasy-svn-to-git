using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Legend.Fakes.Tests
{
    [TestFixture]
    public class IsValidationsTests
    {
        [Test]
        public void NotNull_should_print_correctly_in_exception_message()
        {
            var foo = A.Fake<IFoo>();

            try
            {
                Fake.Assert(foo).WasCalled(x => x.Bar(Argument.Is.NotNull<object>()));
            }
            catch (ExpectationException ex)
            {
                Assert.That(ex, Has.Message.Contains("<NOT NULL>"));
            }
        }
    }
}
