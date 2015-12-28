using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.ExtensionSyntax;

namespace Legend.Fakes.Examples
{
    public class CreatingFakeObjects
    {
        public void Creating_an_interface_fake()
        {
            var foo = A.Fake<IWidgetFactory>();
        }

        public void Creating_a_fake_for_a_class_that_takes_constructor_arguments()
        {
            // Not that the arguments for the constructor is specified as
            // an expression rather than an object array and is "refactor friendly".
            var fake = A.Fake<ClassThatTakesConstructorArguments>(() =>
                new ClassThatTakesConstructorArguments(A.Fake<IWidgetFactory>(), "name"));
        }

        public void Properties_of_types_that_can_be_faked_are_set_to_fake_objects_by_default()
        {
            var factory = A.Fake<IWidgetFactory>();

            // The following call is legal since by default property values are set
            // to fake objects.
            factory.SubFactory.Configure().CallsTo(x => x.Create()).Returns(A.Fake<IWidget>());
        }
    }
}
