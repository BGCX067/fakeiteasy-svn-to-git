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
            A.CallTo(() => factory.Create()).MustHaveHappened();

            // This on the other hand would not throw.
            A.CallTo(() => factory.Create()).MustHaveHappened(repeat => repeat == 0);

            // To constraint the number of times a method must have been called pass a lambda that evaluates the repeat:
            A.CallTo(() => factory.Create()).MustHaveHappened(repeat => repeat > 1);
            A.CallTo(() => factory.Create()).MustHaveHappened(repeat => repeat <= 10);
        }
    }
}