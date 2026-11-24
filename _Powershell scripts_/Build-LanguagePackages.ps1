<#
.SYNOPSIS
Downloads tessdata_fast models and creates a NuGet package for each model.
.DESCRIPTION
Packages all language and script models from one upstream commit. Use -Models to
select specific models by their path without the .traineddata extension.
Package names use the TesseractOCR Language enum name whose String attribute matches
the model (for example TesseractOCR.Languages.English for eng). Models without an
enum value keep their model name (for example TesseractOCR.Languages.script.Latin).
Downloaded models are removed after their packages are successfully created.
The package version is independent of the upstream repository revision.
-TesseractOCRVersion specifies the minimum version of the TesseractOCR dependency.
.EXAMPLE
& '.\_Powershell scripts_\Build-LanguagePackages.ps1' -Version 1.0.0
.EXAMPLE
& '.\_Powershell scripts_\Build-LanguagePackages.ps1' -Models eng,script/Latin -Version 1.0.0
#>
[CmdletBinding()]
param(
	[string] $Version = '1.0.0',
	[string] $Revision = 'main',
	[string[]] $Models = @(),
	[string] $OutputDirectory = (Join-Path $PSScriptRoot 'artifacts'),
	[string] $TesseractOCRVersion = '5.5.2'
)

$ErrorActionPreference = 'Stop'
$languageSource = Join-Path $PSScriptRoot '..\TesseractOCR\Enums\Language.cs'
$languageNames = @{}
foreach ($match in [regex]::Matches([IO.File]::ReadAllText($languageSource), '\[String\("([^"]+)"\)\]\s*(\w+)')) {
	$languageNames[$match.Groups[1].Value] = $match.Groups[2].Value
}
if ($languageNames.Count -eq 0) { throw "No language mappings were found in $languageSource." }
$headers = @{ 'User-Agent' = 'TesseractOCR-Language-Packager' }
$api = 'https://api.github.com/repos/tesseract-ocr/tessdata_fast'
$revisionName = [Uri]::EscapeDataString($Revision)
$commit = (Invoke-RestMethod -Uri "$api/commits/$revisionName" -Headers $headers).sha
if ($commit -notmatch '^[0-9a-f]{40}$') {
	throw "Unable to resolve upstream revision '$Revision' to a commit."
}
$tree = Invoke-RestMethod -Uri "$api/git/trees/${commit}?recursive=1" -Headers $headers
if ($tree.truncated) { throw 'The upstream file list is truncated.' }
$available = @($tree.tree | Where-Object {
	$_.type -eq 'blob' -and $_.path -cmatch '^(?:script/)?[A-Za-z0-9_]+\.traineddata$'
} | Sort-Object path)
if ($available.Count -eq 0) { throw 'No traineddata models were found in the upstream revision.' }

if ($Models.Count -gt 0) {
	foreach ($model in $Models) {
		if (-not ($available | Where-Object { $_.path -ceq "$model.traineddata" })) {
			throw "Model '$model' was not found in upstream revision $commit."
		}
	}
	$available = @($available | Where-Object { $Models -ccontains $_.path.Substring(0, $_.path.Length - '.traineddata'.Length) })
}

$dataDirectory = Join-Path $OutputDirectory 'tessdata'
$packageDirectory = Join-Path $OutputDirectory 'packages'
New-Item -ItemType Directory -Force -Path $dataDirectory, $packageDirectory | Out-Null
$dataDirectory = (Resolve-Path $dataDirectory).Path
$packageDirectory = (Resolve-Path $packageDirectory).Path
$project = Join-Path $PSScriptRoot 'LanguagePackage.csproj'

Write-Host "Using tessdata_fast commit $commit; building $($available.Count) package(s)."
& dotnet restore $project "-p:LanguagePackageVersion=$Version" "-p:TesseractOCRVersion=$TesseractOCRVersion" --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed with exit code $LASTEXITCODE." }

foreach ($entry in $available) {
	$relativePath = $entry.path.Replace('/', [IO.Path]::DirectorySeparatorChar)
	$modelFile = Join-Path $dataDirectory $relativePath
	$modelDirectory = Split-Path $modelFile -Parent
	New-Item -ItemType Directory -Force -Path $modelDirectory | Out-Null
	$downloadPath = "$modelFile.download"
	$url = "https://raw.githubusercontent.com/tesseract-ocr/tessdata_fast/$commit/$($entry.path)"
	Write-Host "Downloading $($entry.path)"
	try {
		Invoke-WebRequest -Uri $url -Headers $headers -OutFile $downloadPath -UseBasicParsing
		if ((Get-Item $downloadPath).Length -ne $entry.size) {
			throw "Downloaded size does not match the upstream size for $($entry.path)."
		}
		$hash = [System.Security.Cryptography.SHA1]::Create()
		$hashStream = [System.Security.Cryptography.CryptoStream]::new([IO.Stream]::Null, $hash, [System.Security.Cryptography.CryptoStreamMode]::Write)
		try {
			$prefix = [Text.Encoding]::ASCII.GetBytes("blob $($entry.size)`0")
			$hashStream.Write($prefix, 0, $prefix.Length)
			$inputStream = [IO.File]::OpenRead($downloadPath)
			try { $inputStream.CopyTo($hashStream) } finally { $inputStream.Dispose() }
			$hashStream.FlushFinalBlock()
			$actualHash = [BitConverter]::ToString($hash.Hash).Replace('-', '').ToLowerInvariant()
		} finally {
			$hashStream.Dispose()
			$hash.Dispose()
		}
		if ($actualHash -ne $entry.sha) { throw "Downloaded hash does not match the upstream hash for $($entry.path)." }
		Move-Item -LiteralPath $downloadPath -Destination $modelFile -Force
	} finally {
		if (Test-Path -LiteralPath $downloadPath) { Remove-Item -LiteralPath $downloadPath }
	}

	$modelName = $entry.path.Substring(0, $entry.path.Length - '.traineddata'.Length)
	$modelId = $modelName.Replace('/', '.')
	$packageName = if ($languageNames.ContainsKey($modelName)) { $languageNames[$modelName] } else { $modelId }
	$packageSubdirectory = if ($modelName.StartsWith('script/')) { 'script' } else { '' }
	Write-Host "Packing TesseractOCR.Languages.$packageName $Version ($modelName)"
	& dotnet pack $project --no-build --no-restore --output $packageDirectory --verbosity quiet `
		"-p:LanguagePackageVersion=$Version" "-p:TesseractOCRVersion=$TesseractOCRVersion" "-p:ModelId=$modelId" `
		"-p:PackageName=$packageName" "-p:ModelFile=$modelFile" "-p:ModelDirectory=$packageSubdirectory" "-p:ModelCommit=$commit"
	if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed for $modelName with exit code $LASTEXITCODE." }
	Remove-Item -LiteralPath $modelFile
}
Write-Host "Packages: $packageDirectory"
