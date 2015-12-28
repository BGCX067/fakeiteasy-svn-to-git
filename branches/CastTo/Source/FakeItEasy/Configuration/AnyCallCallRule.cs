namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Api;

    internal class AnyCallCallRule
        : BuildableCallRule
    {
        private Func<ArgumentCollection, bool> argumentsPredicate;

        public AnyCallCallRule()
        {
            this.argumentsPredicate = x => true;
        }

        public Type ApplicableToMembersWithReturnType
        {
            get; 
            set;
        }

        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return 
                this.argumentsPredicate(fakeObjectCall.Arguments) &&        
                (this.ApplicableToMembersWithReturnType == null || this.ApplicableToMembersWithReturnType.Equals(fakeObjectCall.Method.ReturnType));
        }

        public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            this.argumentsPredicate = argumentsPredicate;   
        }

        public override string ToString()
        {
            if (this.ApplicableToMembersWithReturnType != null)
            {
                return "Any call with return type {0} to the fake object.".FormatInvariant(this.ApplicableToMembersWithReturnType.FullName);
            }

            return "Any call made to the fake object.";
        }
    }
}
