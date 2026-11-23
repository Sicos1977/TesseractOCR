# Native dependency provenance and licensing

This note records what is known in this repository about the Windows x64
`leptonica-1.85.0.dll` distributed with `TesseractOCR` 5.5.2. It does not
establish provenance or licensing facts that are not backed by retained build
records.

## Identified artifact

| Item | Value |
| --- | --- |
| NuGet package | `TesseractOCR` 5.5.2 |
| Package SHA-256 reported with issue #87 | `2D11E0A45F8F5887D91277DD61E4B2B57423BE0E06AEFBA432484EBC33A8A794` |
| DLL in package | `x64/leptonica-1.85.0.dll` |
| DLL SHA-256 | `64BB1D584BCE83DB5E5C3625F1857E4BC12993625699CABD0A9B8B76E393BD01` |

The DLL digest matches the x64 Leptonica DLL currently tracked in this
repository. The NuGet archive itself is not retained here, so its reported
digest cannot be independently checked from this checkout.

The report accompanying issue #87 says that `getLeptonicaVersion()` returned
Leptonica 1.85.0, `MSC v.1943`, `LIB Release x64`. It also reports these
`getImagelibVersions()` results:

```text
libgif 5.2.1
libjpeg 6b (libjpeg-turbo 2.0.6)
libpng 1.6.37
libtiff 4.3.0
zlib 1.2.11
```

These are reported runtime/version results, not a complete native dependency
inventory or proof of how any library was linked.

## Build provenance and dependencies

This repository does not retain the source revision, local patches, build
manifest, compiler/toolset details beyond the reported version string, build
flags, enabled-codec configuration, vcpkg baseline/manifest/triplet, or a
reproducible build procedure for this x64 DLL. The mention of vcpkg in the CI
workflow describes the ARM64 binaries and Leptonica 1.87.0; it does not identify
the inputs used for this x64 Leptonica 1.85.0 DLL.

Issue #87 reports LZMA-related strings and no liblzma DLL import. Those
observations may suggest static linkage, but do not prove the linkage mechanism,
identify a liblzma/XZ Utils version, or establish which license and
attributions apply. The versions reported above likewise do not establish
whether those libraries are statically or dynamically linked, whether
additional or transitive libraries are present, or their exact versions and
licenses. No complete native dependency inventory or corresponding license
and notice bundle for this binary is retained in this repository.

## Licensing and redistribution

The project declares `Apache-2.0` as its NuGet package license, but that
declaration alone is not evidence that the bundled native DLL or all code in it
is licensed under Apache-2.0. Because the native source provenance and matching
license/notice records are unavailable here, this repository cannot confirm
the exact third-party licenses, required attributions, or redistribution
conditions for this particular DLL. Do not rely on the package license
metadata as confirmation of the native binary's redistribution terms. Obtain
and review the matching build and licensing records before redistributing this
native binary.
