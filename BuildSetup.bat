@ECHO OFF
SETLOCAL
SET RootDir=%~dp0.
CALL "%RootDir%\SetupEnvironment.bat" %*

:: HelpStudio Lite is not installed anymore with VS SDK
IF %TargetVisualStudioMajorMinorVersion% LSS 15.0 (
	CALL "%RootDir%\BuildHelp.bat" %*
)
:: If VSWhere was used then we need to pass in the private asssembly location - this is the newer VS
CALL:SetupFrom%VSInformationSource% %*

GOTO:EOF

:SetupFromRegistry
MSBuild.exe /nologo "%RootDir%\Setup.proj" %*
GOTO:EOF

:SetupFromVSWhere
FOR /f "usebackq tokens=*" %%i IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) DO (
	SET VSInstallDir=%%i
)
MSBuild.exe /nologo "%RootDir%\Setup.proj" /p:"ReferencePath=%VSInstallDir%\Common7\IDE\PrivateAssemblies" %*
GOTO:EOF