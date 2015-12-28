namespace FakeItEasy.Api
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using FakeItEasy.Configuration;

    /// <summary>
    /// A call rule that has been recorded.
    /// </summary>
    internal class PredicateCallRule
        : BuildableCallRule
    {
        public MethodInfo ApplicableToMethod { get; set; }
        public Func<ArgumentCollection, bool> IsApplicableToArguments { get; set; }
    
        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            Debug.WriteLine(this.IsApplicableToArguments(fakeObjectCall.Arguments).ToString());


            return FakeItEasy.Expressions.ExpressionCallMatcher.WillInvokeSameMethodWhenCalledOnTarget(fakeObjectCall.FakedObject, fakeObjectCall.Method, this.ApplicableToMethod)
                && this.IsApplicableToArguments(fakeObjectCall.Arguments);
        }
    }
}
