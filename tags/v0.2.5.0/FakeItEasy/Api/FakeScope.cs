using System.Collections.Generic;
using System.Linq;
using System;

namespace FakeItEasy.Api
{
    internal abstract class FakeScope
        : IDisposable
    {
        internal static FakeScope Current
        {
            get;
            private set;
        }

        static FakeScope()
        {
            Current = new NullScope();
        }

        internal abstract void AddRule(CallRuleMetadata rule);
        
        /// <summary>
        /// Closes the scope.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.OnDispose();
        }

        protected abstract void OnDispose();

        /// <summary>
        /// Creates a new scope and sets it as the current scope.
        /// </summary>
        /// <returns>The created scope.</returns>
        public static FakeScope Create()
        {
            var result = new ChildScope(FakeScope.Current);
            FakeScope.Current = result;
            return result;
        }

        private class NullScope
            : FakeScope
        {
            internal override void AddRule(CallRuleMetadata rule)
            {
                // Do nothing
            }

            protected override void OnDispose()
            {
                // Do nothing
            }
        }

        private class ChildScope
            : FakeScope
        {
            private FakeScope parentScope;
            private List<CallRuleMetadata> rulesField;

            public ChildScope(FakeScope parentScope)
            {
                this.parentScope = parentScope;
                this.rulesField = new List<CallRuleMetadata>();
            }

            internal override void AddRule(CallRuleMetadata rule)
            {
                this.rulesField.Add(rule);
                this.parentScope.AddRule(rule);
            }

            protected override void OnDispose()
            {
                FakeScope.Current = this.parentScope;
            }
        }
    }

    
}
