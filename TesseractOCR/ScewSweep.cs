//
// ScewSweep.cs
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

namespace TesseractOCR;

/// <summary>
///     Represents the parameters for a sweep search used by scew algorithms
/// </summary>
public struct ScewSweep
{
    #region Constants
    /// <summary>The default image reduction factor for the sweep.</summary>
    public const int DefaultReduction = 4; // Sweep part; 4 is good
    /// <summary>The default sweep range in degrees.</summary>
    public const float DefaultRange = 7.0F;
    /// <summary>The default sweep step in degrees.</summary>
    public const float DefaultDelta = 1.0F;
    #endregion

    #region Fields
    /// <summary>The default sweep parameters.</summary>
    public static ScewSweep Default = new(DefaultReduction);
    #endregion

    #region Properties
    /// <summary>Gets the image reduction factor for the sweep.</summary>
    public int Reduction { get; }

    /// <summary>Gets the angular sweep range in degrees.</summary>
    public float Range { get; }

    /// <summary>Gets the angular sweep step in degrees.</summary>
    public float Delta { get; }
    #endregion

    #region Constructor
    /// <summary>Creates parameters for a skew sweep search.</summary>
    /// <param name="reduction">The image reduction factor for the sweep.</param>
    /// <param name="range">The angular search range in degrees.</param>
    /// <param name="delta">The angular search step in degrees.</param>
    public ScewSweep(int reduction = DefaultReduction, float range = DefaultRange, float delta = DefaultDelta)
    {
        Reduction = reduction;
        Range = range;
        Delta = delta;
    }
    #endregion
}