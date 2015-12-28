using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Legend.Fakes.Extensibility;
using System.Reflection;
using Legend.Fakes.Api;
using System.Diagnostics.CodeAnalysis;

namespace Legend.Fakes.Configuration
{
    /// <summary>
    /// Provides the base for rules that can be built using the FakeConfiguration.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Buildable")]
    public abstract class BuildableCallRule
            : IFakeObjectCallRule
    {
        #region Properties
        public Action<IWritableFakeObjectCall> Applicator { get; set; }

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

            this.Applicator.Invoke(fakeObjectCall);
        }

        /// <summary>
        /// Gets if this rule is applicable to the specified call.
        /// </summary>
        /// <param name="fakeObjectCall">The call to validate.</param>
        /// <returns>True if the rule applies to the call.</returns>
        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            Guard.IsNotNull(fakeObjectCall, "fakeObjectCall");

            return this.OnIsApplicableTo(fakeObjectCall);
        }

        protected abstract bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall);
        #endregion
    }
}
