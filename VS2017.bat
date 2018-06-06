@CALL "%~dp0VSVer.bat" 2017

@ECHO OFF

:: Add Nuget packages to GAC
ECHO "Checking GAC for Nuget Assemblies..."
gacutil.exe /l "Microsoft.VisualStudio.TextTemplating.Interfaces.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL" | findstr /c:"Number of items = 0"
IF NOT ERRORLEVEL 1 (
    gacutil.exe /nologo /f /i "%~dp0%Nuget\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0.10.0.30320\lib\net40\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0.dll"
)
gacutil.exe /l "Microsoft.VisualStudio.TextTemplating.Interfaces.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL" | findstr /c:"Number of items = 0"
IF NOT ERRORLEVEL 1 (
    gacutil.exe /nologo /f /i "%~dp0%Nuget\Microsoft.VisualStudio.TextTemplating.Interfaces.11.0.11.0.50728\lib\net45\Microsoft.VisualStudio.TextTemplating.Interfaces.11.0.dll"
)
gacutil.exe /l "Microsoft.VisualStudio.TextTemplating.15.0, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL" | findstr /c:"Number of items = 0"
IF NOT ERRORLEVEL 1 (
    gacutil.exe /nologo /f /i "%~dp0%Nuget\Microsoft.VisualStudio.TextTemplating.15.0.15.6.27413\lib\net45\Microsoft.VisualStudio.TextTemplating.15.0.dll"
)

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