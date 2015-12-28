﻿using System;
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
            private FakeObject fakeObject;

            public object Value;

            public PropertyBehaviorRule(MethodInfo propertyGetterOrSetter, FakeObject fakeObject)
            {
                this.fakeObject = fakeObject;
                var property = GetProperty(propertyGetterOrSetter);

                this.propertySetter = property.GetSetMethod();
                this.propertyGetter = property.GetGetMethod(true);
            }

            private static PropertyInfo GetProperty(MethodInfo propertyGetterOrSetter)
            {
                return
                    (from property in propertyGetterOrSetter.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                     let getMethod = property.GetGetMethod(true)
                     let setMethod = property.GetSetMethod(true)
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

            public static bool IsPropertySetter(MethodInfo method)
            {
                return method.IsSpecialName && method.Name.StartsWith("set_");
            }

            public static bool IsPropertyGetter(MethodInfo method)
            {
                return method.IsSpecialName && method.Name.StartsWith("get_");
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

                this.fakeObject.MoveRuleToFront(this);
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }
    }
}
