using System.Collections.Generic;
using System.Linq;
using System;

namespace FakeItEasy.Api
{
    internal abstract class FakeScope
        : IDisposable
    {
        internal static FakeScope Current = new RootScope();

        internal void AddInterceptedCall(FakeObject fakeObject, ICompletedFakeObjectCall call)
        {
            fakeObject.AllRecordedCalls.Add(call);
            this.OnAddInterceptedCall(fakeObject, call);
        }

        internal void AddRule(FakeObject fakeObject, CallRuleMetadata rule)
        {
            fakeObject.AllUserRules.AddFirst(rule);
            this.OnAddRule(fakeObject, rule);
        }

        /// <summary>
        /// Closes the scope.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.OnDispose();
        }

        protected abstract void OnDispose();
        protected abstract void OnAddRule(FakeObject fakeObject, CallRuleMetadata rule);
        protected abstract void OnAddInterceptedCall(FakeObject fakeObject, ICompletedFakeObjectCall call);
        internal abstract IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeObject fakeObject);

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

        private class RootScope
            : FakeScope
        {
            protected override void OnAddRule(FakeObject fakeObject, CallRuleMetadata rule)
            {
                // Do nothing
            }

            protected override void OnDispose()
            {
                // Do nothing
            }

            protected override void OnAddInterceptedCall(FakeObject fakeObject, ICompletedFakeObjectCall call)
            {
                // Do nothing
            }

            internal override IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeObject fakeObject)
            {
                return fakeObject.AllRecordedCalls;
            }
        }

        private class ChildScope
            : FakeScope
        {
            private FakeScope parentScope;
            private Dictionary<FakeObject, List<CallRuleMetadata>> rulesField;
            private Dictionary<FakeObject, List<ICompletedFakeObjectCall>> recordedCalls;

            public ChildScope(FakeScope parentScope)
            {
                this.parentScope = parentScope;
                this.rulesField = new Dictionary<FakeObject, List<CallRuleMetadata>>();
                this.recordedCalls = new Dictionary<FakeObject, List<ICompletedFakeObjectCall>>();
            }

            protected override void OnAddRule(FakeObject fakeObject, CallRuleMetadata rule)
            {
                this.parentScope.OnAddRule(fakeObject, rule);

                List<CallRuleMetadata> rules;

                if (!this.rulesField.TryGetValue(fakeObject, out rules))
                {
                    rules = new List<CallRuleMetadata>();
                    this.rulesField.Add(fakeObject, rules);
                }

                rules.Add(rule);
            }

            protected override void OnDispose()
            {
                this.RemoveRulesConfiguredInScope();
                FakeScope.Current = this.parentScope;
            }

            private void RemoveRulesConfiguredInScope()
            {
                foreach (var objectRules in this.rulesField)
                {
                    foreach (var rule in objectRules.Value)
                    {
                        objectRules.Key.AllUserRules.Remove(rule);
                    }
                }
            }

            protected override void OnAddInterceptedCall(FakeObject fakeObject, ICompletedFakeObjectCall call)
            {
                List<ICompletedFakeObjectCall> calls;

                if (!this.recordedCalls.TryGetValue(fakeObject, out calls))
                {
                    calls = new List<ICompletedFakeObjectCall>();
                    this.recordedCalls.Add(fakeObject, calls);
                }

                calls.Add(call);
            }

            internal override IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeObject fakeObject)
            {
                List<ICompletedFakeObjectCall> calls;

                if (!this.recordedCalls.TryGetValue(fakeObject, out calls))
                {
                    calls = new List<ICompletedFakeObjectCall>();
                }

                return calls;
            }
        }
    }

    
}
