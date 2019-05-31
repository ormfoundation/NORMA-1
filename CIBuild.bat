@ECHO OFF
SET RootDir=%~dp0.

rem start of block
rem see https://help.appveyor.com/discussions/questions/6160-ho-to-run-batch-script-within-vs-developer-prompt
rem also see https://github.com/microsoft/visualfsharp/pull/2690/files
REM load Visual Studio 2017 developer command prompt if VS150COMNTOOLS is not set
CALL "%RootDir%\CIVS%VISUAL_STUDIO_VERSION%Prompt.bat"
rem end of block

CALL "%RootDir%\VS%VISUAL_STUDIO_VERSION%.bat"
CALL "%RootDir%\SetupEnvironment.bat"

CALL "%RootDir%\FirstTimeBuildVS%VISUAL_STUDIO_VERSION%.bat"
CALL "%RootDir%\BuildDevToolsVS%VISUAL_STUDIO_VERSION%.bat" /t:clean,build
CALL "%RootDir%\BuildVS%VISUAL_STUDIO_VERSION%.bat" /t:clean,build

rem NOTE: AppVeyor does not allow us to access environment variables after
rem the build, so here goes
CALL "%RootDir%\BuildSetupVS%VISUAL_STUDIO_VERSION%.bat" /t:clean,build
