@echo off
setlocal

set "CONFIG=Release"
set "RUNTIME=win-x64"
set "SELF_CONTAINED=true"
set "PROJECT=SpellWork\SpellWork.csproj"
set "OUTDIR=publish\%RUNTIME%-single"

if not "%~1"=="" set "RUNTIME=%~1"
if not "%~2"=="" set "CONFIG=%~2"
if not "%~3"=="" set "SELF_CONTAINED=%~3"
set "OUTDIR=publish\%RUNTIME%-single"

echo Publishing %PROJECT%
echo   Runtime: %RUNTIME%
echo   Config: %CONFIG%
echo   Self-contained: %SELF_CONTAINED%
echo   Output: %OUTDIR%

dotnet publish "%PROJECT%" ^
  -c %CONFIG% ^
  -r %RUNTIME% ^
  --self-contained %SELF_CONTAINED% ^
  /p:PublishSingleFile=true ^
  /p:IncludeNativeLibrariesForSelfExtract=true ^
  /p:PublishTrimmed=false ^
  -o "%OUTDIR%"

if errorlevel 1 (
  echo.
  echo Publish failed.
  exit /b 1
)

echo.
echo Publish completed successfully.
echo Check "%OUTDIR%".
exit /b 0
