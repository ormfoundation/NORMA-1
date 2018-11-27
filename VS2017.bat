@CALL "%~dp0VSVer.bat" 2017

@ECHO OFF

:: Update the path for dlls needed by the build, we don't want to add them to the GAC
FOR /f "usebackq tokens=*" %%i IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) DO (
	SET VSInstallDir=%%i
)
CALL:EXTENDPATH "%VSInstallDir%\Common7\IDE\PublicAssemblies"
CALL:EXTENDPATH "%VSInstallDir%\VSSDK\VisualStudioIntegration\Common\Assemblies\v4.0"
CALL:EXTENDPATH "%VSInstallDir%\MSBuild\15.0\Bin"
CALL:EXTENDPATH "%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin"

SET VsctPath=%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin\vsct.exe

:: Set Machine Level Environment Variables so that individual projects can be opended via Visual Studio
SETX TargetVisualStudioVersion v15.0 /M
SETX TargetFrameworkVersion v4.6 /M
SETX TargetVisualStudioAssemblyVersion 15.0.0.0 /M
SETX TargetVisualStudioAssemblySuffix .15.0 /M
SETX TargetVisualStudioFrameworkAssemblyVersion 4.6.0.0 /M
SETX TargetVisualStudioFrameworkAssemblySuffix .15.0 /M
SETX TargetVisualStudioLongProductYear 2017 /M
SETX TargetVisualStudioShortProductYear 17 /M
SETX TargetVisualStudioLongProductName "Visual Studio 2017" /M
SETX TargetVisualStudioShortProductName VS2017 /M
SETX TargetVisualStudioMajorMinorVersion 15.0 /M
SETX TargetDslToolsAssemblyVersion 15.0.0.0 /M
SETX TargetDslToolsVersionSuffix .15.0 /M
SETX ProjectToolsVersion 15.0 /M
SETX ProjectToolsAssemblySuffix .Core /M
SETX ProjectToolsAssemblyVersion 15.1.0.0 /M
SETX GeneratedDslFileSuffix .VS2010 /M
SETX UseGAC FALSE /M
SETX VsctPath "%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin\vsct.exe" /M

goto:EOF

:EXTENDPATH
SET PATH=%~1;%PATH%
GOTO:EOF