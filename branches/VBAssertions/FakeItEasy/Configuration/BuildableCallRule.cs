namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Api;

    /// <summary>
    /// Provides the base for rules that can be built using the FakeConfiguration.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Buildable")]
    public abstract class BuildableCallRule
        : IFakeObjectCallRule
    {
        #region Construction
        protected BuildableCallRule()
        {
            this.Actions = new LinkedList<Action<IFakeObjectCall>>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// An action that is called by the Apply method to apply this
        /// rule to a fake object call.
        /// </summary>
        public virtual Action<IWritableFakeObjectCall> Applicator { get; set; }

        /// <summary>
        /// A collection of actions that should be invoked when the configured
        /// call is made.
        /// </summary>
        public virtual ICollection<Action<IFakeObjectCall>> Actions { get; private set; }

        /// <summary>
        /// The number of times the configured rule should be used.
        /// </summary>
        public int? NumberOfTimesToCall
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public virtual void Apply(IWritableFakeObjectCall fakeObjectCall)
        {
            if (this.Applicator == null)
            {
                throw new InvalidOperationException(ExceptionMessages.ApplicatorNotSetExceptionMessage);
            }

            foreach (var action in this.Actions)
            {
                action.Invoke(fakeObjectCall);
            }

            this.Applicator.Invoke(fakeObjectCall);

            if (this.CallBaseMethod)
            {
                fakeObjectCall.CallBaseMethod();
            }
        }

        /// <summary>
        /// Gets or sets wether the base mehtod of the fake object call should be
        /// called when the fake object call is made.
        /// </summary>
        public virtual bool CallBaseMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets if this rule is applicable to the specified call.
        /// </summary>
        /// <param name="fakeObjectCall">The call to validate.</param>
        /// <returns>True if the rule applies to the call.</returns>
        public virtual bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return this.OnIsApplicableTo(fakeObjectCall);
        }

        protected abstract bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall);
        #endregion
    }
}
