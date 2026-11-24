@echo off
setlocal
pushd "%~dp0"
docfx serve _site
set "exitCode=%errorlevel%"
popd
endlocal & exit /b %exitCode%