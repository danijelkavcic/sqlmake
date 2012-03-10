using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SQLMake")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Mitja Golouh")]
[assembly: AssemblyProduct("SQLMake")]
[assembly: AssemblyCopyright("Copyright © Mitja Golouh 2008-2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7389d82f-8aeb-44b1-afaf-ef2938dac1b0")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]




// OK there are a number of posts out there about this error, and many of them recommend ‘linking’ the suspect class 
// into you test project. This is not the best solution. The best approach is to add the following to 
// you assemblyinfo.cs.
// [assembly: InternalsVisibleTo("NameOfTestDllWithoutExtension")]
// Look here from more details http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.internalsvisibletoattribute.aspx

[assembly: InternalsVisibleTo("SQLMakeTest")]