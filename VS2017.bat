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
SETX TargetVisualStudioVersion v15.0 /M > NUL
SETX TargetFrameworkVersion v4.6 /M > NUL
SETX TargetVisualStudioAssemblyVersion 15.0.0.0 /M > NUL
SETX TargetVisualStudioAssemblySuffix .15.0 /M > NUL
SETX TargetVisualStudioFrameworkAssemblyVersion 4.6.0.0 /M > NUL
SETX TargetVisualStudioFrameworkAssemblySuffix .15.0 /M > NUL
SETX TargetVisualStudioLongProductYear 2017 /M > NUL
SETX TargetVisualStudioShortProductYear 17 /M > NUL
SETX TargetVisualStudioLongProductName "Visual Studio 2017" /M > NUL
SETX TargetVisualStudioShortProductName VS2017 /M > NUL
SETX TargetVisualStudioMajorMinorVersion 15.0 /M > NUL
SETX TargetDslToolsAssemblyVersion 15.0.0.0 /M > NUL
SETX TargetDslToolsVersionSuffix .15.0 /M > NUL
SETX ProjectToolsVersion 15.0 /M > NUL
SETX ProjectToolsAssemblySuffix .Core /M > NUL
SETX ProjectToolsAssemblyVersion 15.1.0.0 /M > NUL
SETX GeneratedDslFileSuffix .VS2010 /M > NUL
SETX UseGAC FALSE /M > NUL
SETX VsctPath "%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin\vsct.exe" /M > NUL

goto:EOF

:EXTENDPATH
SET PATH=%~1;%PATH%
GOTO:EOF