using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legend.Fakes.Api
{
    /// <summary>
    /// Interface implemented by generated faked objects in order
    /// to access the fake object behind it.
    /// </summary>
    /// <typeparam name="T">The type of the faked object.</typeparam>
    internal interface IFakeObjectAccessor
    {
        /// <summary>
        /// Gets the fake object behind a faked object.
        /// </summary>
        /// <returns>A fake object.</returns>
        FakeObject GetFakeObject();
    }
}
