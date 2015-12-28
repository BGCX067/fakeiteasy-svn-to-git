namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Assertion;
    using FakeItEasy.Configuration;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Represents a fake object that provides an api for configuring a faked object, exposed by the
    /// FakedObject-property.
    /// </summary>
    /// <typeparam name="T">The type of the faked object.</typeparam>
    public class Fake<T> : IFakeConfiguration<T>
    {
        #region Construction
        /// <summary>
        /// Creates a new fake object.
        /// </summary>
        public Fake()
        {
            this.FakedObject = CreateFake<T>(null);
        }

        /// <summary>
        /// Creates a fake object and passes the arguments for the specified constructor call
        /// to the constructor of the fake object.
        /// </summary>
        /// <param name="constructorCall">An expression describing the constructor to be called
        /// on the faked object.</param>
        /// <exception cref="ArgumentNullException">The constructor call was null.</exception>
        public Fake(Expression<Func<T>> constructorCall)
        {
            Guard.IsNotNull(constructorCall, "constructorCall");

            if (constructorCall.Body.NodeType != ExpressionType.New)
            {
                throw new ArgumentException(ExceptionMessages.NonConstructorExpressionMessage);
            }

            var constructorArguments =
                from argument in ((NewExpression)constructorCall.Body).Arguments
                select ExpressionManager.GetValueProducedByExpression(argument);

            this.FakedObject = CreateFake<T>(constructorArguments);
        }

        /// <summary>
        /// Creates a fake object that wraps the specified instance.
        /// </summary>
        /// <param name="wrappedInstance">The instance to wrap in a fake object wrapper.</param>
        /// <exception cref="ArgumentNullException">The wrappedInstance was null.</exception>
        public Fake(T wrappedInstance)
        {
            this.FakedObject = A.Fake<T>(wrappedInstance);
        }

        /// <summary>
        /// Creates a fake object and passes the specified arguments to the constructor of the fake.
        /// </summary>
        /// <param name="argumentsForConstructor">Arguments to be used when calling the constructor of the faked type.</param>
        /// <exception cref="ArgumentNullException">The argumentsForConstructor was null.</exception>
        public Fake(IEnumerable<object> argumentsForConstructor)
        {
            Guard.IsNotNull(argumentsForConstructor, "argumentsForConstructor");

            if (!typeof(T).IsAbstract)
            {
                throw new InvalidOperationException(ExceptionMessages.FakingNonAbstractClassWithArgumentsForConstructor);
            }

            this.FakedObject = CreateFake<T>(argumentsForConstructor);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the faked object.
        /// </summary>
        public T FakedObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all calls made to the faked object.
        /// </summary>
        public ICallCollection<T> RecordedCalls
        {
            get
            {
                return Fake.GetCalls(this.FakedObject);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Configures calls to the specified member.
        /// </summary>
        /// <param name="callSpecification">An expression specifying the call to configure.</param>
        /// <returns>A configuration object.</returns>
        public IVoidArgumentValidationConfiguration<T> CallsTo(Expression<Action<T>> callSpecification)
        {
            return Fake.Configure(this.FakedObject).CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures calls to the specified member.
        /// </summary>
        /// <typeparam name="TMember">The type of value the member returns.</typeparam>
        /// <param name="callSpecification">An expression specifying the call to configure.</param>
        /// <returns>A configuration object.</returns>
        public IReturnValueArgumentValidationConfiguration<T, TMember> CallsTo<TMember>(Expression<Func<T, TMember>> callSpecification)
        {
            return Fake.Configure(this.FakedObject).CallsTo(callSpecification);
        }

        /// <summary>
        /// Asserts on the faked object.
        /// </summary>
        /// <returns>A fake assertions object.</returns>
        private IFakeAssertions<T> Assert()
        {
            return ServiceLocator.Current.Resolve<IFakeAssertionsFactory>().CreateAsserter<T>(Fake.GetFakeObject(this.FakedObject));
        }


        /// <summary>
        /// Asserts that the specified call was made on the faked object.
        /// </summary>
        /// <param name="callSpecification">The call to assert on.</param>
        public void AssertWasCalled(Expression<Action<T>> callSpecification)
        {
            this.Assert().WasCalled(callSpecification);
        }

        /// <summary>
        /// Asserts that the specified call was made on the faked object.
        /// </summary>
        /// <param name="callSpecification">The call to assert on.</param>
        public void AssertWasCalled(Expression<Action<T>> callSpecification, Expression<Func<int, bool>> repeatValidation)
        {
            this.Assert().WasCalled(callSpecification, repeatValidation);
        }

        /// <summary>
        /// Asserts that the specified call was made on the faked object.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="callSpecification">The call to assert on.</param>
        public void AssertWasCalled<TMember>(Expression<Func<T, TMember>> callSpecification)
        {
            this.Assert().WasCalled(callSpecification);
        }

        /// <summary>
        /// Asserts that the specified call was made on the faked object.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="callSpecification">The call to assert on.</param>
        public void AssertWasCalled<TMember>(Expression<Func<T, TMember>> callSpecification, Expression<Func<int, bool>> repeatValidation)
        {
            this.Assert().WasCalled(callSpecification, repeatValidation);
        }

        /// <summary>
        /// Configures any call to the fake object.
        /// </summary>
        /// <returns>A configuration object.</returns>
        public IVoidConfiguration<T> AnyCall()
        {
            return Fake.Configure(this.FakedObject).AnyCall();
        }

        private static T CreateFake<T>(IEnumerable<object> argumentsForConstructor)
        {
            return (T)Fake.CreateFactory().CreateFake(typeof(T), argumentsForConstructor, false);
        }
        #endregion
    }
}
