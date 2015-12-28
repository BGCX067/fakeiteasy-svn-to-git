using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using FakeItEasy.Extensibility;
using System.ComponentModel;
using FakeItEasy.Api;
using System.Diagnostics.CodeAnalysis;

namespace FakeItEasy
{
    /// <summary>
    /// Provides methods for generating fake objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A")]
    public static class A
    {
        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <returns>A fake object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T Fake<T>() where T : class
        {
            var fake = new FakeObject(typeof(T));
            return (T)fake.Object;
        }

        /// <summary>
        /// Creates a fake object of the non interface type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <param name="constructorCall">An expression that specifies the parameters that will
        /// be passed to the constructor of the generated fake class.</param>
        /// <returns>A fake object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static T Fake<T>(Expression<Func<T>> constructorCall) where T : class
        {
            Guard.IsNotNull(constructorCall, "constructorCall");

            if (constructorCall.Body.NodeType != ExpressionType.New)
            {
                throw new ArgumentException(ExceptionMessages.NonConstructorExpressionMessage);
            }

            var constructorArguments =
                (from argument in ((NewExpression)constructorCall.Body).Arguments
                 select ExpressionManager.GetValueProducedByExpression(argument)).ToArray();

            return (T)new FakeObject(typeof(T), constructorArguments).Object;
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

            return (T)new FakeObject(typeof(T), argumentsForConstructor.ToArray()).Object;
        }

        /// <summary>
        /// Creates a fake version of the object where calls can be configured.
        /// Any unconfigured calls will call the wrapped instance directly.
        /// </summary>
        /// <typeparam name="T">The type of fake to generate.</typeparam>
        /// <param name="wrappedInstance">The object to wrap.</param>
        /// <returns>A fake object.</returns>
        public static T Fake<T>(T wrappedInstance) where T : class
        {
            Guard.IsNotNull(wrappedInstance, "wrappedInstance");

            var fakeObject = new FakeObject(typeof(T));
            fakeObject.AddRule(new WrappedObjectRule(wrappedInstance));
            return (T)fakeObject.Object;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as in the .Net framework.")]
        public static bool Equals(object objA, object objB)
        {
            return object.Equals(objA, objB);
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification="Using the same names as in the .Net framework.")]
        public static bool ReferenceEquals(object objA, object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }
    }
}