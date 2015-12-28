using System;
using NUnit.Framework;
using System.Linq.Expressions;
using FakeItEasy.Extensibility;
using System.Reflection;
using FakeItEasy.Api;

namespace FakeItEasy.Tests
{
    /// <summary>
    /// A fake implementation of IFakeObjectCall, used for testing.
    /// </summary>
    public class FakeCall
            : IWritableFakeObjectCall, ICompletedFakeObjectCall
    {
        public FakeCall()
        {
            this.Arguments = ArgumentCollection.Empty;
        }

        public MethodInfo Method
        {
            get;
            set;
        }

        public ArgumentCollection Arguments
        {
            get;
            set;
        }


        public void SetReturnValue(object returnValue)
        {
            this.ReturnValue = returnValue;           
        }

        public static FakeCall Create<T>(string methodName, Type[] parameterTypes, object[] arguments) where T : class
        {
            var method = typeof(T).GetMethod(methodName, parameterTypes);

            return new FakeCall
            {
                Method = method,
                Arguments = new ArgumentCollection(arguments, method),
                FakedObject = A.Fake<T>()
            };
        }

        public static FakeCall Create<T>(string methodName) where T : class
        {
            return Create<T>(methodName, new Type[] { }, new object[] { });
        }

        public object ReturnValue
        {
            get;
            private set;
        }


        public object FakedObject
        {
            get;
            set;
        }


        public ICompletedFakeObjectCall Freeze()
        {
            return this;
        }


        public void CallBaseMethod()
        {
            
        }
    }
}