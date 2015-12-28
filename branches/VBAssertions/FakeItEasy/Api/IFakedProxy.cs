﻿using System;
namespace FakeItEasy.Api
{
    /// <summary>
    /// Interface implemented by generated faked objects in order
    /// to access the fake object behind it.
    /// </summary>
    internal interface IFakedProxy
    {
        /// <summary>
        /// Gets the fake object behind a faked object.
        /// </summary>
        /// <returns>A fake object.</returns>
        FakeObject GetFakeObject();
    }
}
