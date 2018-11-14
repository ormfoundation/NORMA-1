@CALL "%~dp0VSVer.bat" 2017

@ECHO OFF

:: Update the path for dlls needed by the build, we don't want to add them to the GAC
FOR /f "usebackq tokens=*" %%i IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) DO (
	SET VSInstallDir=%%i
)
::Add SDK tools to path if vsct cannot be found, fails to compile in VS2015 otherwise

for %%i in (Microsoft.VisualStudio.Shell.15.0.dll) do (set INPATH=%%~s$PATH:i)
if '%INPATH%'=='' (
	CALL:EXTENDPATH "%VSInstallDir%\Common7\IDE\PublicAssemblies"
)
SET INPATH=
for %%i in (Microsoft.VisualStudio.Utilities.dll) do (set INPATH=%%~s$PATH:i)
if '%INPATH%'=='' (
	CALL:EXTENDPATH "%VSInstallDir%\VSSDK\VisualStudioIntegration\Common\Assemblies\v4.0"
)
SET INPATH=
for %%i in (Microsoft.Build.Framework.dll) do (set INPATH=%%~s$PATH:i)
if '%INPATH%'=='' (
	CALL:EXTENDPATH "%VSInstallDir%\MSBuild\15.0\Bin"
)
SET INPATH=
for %%i in (vsct.exe) do (set INPATH=%%~s$PATH:i)
if '%INPATH%'=='' (
	CALL:EXTENDPATH "%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin"
)
SET INPATH=
goto:EOF

:EXTENDPATH
SET PATH=%PATH%;%~1
GOTO:EOF