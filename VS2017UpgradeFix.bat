@ECHO OFF

ECHO This file should no longer be needed since we moved away from the GAC for the NORMA Builds.

REM Add SDK tools to path if vsct cannot be found, fails to compile in VS2015 otherwise
REM This is still need due to VCSTCompress.dll lacking an assembly manifest so we can't add it to the GAC
REM FOR /f "usebackq tokens=*" %%i IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -version [%TargetVisualStudioMajorMinorVersion%^,%TargetVisualStudioMajorMinorNextVersion%^) -products * -requires Microsoft.Component.MSBuild -property installationPath`) DO (
REM 	SET VSInstallDir=%%i
REM )
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.Shell.Interop.8.0.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.Shell.Interop.9.0.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.Shell.Interop.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.Shell.15.0.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.Shell.Framework.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.OLE.Interop.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.TextTemplating.Interfaces.11.0.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.TextTemplating.15.0.dll" /f
REM gacutil /i "%VSInstallDir%\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.TextManager.Interop.dll" /f
REM gacutil /i "%VSInstallDir%\VSSDK\VisualStudioIntegration\Common\Assemblies\v4.0\Microsoft.VisualStudio.Utilities.dll" /f
REM gacutil /i "%VSInstallDir%\MSBuild\15.0\Bin\Microsoft.Build.Framework.dll" /f
REM gacutil /i "%VSInstallDir%\MSBuild\15.0\Bin\Microsoft.Build.Tasks.Core.dll" /f
REM gacutil /i "%VSInstallDir%\MSBuild\15.0\Bin\Microsoft.Build.Utilities.Core.dll" /f
REM gacutil /i "%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin\VSCTLibrary.dll" /f
REM gacutil /i "%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin\VSCT.exe" /f
REM gacutil.exe /i "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VSSDK\VisualStudioIntegration\Tools\Bin\VSCTCompress.dll" /f