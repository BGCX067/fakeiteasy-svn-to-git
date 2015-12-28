using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.Extensibility;
using FakeItEasy.VisualBasic;

namespace FakeItEasy.Examples
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
                .CallsTo(x => x.Compare(A<string>.Ignored, A<string>.That.Matches(s => s == "must be equal to this")))
                .Returns(-1);

            // Using specific argument values.
            comparer.Configure()
                .CallsTo(x => x.Compare("a", "b"))
                .Returns(0);

            // Mixing actual values and validators.
            comparer.Configure()
                .CallsTo(x => x.Compare("a", A<string>.Ignored))
                .Returns(1);
        }

        public void Configure_only_calls_that_has_certain_argument_values_using_a_custom_argument_validator()
        {
            var comparer = A.Fake<IComparer<string>>();

            // Using the "CustomArgumentValidators.LongerThan" argument validator defined
            // in this project is seamless since it's just an extension method.
            comparer.Configure()
                .CallsTo(x => x.Compare(A<string>.That.IsLongerThan(10), A<string>.Ignored))
                .Returns(-1);
        }
    }
}