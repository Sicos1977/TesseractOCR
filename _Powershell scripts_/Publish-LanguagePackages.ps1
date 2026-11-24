<#
.SYNOPSIS
Publishes generated TesseractOCR language packages to nuget.org.
.DESCRIPTION
Uploads TesseractOCR.Languages.*.nupkg files from the package directory. The
script stops on the first failure. Use -SkipDuplicate to continue past package
versions that already exist on nuget.org, for example when retrying a partial upload.
.EXAMPLE
& '.\_Powershell scripts_\Publish-LanguagePackages.ps1' -ApiKey $apiKey
.EXAMPLE
& '.\_Powershell scripts_\Publish-LanguagePackages.ps1' -ApiKey $apiKey -PackageDirectory 'C:\packages' -WhatIf
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
	[Parameter(Mandatory = $true)]
	[ValidateNotNullOrEmpty()]
	[string] $ApiKey,
	[string] $PackageDirectory = (Join-Path $PSScriptRoot 'artifacts\packages'),
	[switch] $SkipDuplicate
)

$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $PackageDirectory -PathType Container)) {
	throw "Package directory not found: $PackageDirectory"
}
$packages = @(Get-ChildItem -LiteralPath $PackageDirectory -Filter 'TesseractOCR.Languages.*.nupkg' -File | Sort-Object Name)
if ($packages.Count -eq 0) {
	throw "No TesseractOCR language packages found in: $PackageDirectory"
}

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
