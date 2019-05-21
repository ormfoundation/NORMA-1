@ECHO OFF
SETLOCAL
SET RootDir=%~dp0.

IF "%TargetVisualStudioVersion%"=="v8.0" (
	SET DegradeToolsVersion=/toolsversion:2.0
) ELSE IF "%TargetVisualStudioVersion%"=="v9.0" (
	SET DegradeToolsVersion=/toolsversion:3.5
) ELSE IF "%TargetVisualStudioVersion%"=="v10.0" (
	SET DegradeToolsVersion=/toolsversion:4.0
) ELSE IF "%TargetVisualStudioVersion%"=="v11.0" (
	SET DegradeToolsVersion=/toolsversion:4.0
) ELSE IF "%TargetVisualStudioVersion%"=="v12.0" (
	SET DegradeToolsVersion=/toolsversion:12.0
) ELSE IF "%TargetVisualStudioVersion%"=="v14.0" (
	SET DegradeToolsVersion=/toolsversion:14.0
) ELSE (
	SET TargetVisualStudioVersion=v15.0
	SET TargetVisualStudioMajorMinorVersion=15.0
	SET TargetVisualStudioMajorMinorNextVersion=16.0
	SET DegradeToolsVersion=/toolsversion:%ProjectToolsVersion%
)

FOR /f "usebackq tokens=*" %%i IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -version [%TargetVisualStudioMajorMinorVersion%^,%TargetVisualStudioMajorMinorNextVersion%^) -products * -requires Microsoft.Component.MSBuild -property installationPath`) DO (
	SET VSInstallDir=%%i
)
CALL "%RootDir%\BuildDevTools.bat" %* /consoleloggerparameters:DisableMPLogging %DegradeToolsVersion%
CALL "%RootDir%\Build.bat" %* /p:"ReferencePath=%VSInstallDir%\Common7\IDE\PrivateAssemblies;%VSInstallDir%\Common7\IDE\PublicAssemblies;%VSInstallDir%\VSSDK\VisualStudioIntegration\Common\Assemblies\v4.0;%VSInstallDir%\MSBuild\15.0\Bin;%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin" /consoleloggerparameters:DisableMPLogging %DegradeToolsVersion%

GOTO:EOF
