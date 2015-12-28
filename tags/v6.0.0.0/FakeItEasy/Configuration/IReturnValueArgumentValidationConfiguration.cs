namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Api;
    /// <summary>
    /// Configures a call that returns a value and allows the use to
    /// specify validations for arguments.
    /// </summary>
    /// <typeparam name="TFake">The type of the fake.</typeparam>
    /// <typeparam name="TMember">The type of the member.</typeparam>
    public interface IReturnValueArgumentValidationConfiguration<TFake, TMember>
        : IReturnValueConfiguration<TFake, TMember>,
          IArgumentValidationConfiguration<IReturnValueConfiguration<TFake, TMember>>
    {

    }
}
