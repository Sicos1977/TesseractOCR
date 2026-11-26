<#
.SYNOPSIS
Builds and publishes the TesseractOCR and optional PDF packages to nuget.org.
.DESCRIPTION
Packs all projects in Release mode before publishing any package. Each run
uses a separate output directory so older packages cannot be published by mistake.
The core package is published first. Use -SkipDuplicate when retrying a partial
upload. -WhatIf builds the packages but does not publish them.
.EXAMPLE
& '.\_Powershell scripts_\Publish-Packages.ps1'
.EXAMPLE
& '.\_Powershell scripts_\Publish-Packages.ps1' -ApiKey $apiKey -SkipDuplicate
.EXAMPLE
& '.\_Powershell scripts_\Publish-Packages.ps1' -ApiKey $apiKey -WhatIf
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
	[Parameter(Mandatory = $true)]
	[ValidateNotNullOrEmpty()]
	[string] $ApiKey,
	[switch] $SkipDuplicate
)

$ErrorActionPreference = 'Stop'
$packageDirectory = Join-Path $PSScriptRoot (Join-Path 'artifacts\release-packages' ([guid]::NewGuid().ToString()))
New-Item -ItemType Directory -Path $packageDirectory -Force -WhatIf:$false -Confirm:$false | Out-Null
$packageDirectory = (Resolve-Path -LiteralPath $packageDirectory).Path
$projects = @(
	@{ Name = 'TesseractOCR'; Path = (Join-Path $PSScriptRoot '..\TesseractOCR\TesseractOCR.csproj') },
	@{ Name = 'TesseractOCRPdfSkiaSharp'; Path = (Join-Path $PSScriptRoot '..\TesseractOCRPdfSkiaSharp\TesseractOCRPdfSkiaSharp.csproj') },
	@{ Name = 'TesseractOCRPdfImageMagick'; Path = (Join-Path $PSScriptRoot '..\TesseractOCRPdfImageMagick\TesseractOCRPdfImageMagick.csproj') }
)
$packages = @()
foreach ($project in $projects) {
	$outputDirectory = Join-Path $packageDirectory $project.Name
	New-Item -ItemType Directory -Path $outputDirectory -Force -WhatIf:$false -Confirm:$false | Out-Null
	Write-Host "Building and packing $($project.Name) in Release mode"
	& dotnet pack $project.Path --configuration Release --output $outputDirectory -p:GeneratePackageOnBuild=false
	if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed for $($project.Name) with exit code $LASTEXITCODE." }
	$matches = @(Get-ChildItem -LiteralPath $outputDirectory -Filter "$($project.Name).*.nupkg" -File | Where-Object {
		$_.Name -match ('^' + [regex]::Escape($project.Name) + '\.\d') -and $_.Name -notlike '*.symbols.nupkg'
	})
	if ($matches.Count -ne 1) { throw "Expected one $($project.Name) package in $outputDirectory; found $($matches.Count)." }
	$packages += $matches[0]
}

Write-Host "Packages: $packageDirectory"
$source = 'https://api.nuget.org/v3/index.json'
foreach ($package in $packages) {
	if ($PSCmdlet.ShouldProcess($package.FullName, 'Publish to nuget.org')) {
		Write-Host "Publishing $($package.Name)"
		$pushArguments = @('nuget', 'push', $package.FullName, '--source', $source, '--api-key', $ApiKey, '--no-symbols')
		if ($SkipDuplicate) { $pushArguments += '--skip-duplicate' }
		& dotnet @pushArguments
		if ($LASTEXITCODE -ne 0) { throw "dotnet nuget push failed for $($package.Name) with exit code $LASTEXITCODE." }
	}
}
