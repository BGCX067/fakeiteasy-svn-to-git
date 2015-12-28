using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ExpectationExceptionTests
        : ExceptionTestBase<ExpectationException>
    {
        protected override ExpectationException CreateException(string message)
        {
            return new ExpectationException(message);
        }
    }
}
