using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FakeItEasy.Api;

namespace FakeItEasy
{
    public static class FakeExtensions
    {
        /// <summary>
        /// Specifies NumberOfTimes(1) to the IRepeatConfiguration{TFake}.
        /// </summary>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <param name="configuration">The configuration to set repeat 1 to.</param>
        public static void Once<TFake>(this IRepeatConfiguration<TFake> configuration)
        {
            Guard.IsNotNull(configuration, "configuration");
            configuration.NumberOfTimes(1);
        }

        /// <summary>
        /// Specifies NumberOfTimes(2) to the IRepeatConfiguration{TFake}.
        /// </summary>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <param name="configuration">The configuration to set repeat 2 to.</param>
        public static void Twice<TFake>(this IRepeatConfiguration<TFake> configuration)
        {
            Guard.IsNotNull(configuration, "configuration");

            configuration.NumberOfTimes(2);
        }
    }
}