using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FakeItEasy.Api
{
    public partial class FakeObject
    {
        [Serializable]
        private class ObjectMemberRule
            : IFakeObjectCallRule
        {
            private static readonly MethodInfo toString = typeof(FakeGenerator.ICanInterceptObjectMembers).GetMethod("ToString");
            private static readonly MethodInfo getHashCode = typeof(FakeGenerator.ICanInterceptObjectMembers).GetMethod("GetHashCode");
            private static readonly MethodInfo equals = typeof(FakeGenerator.ICanInterceptObjectMembers).GetMethod("Equals", new[] { typeof(object) });

            public FakeObject FakeObject;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return IsObjetMethod(fakeObjectCall);
            }

            private static bool IsObjetMethod(IFakeObjectCall fakeObjectCall)
            {
                var baseDefinition = fakeObjectCall.Method.GetBaseDefinition();

                return baseDefinition.Equals(toString)
                    || baseDefinition.Equals(getHashCode)
                    || baseDefinition.Equals(equals);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                if (TryHandleToString(fakeObjectCall))
                {
                    return;
                }

                if (TryHandleGetHashCode(fakeObjectCall))
                {
                    return;
                }

                if (TryHandleEquals(fakeObjectCall))
                {
                    return;
                }
            }

            private bool TryHandleToString(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.Name.Equals("ToString"))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue("Faked {0}".FormatInvariant(this.FakeObject.FakeObjectType.FullName));

                return true;
            }

            private static bool TryHandleGetHashCode(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.Name.Equals("GetHashCode"))
                {
                    return false;
                }

                var fakeObject = Fake.GetFakeObject(fakeObjectCall.FakedObject);
                fakeObjectCall.SetReturnValue(fakeObject.GetHashCode());

                return true;
            }

            private bool TryHandleEquals(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.Name.Equals("Equals"))
                {
                    return false;
                }

                var argument = fakeObjectCall.Arguments[0] as IFakedObject;
                if (argument != null)
                {
                    fakeObjectCall.SetReturnValue(argument.GetFakeObject().Equals(this.FakeObject));
                }
                else
                {
                    fakeObjectCall.SetReturnValue(false);
                }

                return true;
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }
    }
}
