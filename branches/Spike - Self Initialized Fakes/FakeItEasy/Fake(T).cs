using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Linq.Expressions;

namespace FakeItEasy
{
    public class Fake<T> : IFakeConfiguration<T> where T : class
    {
        #region Construction
        /// <summary>
        /// Creates a new fake object.
        /// </summary>
        public Fake()
        {
            this.FakedObject = A.Fake<T>();
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
            this.FakedObject = A.Fake<T>(constructorCall);
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
            this.FakedObject = A.Fake<T>(argumentsForConstructor);
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
        public CallCollection<T> RecordedCalls
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
        public IVoidConfiguration<T> CallsTo(Expression<Action<T>> callSpecification)
        {
            return Fake.Configure(this.FakedObject).CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures calls to the specified member.
        /// </summary>
        /// <typeparam name="TMember">The type of value the member returns.</typeparam>
        /// <param name="callSpecification">An expression specifying the call to configure.</param>
        /// <returns>A configuration object.</returns>
        public IReturnValueConfiguration<T, TMember> CallsTo<TMember>(Expression<Func<T, TMember>> callSpecification)
        {
            return Fake.Configure(this.FakedObject).CallsTo(callSpecification);
        }

        /// <summary>
        /// Asserts on the faked object.
        /// </summary>
        /// <returns>A fake assertions object.</returns>
        public IFakeAssertions<T> Assert()
        {
            return Fake.Assert(this.FakedObject);
        }

        /// <summary>
        /// Configures any call to the fake object.
        /// </summary>
        /// <returns>A configuration object.</returns>
        public IVoidConfiguration<T> AnyCall()
        {
            return Fake.Configure(this.FakedObject).AnyCall();
        }
        #endregion
    }
}
