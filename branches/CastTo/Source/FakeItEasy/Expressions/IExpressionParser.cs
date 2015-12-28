namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Api;

    /// <summary>
    /// Manages breaking call specification expression into their various parts.
    /// </summary>
    internal interface IExpressionParser
    {
        /// <summary>
        /// Gets the fake object an expression is called on.
        /// </summary>
        /// <param name="fakeObjectCall">The call expression.</param>
        /// <returns>A FakeObject.</returns>
        /// <exception cref="ArgumentNullException">The fakeObjectCall is null.</exception>
        /// <exception cref="ArgumentException">The specified expression is not an expression where a call is made to a faked object.</exception>
        FakeObject GetFakeObjectCallIsMadeOn(LambdaExpression fakeObjectCall);
    }
}
