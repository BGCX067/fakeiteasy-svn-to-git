namespace FakeItEasy.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Diagnostics;

    /// <summary>
    /// The central point in the API for proxied fake objects handles interception
    /// of fake object calls by using a set of rules. User defined rules can be inserted
    /// by using the AddRule-method.
    /// </summary>
    [Serializable]
    public partial class FakeObject
    {
        private IEnumerable<CallRuleMetadata> preUserRules;
        private LinkedList<CallRuleMetadata> allUserRulesField;
        private IEnumerable<CallRuleMetadata> postUserRules;

        private List<ICompletedFakeObjectCall> recordedCallsField;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeObject"/> class.
        /// </summary>
        /// <param name="type">The type of object to proxy.</param>
        public FakeObject(Type type)
        {
            Guard.IsNotNull(type, "type");
            AssertThatTypeCanBeProxied(type);
            
            this.Initialize(type, GetArgumentsForConstructor(type));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeObject"/> class, faking a concrete class using the argument values
        /// in the specified expression as arguments for the base constructor.
        /// </summary>
        /// <param name="type">The type of object to proxy.</param>
        /// <param name="argumentsForConstructor">The arguments for constructor.</param>
        public FakeObject(Type type, object[] argumentsForConstructor)
        {
            Guard.IsNotNull(type, "type");
            Guard.IsNotNull(argumentsForConstructor, "argumentsForConstructor");

            this.Initialize(type, argumentsForConstructor);
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

        internal LinkedList<CallRuleMetadata> AllUserRules
        {
            get
            {
                return this.allUserRulesField;
            }
        }

        private IEnumerable<CallRuleMetadata> AllRules
        { 
            get
            {
                return this.preUserRules.Concat(this.AllUserRules.Concat(this.postUserRules));
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

        private static void AssertThatTypeCanBeProxied(Type type)
        {
            if (!ServiceLocator.Current.Resolve<IFakeProxyGenerator>().CanCreateProxyOfType(type) || GetArgumentsForConstructor(type) == null)
            {
                throw new InvalidOperationException(ExceptionMessages.TypeCanNotBeProxied.FormatInvariant(type.FullName));
            }
        }

        private static object[] GetArgumentsForConstructor(Type type)
        {
            if (type.IsInterface || type.IsAbstract || Helpers.TypeHasDefaultConstructor(type))
            {
                return new object[] { };
            }

            var constructor = Helpers.GetFirstConstructorWhereAllArgumentsAreFakeable(type);

            if (constructor == null)
            {
                return null;
            }

            return
                (from argument in Helpers.GetFirstConstructorWhereAllArgumentsAreFakeable(type).GetParameters()
                 select A.Fake(argument.ParameterType)).ToArray();
        }

        private void Intercept(IWritableFakeObjectCall fakeObjectCall)
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
        }
        
        private void MoveRuleToFront(CallRuleMetadata rule)
        {
            if (this.allUserRulesField.Remove(rule))
            {
                this.allUserRulesField.AddFirst(rule);
            }
        }

        private void MoveRuleToFront(IFakeObjectCallRule rule)
        {
            var metadata = this.AllRules.Where(x => x.Rule.Equals(rule)).Single();
            this.MoveRuleToFront(metadata);
        }

        private void Initialize(Type type, object[] argumentsForConstructor)
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

            this.recordedCallsField = new List<ICompletedFakeObjectCall>();

            this.FakeObjectType = type;

            this.Object = ServiceLocator.Current.Resolve<IFakeProxyGenerator>().GenerateFake(this, this.Intercept, type, argumentsForConstructor);
        }
    }
}