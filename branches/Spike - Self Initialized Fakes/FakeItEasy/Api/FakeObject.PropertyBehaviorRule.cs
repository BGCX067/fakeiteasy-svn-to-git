using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;
using System.Reflection;
using System.Diagnostics;

namespace FakeItEasy.Api
{
    public partial class FakeObject
    {
        private class PropertyBehaviorRule
            : IFakeObjectCallRule
        {
            private MethodInfo propertySetter;
            private MethodInfo propertyGetter;

            public object Value;

            public PropertyBehaviorRule(MethodInfo propertyGetterOrSetter)
            {
                var property = GetProperty(propertyGetterOrSetter);

                this.propertySetter = property.GetSetMethod();
                this.propertyGetter = property.GetGetMethod();
            }

            private static PropertyInfo GetProperty(MethodInfo propertyGetterOrSetter)
            {
                return
                    (from property in propertyGetterOrSetter.DeclaringType.GetProperties()
                     let getMethod = property.GetGetMethod()
                     let setMethod = property.GetSetMethod()
                     where (getMethod != null && getMethod.GetBaseDefinition().Equals(propertyGetterOrSetter.GetBaseDefinition()))
                         || (setMethod != null && setMethod.GetBaseDefinition().Equals(propertyGetterOrSetter.GetBaseDefinition()))
                     select property).Single();
            }

            
            private bool IsPropertySetter(IFakeObjectCall fakeObjectCall)
            {
                return this.propertySetter != null && this.propertySetter.GetBaseDefinition().Equals(fakeObjectCall.Method.GetBaseDefinition());
            }

            private bool IsPropertyGetter(IFakeObjectCall fakeObjectCall)
            {
                return this.propertyGetter != null && this.propertyGetter.GetBaseDefinition().Equals(fakeObjectCall.Method.GetBaseDefinition());
            }

            [DebuggerStepThrough]
            public static bool IsPropertySetter(MethodInfo method)
            {
                return method.DeclaringType.GetProperties().Any(x => object.Equals(method, x.GetSetMethod()));
            }

            [DebuggerStepThrough]
            public static bool IsPropertyGetter(MethodInfo method)
            {
                return method.DeclaringType.GetProperties().Any(x => object.Equals(method, x.GetGetMethod()));
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return this.IsPropertySetter(fakeObjectCall) || this.IsPropertyGetter(fakeObjectCall);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                if (this.IsPropertyGetter(fakeObjectCall))
                {
                    fakeObjectCall.SetReturnValue(this.Value);
                }
                else
                {
                    this.Value = fakeObjectCall.Arguments[0];
                }
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }
    }
}
