using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.Configuration;

namespace Legend.Fakes.VisualBasic
{
    public static class VisualBasicHelpers
    {
        public static IVoidConfiguration<TFake> WithAnyArguments<TFake>(this IVisualBasicConfiguration<TFake> configuration)
        {
            return configuration.WhenArgumentsMatch(x => true);
        }
    }
}
