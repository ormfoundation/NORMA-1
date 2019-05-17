using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle("Neumont.Build.VisualStudio.dll")]
[assembly: AssemblyProduct("Neumont Build System")]
[assembly: AssemblyDescription("Neumont Build System - Visual Studio Targets DLL")]

[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]
[assembly: AssemblyVersion(
#if NET_4_7  // We can't use the Tools Version for MSBuild 16.0 because the Tools Version for this is now 4.0 and there is a sub version
"16.0.0.0"
#elif TOOLS_2_0
"2.0.0.0"
#elif TOOLS_3_5
"3.5.0.0"
#elif TOOLS_4_0
"4.0.0.0"
#elif TOOLS_12_0
"12.0.0.0"
#elif TOOLS_14_0
"14.0.0.0"
#elif TOOLS_15_0
"15.1.0.0"
#else
NEW_TOOLS_VERSION
#endif
)]
