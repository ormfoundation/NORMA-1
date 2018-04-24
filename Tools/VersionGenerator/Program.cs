#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright � Neumont University. All rights reserved.                     *
* Copyright � The ORM Foundation. All rights reserved.                     *
*                                                                          *
* The use and distribution terms for this software are covered by the      *
* Common Public License 1.0 (http://opensource.org/licenses/cpl) which     *
* can be found in the file CPL.txt at the root of this distribution.       *
* By using this software in any fashion, you are agreeing to be bound by   *
* the terms of this license.                                               *
*                                                                          *
* You must not remove this notice, or any other, from this software.       *
\**************************************************************************/
#endregion

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Neumont.Tools.ORM.SDK
{
	internal static partial class VersionGenerator
	{
		private static int Main()
		{
			const string generatedWarning = " This file was generated by VersionGenerator.exe. It should NOT be directly modified. ";
			const string statusPrefix = "VersionGenerator.exe: ";

			Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
			FileInfo versionConfig = new FileInfo("VersionGenerator.exe.config");
			if (!versionConfig.Exists)
			{
				// If we don't have the configuration file, bad things are going to happen...
				Console.Error.WriteLine(statusPrefix + "VersionGenerator.exe.config not found!");
				return 1;
			}
			DateTime versionConfigLastModified = versionConfig.LastWriteTime;

			DateTime today = DateTime.Today;
			int build = ((Config.ReleaseYearMonth.Year - 2000) * 100) + Config.ReleaseYearMonth.Month;
			int month = ((today.Year - Config.RevisionStartYearMonth.Year) * 12) + (today.Month - Config.RevisionStartYearMonth.Month) + 1;
			int monthsAsQuarters = ((today.Year - Config.CountQuartersFromYearMonth.Year) * 12) + (today.Month - Config.CountQuartersFromYearMonth.Month) + 1;
			int revision;
			if (monthsAsQuarters > 0)
			{
				// This revision mechanism was moving much too quickly, so allow the
				// option to increment by quarter instead of month. For quarter increments,
				// days 1-31 are the first month, 34-64 are the second month, and 67-97 are
				// the third month in the quarter. Months before quarter counting began are
				// added to the quarter count, giving sequential version numbers.
				revision = ((month - monthsAsQuarters) + (monthsAsQuarters + 2) / 3) * 100;
				switch ((monthsAsQuarters - 1) % 3)
				{
					case 1:
						revision += 33;
						break;
					case 2:
						revision += 66;
						break;
				}
			}
			else
			{
				revision = month * 100;
			}
			revision += today.Day;

			string unquotedFileVersion = string.Format("{0}.{1}.{2}.{3}", Config.MajorVersion, Config.MinorVersion, build, revision);
			string quotedInformationalVersion = string.Format("\"{0} {1:yyyy-MM}{2}\"", unquotedFileVersion, Config.ReleaseYearMonth, Config.ReleaseType);
			string quotedProductVersion = string.Format("\"{0}.{1}.0.0\"", Config.MajorVersion, Config.MinorVersion);
			string quotedReleaseDescription = string.Format("\"{0:yyyy-MM} {1}\"", Config.ReleaseYearMonth, Config.ReleaseType);

			#region Version.cs
			FileInfo versionCS = new FileInfo("Version.cs");
			if (!versionCS.Exists || versionCS.LastWriteTime.Date != today || versionCS.LastWriteTime < versionConfigLastModified)
			{
				using (StreamWriter writer = versionCS.CreateText())
				{
					writer.WriteLine("/*" + generatedWarning + "*/");
					writer.Write("[assembly: System.Reflection.AssemblyFileVersion(\"");
					writer.Write(unquotedFileVersion);
					writer.WriteLine("\")]");
					writer.Write("[assembly: System.Reflection.AssemblyInformationalVersion(");
					writer.Write(quotedInformationalVersion);
					writer.WriteLine(")]");
					writer.Write("[assembly: System.Reflection.AssemblyVersion(");
					writer.Write(quotedProductVersion);
					writer.WriteLine(")]");
					writer.Write("[assembly: System.Resources.SatelliteContractVersion(");
					writer.Write(quotedProductVersion);
					writer.WriteLine(")]");
				}
				Console.WriteLine(statusPrefix + "Generated Version.cs.");
			}
			else
			{
				Console.WriteLine(statusPrefix + "Version.cs already up to date.");
			}
			#endregion

			#region Version.wxi
			FileInfo versionWXI = new FileInfo("Version.wxi");
			if (!versionWXI.Exists || versionWXI.LastWriteTime.Date != today || versionWXI.LastWriteTime < versionConfigLastModified)
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.IndentChars = "\t";
				settings.CloseOutput = true;
				using (XmlWriter writer = XmlWriter.Create(versionWXI.CreateText(), settings))
				{
					writer.WriteStartDocument();
					writer.WriteComment(generatedWarning);
					writer.WriteStartElement("Include", "http://schemas.microsoft.com/wix/2006/wi");
					writer.WriteProcessingInstruction("define", string.Format("MajorMinorVersion=\"{0}.{1}\"", Config.MajorVersion, Config.MinorVersion));
					writer.WriteProcessingInstruction("define", string.Format("MajorVersionHexits=\"{0:d2}\"", Config.MajorVersion));
					writer.WriteProcessingInstruction("define", string.Format("BuildVersion=\"{0}\"", build));
					writer.WriteProcessingInstruction("define", string.Format("RevisionVersion=\"{0}\"", revision));
					writer.WriteProcessingInstruction("define", string.Format("ProductVersion=\"{0}\"", unquotedFileVersion));
					writer.WriteProcessingInstruction("define", string.Format("VersionGuidSuffix=\"$(var.Debug)$(var.ExperimentalHive)$(var.Architecture)-$(var.MajorVersionHexits){0:d2}{1:d4}{2:d4}\"", Config.MinorVersion, build, revision));
					writer.WriteProcessingInstruction("define", "ReleaseDescription=" + quotedReleaseDescription);
					writer.WriteEndElement();
					writer.WriteEndDocument();
				}
				Console.WriteLine(statusPrefix + "Generated Version.wxi.");
			}
			else
			{
				Console.WriteLine(statusPrefix + "Version.wxi already up to date.");
			}
			#endregion

			#region Version.bat
			FileInfo versionBAT = new FileInfo("Version.bat");
			if (!versionBAT.Exists || versionBAT.LastWriteTime.Date != today || versionBAT.LastWriteTime < versionConfigLastModified)
			{
				using (StreamWriter writer = versionBAT.CreateText())
				{
					writer.WriteLine(":: " + generatedWarning);
					writer.Write("@SET ProductMajorVersion=");
					writer.WriteLine(Config.MajorVersion);
					writer.Write("@SET ProductMinorVersion=");
					writer.WriteLine(Config.MinorVersion);
					writer.Write("@SET ProductBuildVersion=");
					writer.WriteLine(build);
					writer.Write("@SET ProductRevisionVersion=");
					writer.WriteLine(revision);
				}
				Console.WriteLine(statusPrefix + "Generated Version.bat.");
			}
			else
			{
				Console.WriteLine(statusPrefix + "Version.bat already up to date.");
			}
			#endregion

			Console.WriteLine("VersionGenerator.exe finished successfully.");
			return 0;
		}
	}
}
