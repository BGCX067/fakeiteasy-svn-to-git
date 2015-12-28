using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.Extensibility;
using System.Diagnostics;

namespace FakeItEasy.Examples
{
    public class Asserting
    {
        public void Asserting_on_calls()
        {
            var factory = A.Fake<IWidgetFactory>();

            // This would throw an exception since no call has been made.
            Fake.Assert(factory).WasCalled(x => x.Create());

            // This on the other hand would not throw.
            Fake.Assert(factory).WasNotCalled(x => x.Create());

            // To constraint the number of times a method must have been called pass a lambda that evaluates the repeat:
            Fake.Assert(factory).WasCalled(x => x.Create(), repeat => repeat > 1);

            Fake.Assert(factory).WasCalled(x => x.Create(), repeat => repeat <= 10);
        }
    }
}