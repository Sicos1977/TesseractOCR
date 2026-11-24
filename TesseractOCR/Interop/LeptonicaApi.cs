//
// LeptonicaApi.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2027 Kees van Spelde
//
// Licensed under the Apache License, Version 2.0 (the "License");
//
// - You may not use this file except in compliance with the License.
// - You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using TesseractOCR.InteropDotNet;
#pragma warning disable CS1591

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Interop;

internal static class LeptonicaApi
{
    #region Fields
    private static ILeptonicaApiSignatures native;
    #endregion

    #region Properties
    public static ILeptonicaApiSignatures Native
    {
        get
        {
            if (native == null)
                Initialize();

            return native;
        }
    }
    #endregion

    #region Initialize
    public static void Initialize()
    {
        if (native != null) return;
        Helper.SetPath();
        native = InteropRuntimeImplementer.CreateInstance<ILeptonicaApiSignatures>();
    }
    #endregion
}