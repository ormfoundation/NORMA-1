@CALL "%~dp0VSVer.bat" 2017

@ECHO OFF
::Add SDK tools to path if vsct cannot be found, fails to compile in VS2015 otherwise
::This is still need due to VCSTCompress.dll lacking an assembly manifest so we can't add it to the GAC
FOR /f "usebackq tokens=*" %%i IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) DO (
	SET VSInstallDir=%%i
)
::Add SDK tools to path if vsct cannot be found, fails to compile in VS2015 otherwise
for %%i in (vsct.exe) do (set EXISTINGVSCT=%%~s$PATH:i)
if '%EXISTINGVSCT%'=='' (
	CALL:EXTENDPATH "%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin"
)
SET EXISTINGVSCT=
goto:EOF

:EXTENDPATH
SET PATH=%PATH%;%~1
GOTO:EOF