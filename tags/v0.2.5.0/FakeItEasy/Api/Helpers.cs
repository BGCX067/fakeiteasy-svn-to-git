using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace FakeItEasy.Api
{
    internal static class Helpers
    {
        public static void WriteCalls(this IEnumerable<IFakeObjectCall> calls, TextWriter writer)
        {
            Guard.IsNotNull(calls, "calls");
            Guard.IsNotNull(writer, "writer");

            foreach (var call in calls)
            {
                writer.WriteLine(call.GetDescription());
            }
        }

        public static string GetDescription(this IFakeObjectCall fakeObjectCall)
        {
            Guard.IsNotNull(fakeObjectCall, "fakeObjectCall");

            var method = fakeObjectCall.Method;

            return "{0}.{1}({2})".FormatInvariant(method.DeclaringType.FullName, method.Name, GetParametersString(fakeObjectCall));
        }

        private static string GetParametersString(IFakeObjectCall fakeObjectCall)
        {
            var result = new StringBuilder();

            using (var parameters = fakeObjectCall.Method.GetParameters().Cast<ParameterInfo>().GetEnumerator())
            using (var arguments = fakeObjectCall.Arguments.GetEnumerator())
            {
                while (parameters.MoveNext() && arguments.MoveNext())
                {
                    if (result.Length > 0)
                    {
                        result.Append(", ");
                    }

                    result.AppendFormat("[{0}] {1}", parameters.Current.ParameterType, GetArgumentAsString(arguments.Current));
                }
            }

            return result.ToString();
        }

        private static string GetArgumentAsString(object argument)
        {
            return argument != null ? argument.ToString() : "<NULL>";
        }

        internal static bool IsSameMethodOrDerivative(Type calledOnType, MethodInfo method, MethodInfo baseMethod)
        {
            Guard.IsNotNull(calledOnType, "calledOnType");
            Guard.IsNotNull(method, "method");
            Guard.IsNotNull(baseMethod, "baseMethod");


            return MethodEqualsOrDerives(method, baseMethod) || MethodImplementsInterfaceMethod(calledOnType, method, baseMethod);
        }

        private static bool MethodEqualsOrDerives(MethodInfo method, MethodInfo baseMethod)
        {
            return baseMethod.GetBaseDefinition().Equals(method.GetBaseDefinition());
        }

        private static bool MethodImplementsInterfaceMethod(Type calledOnType, MethodInfo method, MethodInfo baseMethod)
        {
            var definitionOfBaseMethod = baseMethod.GetBaseDefinition();

            if (definitionOfBaseMethod.DeclaringType.IsInterface && ImplementsInterface(calledOnType, definitionOfBaseMethod.DeclaringType))
            {
                var map = calledOnType.GetInterfaceMap(definitionOfBaseMethod.DeclaringType);
                var indexOfMethod = map.TargetMethods.TakeWhile(x => !x.GetBaseDefinition().Equals(method.GetBaseDefinition())).Count();

                if (indexOfMethod < map.InterfaceMethods.Length)
                {
                    var interfaceMethod = map.InterfaceMethods[indexOfMethod];

                    return interfaceMethod.GetBaseDefinition().Equals(definitionOfBaseMethod);
                }
            }

            return false;
        }

        private static bool ImplementsInterface(Type implemetor, Type interfaceType)
        {
            if (implemetor.IsInterface)
            {
                return false;
            }

            var foundInterface = implemetor.GetInterfaces().Where(x => x.Equals(interfaceType)).FirstOrDefault();
            return foundInterface != null;
        }

        [DebuggerStepThrough]
        public static object GetDefaultValueOfType(Type type)
        {
            return type.IsValueType && !type.Equals(typeof(void)) ? Activator.CreateInstance(type) : null;
        }
    }
}
