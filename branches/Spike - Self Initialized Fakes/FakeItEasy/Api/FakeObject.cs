using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Globalization;
using System.Reflection;
using FakeItEasy.Configuration;
using System.Diagnostics;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace FakeItEasy.Api
{
    [Serializable]
    public partial class FakeObject
    {
        private IEnumerable<CallRuleMetadata> preUserRules;
        private LinkedList<CallRuleMetadata> allUserRulesField;
        private IEnumerable<CallRuleMetadata> postUserRules;

        private List<ICompletedFakeObjectCall> recordedCallsField;
        internal FakeObjectInterceptor Interceptor;        
        

        /// <summary>
        /// Creates a new fake object.
        /// </summary>
        public FakeObject(Type type)
        {
            Guard.IsNotNull(type, "type");
            AssertThatTypeIsFakeable(type);

            this.Initialize(type, () => FakeGenerator.GenerateFake(type, this));
        }

        /// <summary>
        /// Creates a new fake object, faking a concrete class using the argument values
        /// in the specified expression as arguments for the base constructor.
        /// </summary>
        /// <param name="constructorCall">The constructor to call on the base class.</param>
        public FakeObject(Type type, object[] argumentsForConstructor)
        {
            Guard.IsNotNull(type, "type");
            Guard.IsNotNull(argumentsForConstructor, "argumentsForConstructor");

            this.Initialize(type, () => FakeGenerator.GenerateFake(type, this, argumentsForConstructor));
        }

        private void Initialize(Type type, Func<object> fakedObject)
        {
            this.preUserRules = new[] 
            {
                new CallRuleMetadata { Rule = new EventRule { FakeObject = this } } 
            };
            this.allUserRulesField = new LinkedList<CallRuleMetadata>();
            this.postUserRules = new[] 
            { 
                new CallRuleMetadata { Rule = new ObjectMemberRule { FakeObject = this } },
                new CallRuleMetadata { Rule = new AutoFakePropertyRule { FakeObject = this } },
                new CallRuleMetadata { Rule = new PropertySetterRule { FakeObject = this } },
                new CallRuleMetadata { Rule = new DefaultReturnValueRule() }
            };

            this.Interceptor = new FakeObjectInterceptor(this);
            this.recordedCallsField = new List<ICompletedFakeObjectCall>();

            this.FakeObjectType = type;
            this.Object = fakedObject.Invoke();
        }

        /// <summary>
        /// Gets the faked object.
        /// </summary>
        public object Object
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the faked type.
        /// </summary>
        public Type FakeObjectType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the interceptions that are currently registered with the fake object.
        /// </summary>
        public IEnumerable<IFakeObjectCallRule> Rules
        {
            get
            {
                return this.allUserRulesField.Select(x => x.Rule);
            }
        }

        /// <summary>
        /// Gets a collection of all the calls made to the fake object within the current scope.
        /// </summary>
        public IEnumerable<ICompletedFakeObjectCall> RecordedCallsInScope
        {
            get
            {
                return FakeScope.Current.GetCallsWithinScope(this);
            }
        }

        internal List<ICompletedFakeObjectCall> AllRecordedCalls
        {
            get
            {
                return this.recordedCallsField;
            }
        }

        private IEnumerable<CallRuleMetadata> AllRules
        { 
            get
            {
                return this.preUserRules.Concat(this.AllUserRules.Concat(this.postUserRules));
            }
        }

        internal LinkedList<CallRuleMetadata> AllUserRules
        {
            get
            {
                return this.allUserRulesField;
            }
        }

        /// <summary>
        /// Adds a call rule to the fake object.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public void AddRule(IFakeObjectCallRule rule)
        {
            var newRule = new CallRuleMetadata { Rule = rule };
            FakeScope.Current.AddRule(this, newRule);
        }

        internal void Intercept(IWritableFakeObjectCall fakeObjectCall)
        {
            var ruleToUse =
                (from rule in this.AllRules
                 where rule.Rule.IsApplicableTo(fakeObjectCall) && rule.HasNotBeenCalledSpecifiedNumberOfTimes()
                 select rule).First();

            this.ApplyRule(ruleToUse, fakeObjectCall);

            FakeScope.Current.AddInterceptedCall(this, fakeObjectCall.Freeze());
        }

        private void ApplyRule(CallRuleMetadata rule, IWritableFakeObjectCall fakeObjectCall)
        {
            rule.CalledNumberOfTimes++;
            rule.Rule.Apply(fakeObjectCall);
            this.MoveRuleToFront(rule);
        }
        
        private void MoveRuleToFront(CallRuleMetadata rule)
        {
            if (this.allUserRulesField.Remove(rule))
            {
                this.allUserRulesField.AddFirst(rule);
            }
        }

        private static void AssertThatTypeIsFakeable(Type type)
        {
            if (type.IsSealed)
            {
                throw new InvalidOperationException(ExceptionMessages.TypeIsSealedMessage);
            }

            if (!type.IsAbstract && !type.IsInterface)
            {
                var defaultConstructor =
                    from constructor in type.GetConstructors()
                    where constructor.GetParameters().Length == 0
                    select constructor;

                if (defaultConstructor.Count() < 1)
                {
                    throw new InvalidOperationException(ExceptionMessages.NoDefaultConstructorMessage);
                }
            }
        }
    }
}