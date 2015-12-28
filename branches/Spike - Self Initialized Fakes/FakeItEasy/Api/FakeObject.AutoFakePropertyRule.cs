using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FakeItEasy.Api
{
    public partial class FakeObject
    {
        [Serializable]
        private class AutoFakePropertyRule
            : IFakeObjectCallRule
        {
            public FakeObject FakeObject;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method) && TypeIsFakeable(fakeObjectCall.Method.ReturnType);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                var newRule = new CallRuleMetadata
                {
                    Rule = new PropertyBehaviorRule(fakeObjectCall.Method) { Value = new FakeObject(fakeObjectCall.Method.ReturnType).Object },
                    CalledNumberOfTimes = 1
                };

                newRule.Rule.Apply(fakeObjectCall);
                this.FakeObject.allUserRulesField.AddFirst(newRule);
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            private static bool TypeIsFakeable(Type type)
            {
                if (type.IsSealed)
                {
                    return false;
                }

                if (!type.IsAbstract && !type.IsInterface)
                {
                    var defaultConstructor =
                        from constructor in type.GetConstructors()
                        where constructor.GetParameters().Length == 0
                        select constructor;

                    if (defaultConstructor.Count() < 1)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
