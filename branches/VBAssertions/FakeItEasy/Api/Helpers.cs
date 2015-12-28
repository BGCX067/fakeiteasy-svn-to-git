namespace FakeItEasy.Api
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

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

        internal static bool TypeHasDefaultConstructor(Type type)
        {
            return
                (from constructor in type.GetConstructors()
                 where constructor.GetParameters().Length == 0
                 select constructor).FirstOrDefault() != null;
        }

        internal static ConstructorInfo GetFirstConstructorWhereAllArgumentsAreFakeable(Type type)
        {
            return
                (from constructor in type.GetConstructors()
                 where constructor.GetParameters().All(x => A.TypeIsFakeable(x.ParameterType))
                 select constructor).FirstOrDefault();
        }

        private static string GetParametersString(IFakeObjectCall fakeObjectCall)
        {
            var result = new StringBuilder();

            using (var parameters = fakeObjectCall.Method.GetParameters().Cast<ParameterInfo>().GetEnumerator())
            using (var arguments = fakeObjectCall.Arguments.AsEnumerable().GetEnumerator())
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

        [DebuggerStepThrough]
        public static object GetDefaultValueOfType(Type type)
        {
            return type.IsValueType && !type.Equals(typeof(void)) ? Activator.CreateInstance(type) : null;
        }
    }
}
