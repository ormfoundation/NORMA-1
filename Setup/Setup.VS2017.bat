@echo off

:: Code From: https://stackoverflow.com/questions/7044985/how-can-i-auto-elevate-my-batch-file-so-that-it-requests-from-uac-administrator?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
NET SESSION 1>NUL 2>NUL
IF %ERRORLEVEL% NEQ 0 GOTO ELEVATE
GOTO ADMINTASKS

:ELEVATE
CD /d %~dp0
MSHTA "javascript: var shell = new ActiveXObject('shell.application'); shell.ShellExecute('%~nx0', '', '', 'runas', 1);close();"
EXIT

:ADMINTASKS

setlocal

echo Natural Object-Role Modeling Architect for Visual Studio 2017 Installation

:: Check to see if vswhere.exe exists
IF "%ProgramFiles(X86)%"=="" (
	SET ResolvedProgramFiles=%ProgramFiles%
) ELSE (
	CALL:_ResolveProgramFiles
)
SET "VSWHEREPATH=%ResolvedProgramFiles%\Microsoft Visual Studio\Installer\"
IF NOT EXIST "%VSWHEREPATH%\vswhere.exe" (
	CALL:_InstallVSWhere
)

::See if a current NORMA version is already installed
REG QUERY "HKCR\Installer\Products\B4238B98000088040123100000000000" 1>NUL 2>&1
IF ERRORLEVEL 1 goto :INSTALLNORMA

::Uninstall NORMA, including verification that the registry key is gone after MSIExec is completed
echo Removing previous NORMA version
start /w msiexec.exe /x {89B8324B-0000-4088-1032-010000000000} /q
REG QUERY "HKCR\Installer\Products\B4238B98000088040123100000000000" 1>NUL 2>&1
IF NOT ERRORLEVEL 1 (
echo Existing NORMA version failed to uninstall
pause
goto :EOF
)

:INSTALLNORMA

::See if a current PLiX version is already installed
REG QUERY "HKCR\Installer\Products\1D5CAB24000068240123100000000000" 1>NUL 2>&1
IF ERRORLEVEL 1 goto :INSTALLPLIX

::Uninstall plix, including verification that the registry key is gone after MSIExec is completed
echo Removing previous PLiX version
start /w msiexec.exe /x {42BAC5D1-0000-4286-1032-010000000000} /q
REG QUERY "HKCR\Installer\Products\1D5CAB24000068240123100000000000" 1>NUL 2>&1
IF NOT ERRORLEVEL 1 (
echo Existing PLiX version failed to uninstall
pause
goto :EOF
)

:INSTALLPLIX
echo Installing current PLiX version
start /w msiexec.exe /i "%~dp0PLiX for Visual Studio 2017.msi"
REG QUERY "HKCR\Installer\Products\1D5CAB24000068240123100000000000" 1>NUL 2>&1
IF ERRORLEVEL 1 (
echo New PLiX version failed to install
pause
goto :EOF
)

:PLIXINSTALLED

echo Installing current NORMA version
start msiexec.exe /i "%~dp0Natural ORM Architect for Visual Studio 2017.msi"
goto :EOF

:_ResolveProgramFiles
SET ResolvedProgramFiles=%ProgramFiles(x86)%
goto :EOF

:_InstallVSWhere
echo Installing VSWhere
SET LOCALVSWHERE=%~dp0vswhere.exe
XCOPY /Y /T /Q "%LOCALVSWHERE%" "%VSWHEREPATH%"
XCOPY /Y /D /V /Q "%LOCALVSWHERE%" "%VSWHEREPATH%"
goto :EOF