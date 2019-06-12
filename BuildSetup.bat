@ECHO OFF
SETLOCAL
SET RootDir=%~dp0.
REM CALL "%RootDir%\SetupEnvironment.bat" %*

:: HelpStudio Lite is not installed anymore with VS SDK
IF %TargetVisualStudioMajorMinorVersion% LSS 15.0 (
	CALL "%RootDir%\BuildHelp.bat" %*
)
MSBuild.exe /nologo "%RootDir%\Setup.proj" %*

REM Build the Installation Zip File
ECHO Building Installation Zip File
SETLOCAL ENABLEDELAYEDEXPANSION
REM See if the configuration has been set to release, if so we need to look in the
REM See if we are in Debug or Release mode
SET parameters=%*
SET parameters=%parameters:"=%
CALL :l_replace "%parameters%" "=" ""
SET parameters=!str!
REM Now that we have removed problematic characters release mode should look like ConfigurationRelease in the parameters
SET testString=%parameters:ConfigurationRelease=%
IF "%parameters%"=="%testString%" (
	SET BuildConfiguration=Debug
) ELSE (
	SET BuildConfiguration=Release
)
ECHO Output folder set to: %BuildConfiguration%
REM Try to find PLiX
IF %TargetVisualStudioMajorMinorVersion% GEQ 15.0 (
	FOR /f "delims=" %%i IN ('dir /B ^"%RootDir%\..\*PLiX*^"') DO (
		SET "PLiXPath=%RootDir%\..\%%i"
		IF EXIST "!PLiXPath!\!TargetVisualStudioShortProductName!.bat" (
			IF EXIST "!PLiXPath!\BuildAll.bat" (
				SET "PLiXPathPreBuild=!PLiXPath!\!TargetVisualStudioShortProductName!.bat"
				SET "PLiXPathBuild=!PLiXPath!\BuildAll.bat"
			)
		)
	)
	IF "!PLiXPath!"=="" (
		ECHO Unable to locate PLiX project...
		GOTO:EOF
	)
	IF "!PLiXPathPreBuild!"=="" (
		ECHO Unable to locate PLiX Pre Build bat file...
		GOTO:EOF
	)
	IF "!PLiXPathBuild!"=="" (
		ECHO Unable to locate PLiX Build bat file...
		GOTO:EOF
	)
	ECHO Found PLiX: !PLiXPath!
	ECHO Using PLiX Pre Build File: !PLiXPathPreBuild!
	ECHO Using PLiX Build All File: !PLiXPathBuild!
)
REM Build PLiX
ECHO Building PLiX...
CD "!PLiXPath!"
ECHO Executing !PLiXPathPreBuild!...
CALL "!PLiXPathPreBuild!"
ECHO Executing !PLiXPathBuild!...
CALL "!PLiXPathBuild!" /t:clean,build /p:Configuration=!BuildConfiguration!
REM Gather Files
CD "!RootDir!"
SET "InstallationFolder=%RootDir%\..\NORMA%TargetVisualStudioShortProductName%Installation"
IF NOT EXIST "%InstallationFolder%\" GOTO NOINSTALLDIR
ECHO Cleaning Up Previous Installation Folder: !InstallationFolder!
RMDIR "%InstallationFolder%" /S /Q
:NOINSTALLDIR
SET "InstallationZipFile=%InstallationFolder%.zip"
IF NOT EXIST "%InstallationZipFile%" GOTO NOINSTALLFILE
ECHO Cleaning Up Previous Installation Zip File: !InstallationZipFile!
DEL "%InstallationZipFile%" /F
:NOINSTALLFILE
MKDIR "!InstallationFolder!"
COPY "!PLiXPath!\Setup.2017+\bin\!BuildConfiguration!\PLiX for Visual Studio %TargetVisualStudioLongProductYear%.msi" "%InstallationFolder%\PLiX for Visual Studio %TargetVisualStudioLongProductYear%.msi" /v /y
COPY "%RootDir%\Setup\bin\!BuildConfiguration!\\en-US\Natural ORM Architect for Visual Studio %TargetVisualStudioLongProductYear%.msi" "%InstallationFolder%\Natural ORM Architect for Visual Studio %TargetVisualStudioLongProductYear%.msi" /v /y
COPY "%RootDir%\Setup\Setup.%TargetVisualStudioShortProductName%.bat" "%InstallationFolder%\Setup.bat" /v /y
COPY "%RootDir%\Setup\Readme.htm" "%InstallationFolder%\Readme.htm" /v /y
powershell -Command "Invoke-WebRequest https://github.com/Microsoft/vswhere/releases/latest/download/vswhere.exe -OutFile ""%InstallationFolder%\vswhere.exe"""
powershell -Command "Compress-Archive -Path '%InstallationFolder%' -DestinationPath '%InstallationZipFile%'"
REM Clean Up
ECHO Cleaning Up...
RMDIR "%InstallationFolder%" /S /Q
REM Complete
FOR /f "delims=" %%i IN ('dir /B /S ^"%InstallationZipFile%^"') DO (
	SET "NewZipFilePath=%%i"
)
ECHO New Zip File: !NewZipFilePath!
GOTO:EOF

REM Replacing the equals sign
REM code from https://www.dostips.com/forum/viewtopic.php?t=1485
:l_replace
set "str=x%~1x"
:l_replaceloop
for /f "delims=%~2 tokens=1*" %%x in ("!str!") do (
if "%%y"=="" set "str=!str:~1,-1!"&exit/b
set "str=%%x%~3%%y"
)
goto l_replaceloop