using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;
#if __ANDROID__
using Android.App;
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if __ANDROID__
[assembly: AssemblyDescription("Appercode UI Framework Library for Android")]
[assembly: AssemblyTitle("Appercode UI Framework Library for Android")]
[assembly: Guid(@"9031DD7D-D1AF-40B1-A90A-26E977B9F711")]
[assembly: InternalsVisibleTo("Appercode.UIFramework.Maps.Droid")]
#elif WINDOWS_PHONE
[assembly: AssemblyDescription("Appercode UI Framework Library for Windows Phone")]
[assembly: AssemblyTitle("Appercode UI Framework Library for Windows Phone")]
[assembly: Guid(@"9031DD7D-D1AF-40B1-A90A-26E977B9F712")]
[assembly: InternalsVisibleTo("Appercode.UIFramework.Maps.WP")]
#elif __IOS__
[assembly: AssemblyDescription("Appercode UI Framework Library for iOS")]
[assembly: AssemblyTitle("Appercode UI Framework Library for iOS")]
[assembly: Guid(@"9031DD7D-D1AF-40B1-A90A-26E977B9F710")]
[assembly: InternalsVisibleTo("Appercode.UIFramework.Maps.iOS")]
#else
[assembly: AssemblyTitle("Appercode UI Framework Abstract Library")]
[assembly: AssemblyDescription("Appercode UI Framework Abstract Library")]
[assembly: Guid(@"9031DD7D-D1AF-40B1-A90A-26E977B9F700")]
// [assembly: InternalsVisibleTo("Appercode.UIFramework.Abstract.Tests, PublicKeyToken=ee7983d40e65a548")]
#endif

[assembly: AssemblyCompany("Appercode")]
[assembly: AssemblyProduct("Appercode.UIFramework")]
[assembly: AssemblyCopyright("Copyright © Appercode 2015")]
[assembly: ComVisible(false)]

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
[assembly: AssemblyVersion("0.9.2.4600")]
[assembly: AssemblyFileVersion("0.9.2.4600")]

[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "Appercode.UI")]
[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "Appercode.UI.Controls")]
[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "Appercode.UI.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "Appercode.UI.Data")]
[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "Appercode.UI.Maps.Controls")]
[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "Appercode.UI.Markup")]
[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "Appercode.UI.StylesAndResources")]
[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "System.Drawing")]
[assembly: XmlnsDefinition("http://schemas.appercode.com/2013", "System.Windows")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "System.Windows.Markup")]
// [assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml", "x")]

// Add some common permissions, these can be removed if not needed
#if __ANDROID__
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
#endif