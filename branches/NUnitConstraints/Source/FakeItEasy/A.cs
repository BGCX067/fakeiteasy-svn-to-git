namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using FakeItEasy.Expressions;
    using FakeItEasy.SelfInitializedFakes;
    using FakeItEasy.Configuration;
    
    /// <summary>
    /// Provides methods for generating fake objects.
    /// </summary>
    public static class A
    {
        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <returns>A fake object.</returns>
        public static T Fake<T>()
        {
            return CreateFake<T>(null);
        }

        /// <summary>
        /// Creates a fake object of the non interface type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <param name="constructorCall">An expression that specifies the parameters that will
        /// be passed to the constructor of the generated fake class.</param>
        /// <returns>A fake object.</returns>
        public static T Fake<T>(Expression<Func<T>> constructorCall)
        {
            Guard.IsNotNull(constructorCall, "constructorCall");

            if (constructorCall.Body.NodeType != ExpressionType.New)
            {
                throw new ArgumentException(ExceptionMessages.NonConstructorExpressionMessage);
            }

            var constructorArguments =
                from argument in ((NewExpression)constructorCall.Body).Arguments
                select ExpressionManager.GetValueProducedByExpression(argument);

            return CreateFake<T>(constructorArguments);
        }

        /// <summary>
        /// Creates a fake object from an abstract type that takes arguments to a constructor.
        /// </summary>
        /// <typeparam name="T">The type of fake to create.</typeparam>
        /// <param name="argumentsForConstructor">Arguments to pass to the constructor of the faked type.</param>
        /// <returns>A fake object.</returns>
        /// <exception cref="InvalidOperationException">The type is not abstract, use the Fake-overload that takes a constructor expression
        /// instead.</exception>
        /// <exception cref="MissingMethodException">No constructor matching the specified arguments was found.</exception>
        public static T Fake<T>(IEnumerable<object> argumentsForConstructor)
        {
            Guard.IsNotNull(argumentsForConstructor, "argumentsForConstructor");

            if (!typeof(T).IsAbstract)
            {
                throw new InvalidOperationException(ExceptionMessages.FakingNonAbstractClassWithArgumentsForConstructor);
            }

            return CreateFake<T>(argumentsForConstructor);
        }

        /// <summary>
        /// Creates a fake version of the object where calls can be configured.
        /// Any unconfigured calls will call the wrapped instance directly.
        /// </summary>
        /// <typeparam name="T">The type of fake to generate.</typeparam>
        /// <param name="wrappedInstance">The object to wrap.</param>
        /// <returns>A fake object.</returns>
        public static T Fake<T>(T wrappedInstance)
        {
            Guard.IsNotNull(wrappedInstance, "wrappedInstance");
            
            var fakeObject = FakeItEasy.Fake.GetFakeObject(CreateFake<T>(null));
            fakeObject.AddRule(new WrappedObjectRule(wrappedInstance));
            return (T)fakeObject.Object;
        }

        /// <summary>
        /// Creates a self initializing fake wrapper wrapping the specified object. The
        /// recorder is used to record calls so that they are only made to the wrapped instance
        /// if they've not already been recorded.
        /// </summary>
        /// <typeparam name="T">The type of the fake.</typeparam>
        /// <param name="wrappedInstance">The wrapped.</param>
        /// <param name="recorder">The recorder.</param>
        /// <returns>The faked object.</returns>
        public static T Fake<T>(T wrappedInstance, ISelfInitializingFakeRecorder recorder)
        {
            Guard.IsNotNull(wrappedInstance, "wrappedInstance");
            Guard.IsNotNull(recorder, "recorder");

            var fake = FakeItEasy.Fake.GetFakeObject(CreateFake<T>(null));
            fake.AddRule(new SelfInitializationRule(new WrappedObjectRule(wrappedInstance), recorder));
            return (T)fake.Object;
        }

        /// <summary>
        /// Gets a dummy object of the specified type. The value of a dummy object
        /// should be irrelevant. Dummy objects should not be configured.
        /// </summary>
        /// <typeparam name="T">The type of dummy to return.</typeparam>
        /// <returns>A dummy object of the specified type.</returns>
        /// <exception cref="ArgumentException">Dummies of the specified type can not be created.</exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static T Dummy<T>()
        {
            return (T)Factory.CreateFake(typeof(T), null, true);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>True if the two objects are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object objA, object objB)
        {
            return object.Equals(objA, objB);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are the same reference.
        /// </summary>
        /// <param name="objA">The obj A.</param>
        /// <param name="objB">The obj B.</param>
        /// <returns>True if the objects are the same reference.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool ReferenceEquals(object objA, object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }

        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fake object to create.</typeparam>
        /// <returns>A fake object.</returns>
        internal static object Fake(Type typeOfFake)
        {
            return CreateFake(typeOfFake, null);
        }

        private static FakeObjectFactory Factory
        {
            get
            {
                return ServiceLocator.Current.Resolve<FakeObjectFactory>();
            }
        }

        private static T CreateFake<T>(IEnumerable<object> argumentsForConstructor)
        {
            return (T)CreateFake(typeof(T), argumentsForConstructor);
        }

        private static object CreateFake(Type typeOfFake, IEnumerable<object> argumentsForConstructor)
        {
            return Factory.CreateFake(typeOfFake, argumentsForConstructor, false);
        }

        /// <summary>
        /// Configures a call to a faked object.
        /// </summary>
        /// <param name="callSpecification">An expression where the configured memeber is called.</param>
        /// <returns>A configuration object.</returns>
        public static IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        /// <summary>
        /// Configures a call to a faked object.
        /// </summary>
        /// <typeparam name="T">The type of member on the faked object to configure.</typeparam>
        /// <param name="callSpecification">An expression where the configured memeber is called.</param>
        /// <returns>A configuration object.</returns>
        public static IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        private static IFakeConfigurationManager ConfigurationManager
        {
            get
            {
                return ServiceLocator.Current.Resolve<IFakeConfigurationManager>();
            }
        }
    }



    /// <summary>
    /// Provides an api entry point for validating arguments of fake object calls.
    /// </summary>
    /// <typeparam name="T">The type of argument to validate.</typeparam>
    public class A<T>
    {
        /// <summary>
        /// Gets an argument validations object that provides validations for the argument.
        /// </summary>
        public static ArgumentValidatorScope<T> That
        {
            get
            {
                return new RootValidations<T>();
            }
        }

        /// <summary>
        /// Returns a validator that considers any value of an argument as valid.
        /// </summary>
        public static ArgumentValidator<T> Ignored
        {
            get
            {
                return ArgumentValidator<T>.Create(new RootValidations<T>(), x => true, "Ignored");
            }
        }
    }
}