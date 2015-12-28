using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.ExtensionSyntax;
using Legend.Fakes.Extensibility;
using Legend.Fakes.VisualBasic;

namespace Legend.Fakes.Examples
{
    public class ConfiguringCalls
    {
        public void Setting_return_value()
        {
            var factory = A.Fake<IWidgetFactory>();

            // Configure method to always return the same widget
            factory.Configure().CallsTo(x => x.Create()).Returns(A.Fake<IWidget>());

            // Configure method to return a new widget each time it's called
            factory.Configure().CallsTo(x => x.Create()).Returns(() => A.Fake<IWidget>());

            // A call can be configured to only be valid a certain number of times.
            factory.Configure().CallsTo(x => x.Create()).Returns(A.Fake<IWidget>()).Twice();
        }

        public void Configure_call_to_throw_exception_when_called()
        {
            var factory = A.Fake<IWidgetFactory>();

            factory.Configure().CallsTo(x => x.Create()).Throws(new NotSupportedException("Can't create"));
        }

        public void Configure_only_calls_that_has_certain_argument_values()
        {
            var comparer = A.Fake<IComparer<string>>();

            // Using argument validators.
            comparer.Configure()
                .CallsTo(x => x.Compare(Argument.Is.Any<string>(), Argument.Is.Match<string>(s => s == "must be equal to this")))
                .Returns(-1);

            // Using specific argument values.
            comparer.Configure()
                .CallsTo(x => x.Compare("a", "b"))
                .Returns(0);

            // Mixing actual values and validators.
            comparer.Configure()
                .CallsTo(x => x.Compare("a", Argument.Is.Any<string>()))
                .Returns(1);
        }

        public void Configure_only_calls_that_has_certain_argument_values_using_a_custom_argument_validator()
        {
            var comparer = A.Fake<IComparer<string>>();

            // Using the "CustomArgumentValidators.LongerThan" argument validator defined
            // in this project is seamless since it's just an extension method.
            comparer.Configure()
                .CallsTo(x => x.Compare(Argument.Is.LongerThan(10), Argument.Is.Any<string>()))
                .Returns(-1);
        }
    }
}