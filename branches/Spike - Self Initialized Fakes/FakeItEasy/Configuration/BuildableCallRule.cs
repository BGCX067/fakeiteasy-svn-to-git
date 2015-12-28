using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FakeItEasy.Extensibility;
using System.Reflection;
using FakeItEasy.Api;
using System.Diagnostics.CodeAnalysis;

namespace FakeItEasy.Configuration
{
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
        public Action<IWritableFakeObjectCall> Applicator { get; set; }

        public ICollection<Action<IFakeObjectCall>> Actions { get; private set; }

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

        public bool CallBaseMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets if this rule is applicable to the specified call.
        /// </summary>
        /// <param name="fakeObjectCall">The call to validate.</param>
        /// <returns>True if the rule applies to the call.</returns>
        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return this.OnIsApplicableTo(fakeObjectCall);
        }

        protected abstract bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall);
        #endregion
    }
}
