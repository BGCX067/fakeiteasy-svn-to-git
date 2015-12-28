using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Castle.DynamicProxy;
using System.Diagnostics.CodeAnalysis;
using System.Resources;
using System;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Legend.Fakes")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Legend.Fakes")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Legend.Fakes.Tests")]
[assembly: InternalsVisibleTo("Legend.Fakes.RhinoCompatibility")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: CLSCompliant(true)]

[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Legend.Fakes.ExtensionSyntax")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Legend.Fakes.VisualBasic")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Legend.Fakes.Extensibility")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Legend.Fakes.Assertion")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2622059a-d73a-4066-9d7f-eb545a79ca5e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.2.4.0")]
[assembly: AssemblyFileVersion("0.2.4.0")]
