@echo off
setlocal
pushd "%~dp0"
docfx metadata docfx.json
set "exitCode=%errorlevel%"
popd
endlocal & exit /b %exitCode%