using System;
using System.Reflection;

namespace FakeItEasy.Api
{
    public partial class FakeObject
    {
        [Serializable]
        private class ObjectMemberRule
            : IFakeObjectCallRule
        {
            public FakeObject FakeObject;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return IsObjetMethod(fakeObjectCall);
            }

            private static bool IsObjetMethod(IFakeObjectCall fakeObjectCall)
            {
                var name = fakeObjectCall.Method.Name;

                return name.Equals("ToString") || name.Equals("GetHashCode") || name.Equals("Equals");
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

                var argument = fakeObjectCall.Arguments[0] as IFakedProxy;
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
