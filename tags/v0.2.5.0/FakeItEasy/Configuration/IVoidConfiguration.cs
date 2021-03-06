using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;

namespace FakeItEasy.Configuration
{
    public interface IVoidConfiguration<TFake>
            : IExceptionThrowerConfiguration<TFake>, IHideObjectMembers
    {
        /// <summary>
        /// Configures the specified call to do nothing when called.
        /// </summary>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration<TFake> DoesNothing();
    }    
}
