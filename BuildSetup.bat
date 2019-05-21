@ECHO OFF
SETLOCAL
SET RootDir=%~dp0.
CALL "%RootDir%\SetupEnvironment.bat" %*

:: HelpStudio Lite is not installed anymore with VS SDK
IF %TargetVisualStudioMajorMinorVersion% LSS 15.0 (
	CALL "%RootDir%\BuildHelp.bat" %*
)
MSBuild.exe /nologo "%RootDir%\Setup.proj" %*