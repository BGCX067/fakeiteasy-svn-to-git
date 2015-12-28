using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using System.Reflection;
using System.Diagnostics;

namespace FakeItEasy.Api
{
    internal class RecordedCallRule
        : BuildableCallRule
    {
        public MethodInfo ApplicableToMethod { get; set; }
        public Func<ArgumentCollection, bool> IsApplicableToArguments { get; set; }


        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            Debug.WriteLine(this.IsApplicableToArguments(fakeObjectCall.Arguments).ToString());


            return fakeObjectCall.Method.Equals(this.ApplicableToMethod)
                && this.IsApplicableToArguments(fakeObjectCall.Arguments);
        }
    }
}
