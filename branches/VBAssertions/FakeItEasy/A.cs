namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using FakeItEasy.Expressions;
    
    /// <summary>
    /// Provides methods for generating fake objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A", Justification = "This name is used to provide more readable creation of fake objects.")]
    public static class A
    {
        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <returns>A fake object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of fake to create.")]
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
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design of the Expression<> type.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Makes no sense in this case.")]
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

            var fakeObject = new FakeObject(typeof(T));
            fakeObject.AddRule(new WrappedObjectRule(wrappedInstance));
            return (T)fakeObject.Object;
        }

        /// <summary>
        /// Gets a value indicating if the two objects are equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>True if the two objects are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as in the .Net framework.")]
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
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as in the .Net framework.")]
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

        /// <summary>
        /// Gets whether the specified type is fakeable or not.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>True if the specified type is fakeable.</returns>
        internal static bool TypeIsFakeable(Type type)
        {
            if (type.IsSealed)
            {
                return false;
            }

            if (!type.IsAbstract && !type.IsInterface)
            {
                if (!Helpers.TypeHasDefaultConstructor(type) && Helpers.GetFirstConstructorWhereAllArgumentsAreFakeable(type) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static T CreateFake<T>(IEnumerable<object> argumentsForConstructor)
        {
            return (T)CreateFake(typeof(T), argumentsForConstructor);
        }

        private static object CreateFake(Type typeOfFake, IEnumerable<object> argumentsForConstructor)
        {
            var factory = ServiceLocator.Current.Resolve<FakeObjectFactory.Creator>().Invoke();
            return factory.CreateFake(typeOfFake, argumentsForConstructor, true);
        }
    }
}