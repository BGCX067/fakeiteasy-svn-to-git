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
                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method) && TypeIsFakable(fakeObjectCall.Method.ReturnType);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                var newRule = new CallRuleMetadata
                {
                    Rule = new PropertyBehaviorRule(fakeObjectCall.Method, FakeObject) { Value = this.Factory.CreateFake(fakeObjectCall.Method.ReturnType, null, true) },
                    CalledNumberOfTimes = 1
                };

                this.FakeObject.allUserRulesField.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }

            private FakeObjectFactory Factory
            { 
                get
                {
                    return ServiceLocator.Current.Resolve<FakeObjectFactory>();
                }
            }

            private bool TypeIsFakable(Type type)
            {
                try
                {
                    this.Factory.CreateFake(type, null, true);
                    return true;
                }
                catch (ArgumentException ex) // TODO: this is a superhack, the fake object factory should be changed to TryCreate.
                {
                    return false;
                }
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }
    }
}
