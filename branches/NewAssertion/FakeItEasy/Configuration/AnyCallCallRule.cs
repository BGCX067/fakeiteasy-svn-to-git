namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Api;

    internal class AnyCallCallRule
            : BuildableCallRule
    {
        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            throw new NotSupportedException();
        }
    }
}
