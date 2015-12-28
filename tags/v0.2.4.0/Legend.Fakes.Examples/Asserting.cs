using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.ExtensionSyntax;
using Legend.Fakes.Extensibility;
using System.Diagnostics;

namespace Legend.Fakes.Examples
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

            // For more advanced assertions, use your favourite unit testing framework
            // Was called...
            Debug.Assert(Fake.GetCalls(factory).Matching(x => x.Create()).Count() > 0);
            // Was called more than ten times...
            Debug.Assert(Fake.GetCalls(factory).Matching(x => x.Create()).Count() > 10);
            // Was called 5 or fewer times...
            Debug.Assert(Fake.GetCalls(factory).Matching(x => x.Create()).Count() <= 5);
        }
    }
}
