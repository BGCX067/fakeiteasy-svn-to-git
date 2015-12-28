using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legend.Fakes.Api
{
    public partial class FakeObject
    {
        [Serializable]
        private class DefaultReturnValueRule
            : IFakeObjectCallRule
        {
            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return true;
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                var returnValue = Helpers.GetDefaultValueOfType(fakeObjectCall.Method.ReturnType);
                fakeObjectCall.SetReturnValue(returnValue);
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }
    }
}
