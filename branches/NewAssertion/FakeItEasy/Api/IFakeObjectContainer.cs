﻿namespace FakeItEasy.Api
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A container that can create fake objects.
    /// </summary>
    public interface IFakeObjectContainer
    {
        /// <summary>
        /// Creates a fake object of the specified type using the specified arguments if it's
        /// supported by the container, returns a value indicating if it's supported or not.
        /// </summary>
        /// <param name="typeOfFakeObject">The type of fake object to create.</param>
        /// <param name="fakeObject">The fake object that was created if the method returns true.</param>
        /// <returns>True if a fake object can be created.</returns>
        bool TryCreateFakeObject(Type typeOfFakeObject, out object fakeObject);

        /// <summary>
        /// Applies base configuration to a fake object.
        /// </summary>
        /// <param name="typeOfFakeObject">The type the fake object represents.</param>
        /// <param name="fakeObject">The fake object to configure.</param>
        void ConfigureFake(Type typeOfFakeObject, object fakeObject);
    }
}
