@echo off
setlocal
pushd "%~dp0"

REM Generate API metadata for the TesseractOCR library and build the site.
docfx metadata docfx.json
if errorlevel 1 (
    echo DocFX metadata generation failed
    popd
    exit /b 1
)

docfx build docfx.json
if errorlevel 1 (
    echo DocFX build failed
    popd
    exit /b 1
)

echo DocFX build completed successfully
popd
endlocal