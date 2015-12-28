using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;

namespace FakeItEasy.Configuration
{
    public interface IRepeatConfiguration<TFake>
            : IHideObjectMembers
    {
        /// <summary>
        /// Specifies the number of times for the configured event.
        /// </summary>
        /// <param name="numberOfTimes">The number of times to repeat.</param>
        void NumberOfTimes(int numberOfTimesToRepeat);
    }
}
