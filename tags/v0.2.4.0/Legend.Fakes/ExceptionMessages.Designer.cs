﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4016
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Legend.Fakes {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Legend.Fakes.ExceptionMessages", typeof(ExceptionMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Apply method of the ExpressionInterceptor may no be called before the Applicator property has been set..
        /// </summary>
        internal static string ApplicatorNotSetExceptionMessage {
            get {
                return ResourceManager.GetString("ApplicatorNotSetExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified argument name does not exist in the ArgumentList..
        /// </summary>
        internal static string ArgumentNameDoesNotExist {
            get {
                return ResourceManager.GetString("ArgumentNameDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An argument validation was not configured correctly..
        /// </summary>
        internal static string ArgumentValidationDefaultMessage {
            get {
                return ResourceManager.GetString("ArgumentValidationDefaultMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The validator type &apos;{0}&apos; specified in the ArgumentValidatorAttribute does not have a constructor with a signature that matches the argument validation extension method, the signature of the constructor needs to be {1}..
        /// </summary>
        internal static string ArgumentValidatorConstructorSignatureDoesntMatchValidationMethod {
            get {
                return ResourceManager.GetString("ArgumentValidatorConstructorSignatureDoesntMatchValidationMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of generic arguments to the validation extension method was {0} but the number of generic arguments to the argument validator type specified in the ArgumentValidatorAttribute was {1}..
        /// </summary>
        internal static string ArgumentValidatorTypeArgumentsDoesntMatchValidationMethod {
            get {
                return ResourceManager.GetString("ArgumentValidatorTypeArgumentsDoesntMatchValidationMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The method &apos;{0}&apos; was called too few times, expected #{1} times but was called #{2} times..
        /// </summary>
        internal static string CalledTooFewTimesMessage {
            get {
                return ResourceManager.GetString("CalledTooFewTimesMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The method &apos;{0}&apos; was called too many times, expected #{1} times but was called #{2} times..
        /// </summary>
        internal static string CalledTooManyTimesMessage {
            get {
                return ResourceManager.GetString("CalledTooManyTimesMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only fake objects can be configured with the Fake.Configure method..
        /// </summary>
        internal static string ConfiguringNonFakeObjectExceptionMessage {
            get {
                return ResourceManager.GetString("ConfiguringNonFakeObjectExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only abstract classes can be faked using the A.Fake-method that takes an enumerable of objects as arguments for constructor, use the overload that takes an expression instead..
        /// </summary>
        internal static string FakingNonAbstractClassWithArgumentsForConstructor {
            get {
                return ResourceManager.GetString("FakingNonAbstractClassWithArgumentsForConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The member accessor expression must be a lambda expression with a MethodCallExpression or MemberAccessExpression as its body..
        /// </summary>
        internal static string MemberAccessorNotCorrectExpressionType {
            get {
                return ResourceManager.GetString("MemberAccessorNotCorrectExpressionType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No constructor matching the specified arguments was found on the type {0}..
        /// </summary>
        internal static string NoConstructorMatchingArguments {
            get {
                return ResourceManager.GetString("NoConstructorMatchingArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can not generate fake object for the class since no default constructor was found, specify a constructor call..
        /// </summary>
        internal static string NoDefaultConstructorMessage {
            get {
                return ResourceManager.GetString("NoDefaultConstructorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The validator type specified in the ArgumentValidatorAttribute must be a type that implements the IArgumentValidator-interface, the specified type &apos;{0}&apos; does not implement the interface..
        /// </summary>
        internal static string NonArgumentValidatorTypeExceptionMessage {
            get {
                return ResourceManager.GetString("NonArgumentValidatorTypeExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only expression of the type ExpressionType.New (constructor calls) are accepted..
        /// </summary>
        internal static string NonConstructorExpressionMessage {
            get {
                return ResourceManager.GetString("NonConstructorExpressionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Now-method on the event raise is not meant to be called directly, only use it to register to an event on a fake object that you want to be raised..
        /// </summary>
        internal static string NowCalledDirectly {
            get {
                return ResourceManager.GetString("NowCalledDirectly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can not generate fake objects for sealed classes..
        /// </summary>
        internal static string TypeIsSealedMessage {
            get {
                return ResourceManager.GetString("TypeIsSealedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of argument names does not match the number of arguments..
        /// </summary>
        internal static string WrongNumberOfArgumentNamesMessage {
            get {
                return ResourceManager.GetString("WrongNumberOfArgumentNamesMessage", resourceCulture);
            }
        }
    }
}
