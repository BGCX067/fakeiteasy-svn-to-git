﻿namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides methods for configuring a fake object.
    /// </summary>
    /// <typeparam name="TFake">The type of fake object.</typeparam>
    public interface IStartConfiguration<TFake>
        : IHideObjectMembers
    {
        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <typeparam name="TMember">The type of the return value of the member.</typeparam>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TMember>(Expression<Func<TFake, TMember>> callSpecification);

        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        IVoidArgumentValidationConfiguration CallsTo(Expression<Action<TFake>> callSpecification);

        /// <summary>
        /// Configures the behavior of the fake object whan a call is made to any method on the
        /// object.
        /// </summary>
        /// <returns>A configuration object.</returns>
        IAnyCallConfiguration AnyCall();
    }

    public interface IAnyCallConfiguration
        : IVoidConfiguration
    {
        IReturnValueArgumentValidationConfiguration<TMember> WithReturnType<TMember>();
    }
}