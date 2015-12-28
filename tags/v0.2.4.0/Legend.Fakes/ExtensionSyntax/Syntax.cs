using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.Configuration;

namespace Legend.Fakes.ExtensionSyntax
{
    public static class Syntax
    {
        public static IFakeConfiguration<TFake> Configure<TFake>(this TFake fakedObject) where TFake : class
        {
            return Fake.Configure(fakedObject);
        }
    }
}
