using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;

namespace FakeItEasy.ExtensionSyntax
{
    public static class Syntax
    {
        public static IFakeConfiguration<TFake> Configure<TFake>(this TFake fakedObject) where TFake : class
        {
            return Fake.Configure(fakedObject);
        }
    }
}
