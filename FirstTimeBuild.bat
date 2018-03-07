:: Do not call directly for VS2017+ as the paths for the private assembly references will not be configured correctly

@ECHO OFF
SETLOCAL
SET RootDir=%~dp0.

CALL "%RootDir%\BuildDevTools.bat" %*
CALL "%RootDir%\Build.bat" %*

CALL "%RootDir%\SetupEnvironment.bat" %*
ECHO.
ECHO Running 'devenv.exe /RootSuffix "%VSRegistryRootSuffix%" /Setup'... This may take a few minutes...
"%VSEnvironmentPath%" /RootSuffix "%VSRegistryRootSuffix%" /Setup

GOTO:EOF
