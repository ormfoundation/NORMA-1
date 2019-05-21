@ECHO OFF

REM We need to set theres here because we can't call VSVer.bat until we figure out the ToolsVersion for MSBuild
SET TargetVisualStudioMajorMinorVersion=16.0
SET TargetVisualStudioMajorMinorNextVersion=17.0

REM Determine the install loation for Visual Studio
FOR /f "usebackq tokens=*" %%i IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -version [%TargetVisualStudioMajorMinorVersion%^,%TargetVisualStudioMajorMinorNextVersion%^) -products * -requires Microsoft.Component.MSBuild -property installationPath`) DO (
	SET VSInstallDir=%%i
)

REM Determine the correct ToolsVersion for MSBuild
FOR /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -version [%TargetVisualStudioMajorMinorVersion%^,%TargetVisualStudioMajorMinorNextVersion%^) -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
	SET CurrentMSBuildPath=%%i
)

REM Remove the trailing "\Bin\MSBuild.exe"
SET CurrentToolsVersionPath=%CurrentMSBuildPath:~0,-16%
REM REmove the path up to the current version
CALL SET CurrentMSBuildToolsVersion=%%CurrentToolsVersionPath:%VSInstallDir%\MSBuild\=%%

REM Update the path for dlls needed by the build, we don't want to add them to the GAC
CALL:EXTENDPATH "%VSInstallDir%\Common7\IDE\PublicAssemblies"
CALL:EXTENDPATH "%VSInstallDir%\Common7\IDE\PrivateAssemblies"
CALL:EXTENDPATH "%VSInstallDir%\VSSDK\VisualStudioIntegration\Common\Assemblies\v4.0"
CALL:EXTENDPATH "%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin"
CALL:EXTENDPATH "%VSInstallDir%\MSBuild\%CurrentMSBuildToolsVersion%\Bin"
SET VsctPath=%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin\vsct.exe

REM Call VSVer.bat
SET ProjectToolsVersion=%CurrentMSBuildToolsVersion%
@CALL "%~dp0VSVer.bat" 2019

REM Set the loacl variables so that changes to the environment will be reflected here
SET TargetVisualStudioVersion=v16.0
SET TargetFrameworkVersion=v4.7.2
SET TargetVisualStudioAssemblyVersion=16.0.0.0
SET TargetVisualStudioAssemblySuffix=.15.0
SET TargetVisualStudioFrameworkAssemblyVersion=4.7.2.0
SET TargetVisualStudioFrameworkAssemblySuffix=.15.0
SET TargetVisualStudioLongProductYear=2019
SET TargetVisualStudioShortProductYear=19
SET "TargetVisualStudioLongProductName=Visual Studio 2019"
SET TargetVisualStudioShortProductName=VS2019
SET TargetDslToolsAssemblyVersion=16.0.0.0
SET TargetDslToolsVersionSuffix=.15.0
SET ProjectToolsAssemblySuffix=.Core
SET ProjectToolsAssemblyVersion=15.1.0.0
SET GeneratedDslFileSuffix=.VS2010
SET UseGAC=FALSE
SET VsctPath=%VSInstallDir%\VSSDK\VisualStudioIntegration\Tools\Bin\vsct.exe

REM Set Machine Level Environment Variables so that individual projects can be opended via Visual Studio
SETX TargetVisualStudioVersion %TargetVisualStudioVersion% /M > NUL
SETX TargetFrameworkVersion %TargetFrameworkVersion% /M > NUL
SETX TargetVisualStudioAssemblyVersion %TargetVisualStudioAssemblyVersion% /M > NUL
SETX TargetVisualStudioAssemblySuffix %TargetVisualStudioAssemblySuffix% /M > NUL
SETX TargetVisualStudioFrameworkAssemblyVersion %TargetVisualStudioFrameworkAssemblyVersion% /M > NUL
SETX TargetVisualStudioFrameworkAssemblySuffix %TargetVisualStudioFrameworkAssemblySuffix% /M > NUL
SETX TargetVisualStudioLongProductYear %TargetVisualStudioLongProductYear% /M > NUL
SETX TargetVisualStudioShortProductYear %TargetVisualStudioShortProductYear% /M > NUL
SETX TargetVisualStudioLongProductName "%TargetVisualStudioLongProductName%" /M > NUL
SETX TargetVisualStudioShortProductName %TargetVisualStudioShortProductName% /M > NUL
SETX TargetVisualStudioMajorMinorVersion %TargetVisualStudioMajorMinorVersion% /M > NUL
SETX TargetVisualStudioMajorMinorNextVersion %TargetVisualStudioMajorMinorNextVersion% /M > NUL
SETX TargetDslToolsAssemblyVersion %TargetDslToolsAssemblyVersion% /M > NUL
SETX TargetDslToolsVersionSuffix %TargetDslToolsVersionSuffix% /M > NUL
SETX ProjectToolsVersion %ProjectToolsVersion% /M > NUL
SETX ProjectToolsAssemblySuffix %ProjectToolsAssemblySuffix% /M > NUL
SETX ProjectToolsAssemblyVersion %ProjectToolsAssemblyVersion% /M > NUL
SETX GeneratedDslFileSuffix %GeneratedDslFileSuffix% /M > NUL
SETX UseGAC %UseGAC% /M > NUL
SETX VsctPath "%VsctPath%" /M > NUL

goto:EOF

:EXTENDPATH
SET PATH=%~1;%PATH%
GOTO:EOF