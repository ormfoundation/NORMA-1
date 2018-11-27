@ECHO OFF
SETLOCAL
SET RootDir=%~dp0.
CALL "%RootDir%\..\..\SetupEnvironment.bat" %*

IF NOT EXIST "%MSBuildExtensionsPath%\Neumont" (MKDIR "%MSBuildExtensionsPath%\Neumont")

if EXIST "%VSDIR%" (
	XCOPY /Y /D /V /Q "%RootDir%\Tasks\RegexCompilationInfo.xsd" "%VSDir%\Xml\Schemas\"
)

XCOPY /Y /D /V /Q "%RootDir%\Neumont.Build.targets" "%MSBuildExtensionsPath%\Neumont\"

FOR /F "usebackq skip=3 tokens=*" %%A IN (`REG QUERY "HKLM\%VSRegistryRootBase%"`) DO (REG QUERY "%%~A\MSBuild\SafeImports" 1>NUL 2>&1 && IF NOT ERRORLEVEL 1 (REG ADD "%%~A\MSBuild\SafeImports" /v "NUBuild1" /d "%MSBuildExtensionsPath%\Neumont\Neumont.Build.targets" /f))

::Remove legacy versions
gacutil.exe /nologo /u Neumont.Build,Version=1.0.0.0 1>NUL 2>&1
gacutil.exe /nologo /u Neumont.Build.VisualStudio,Version=1.0.0.0 1>NUL 2>&1

:: Make sure the MS Build dlls are in the GAC
if '%UseGAC%'=='TRUE' (
	gacutil.exe /l "Microsoft.Build.Framework, Version=%ProjectToolsAssemblyVersion%, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL" | findstr /c:"Number of items = 0"
	IF NOT ERRORLEVEL 1 (
		gacutil.exe /nologo /f /i "%MSBuildBinPath%Microsoft.Build.Framework.dll"
	)
	gacutil.exe /l "Microsoft.Build.Tasks%ProjectToolsAssemblySuffix%, Version=%ProjectToolsAssemblyVersion%, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL" | findstr /c:"Number of items = 0"
	IF NOT ERRORLEVEL 1 (
		gacutil.exe /nologo /f /i "%MSBuildBinPath%Microsoft.Build.Tasks%ProjectToolsAssemblySuffix%.dll"
	)
	gacutil.exe /l "Microsoft.Build.Utilities%ProjectToolsAssemblySuffix%, Version=%ProjectToolsAssemblyVersion%, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL" | findstr /c:"Number of items = 0"
	IF NOT ERRORLEVEL 1 (
		gacutil.exe /nologo /f /i "%MSBuildBinPath%Microsoft.Build.Utilities%ProjectToolsAssemblySuffix%.dll"
	)
)

::Add new version. We can't do this in the project because we're building the utility that does it.
::Obviously, the utilities need to be on the path for this to work, which will happen automatically in
::a Visual Studio command prompt.
gacutil.exe /nologo /f /i "%RootDir%\bin\Neumont.Build%ProjectToolsAssemblySuffix%.dll"
if '%UseGAC%'=='TRUE' (
	ngen.exe install "Neumont.Build%ProjectToolsAssemblySuffix%, Version=%ProjectToolsAssemblyVersion%, Culture=neutral, PublicKeyToken=957d5b7d5e79e25f" /NoDependencies /nologo /verbose
)

GOTO:EOF
