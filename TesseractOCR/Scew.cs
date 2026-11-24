//
// Scew.cs
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
///     Represents a detected image skew angle and its confidence.
/// </summary>
public readonly struct Scew
{
    #region Properties
    /// <summary>Gets the detected skew angle in degrees.</summary>
    public float Angle { get; }

    /// <summary>Gets the confidence of the skew estimate.</summary>
    public float Confidence { get; }

    /// <summary>Creates a skew estimate.</summary>
    /// <param name="angle">The detected angle in degrees.</param>
    /// <param name="confidence">The confidence of the estimate.</param>
    public Scew(float angle, float confidence)
    {
        Angle = angle;
        Confidence = confidence;
    }
    #endregion

    #region ToString
    /// <inheritdoc />
    public override string ToString()
    {
        return $"Scew: {Angle} [conf: {Confidence}]";
    }
    #endregion

    #region Equals
    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Scew scew && Equals(scew);
    }

    /// <summary>Compares the angle and confidence with another estimate.</summary>
    /// <param name="other">The estimate to compare.</param>
    /// <returns><c>true</c> if both values match; otherwise, <c>false</c>.</returns>
    public bool Equals(Scew other)
    {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return Confidence == other.Confidence && Angle == other.Angle;
        // ReSharper restore CompareOfFloatsByEqualityOperator
    }
    #endregion

    #region GetHashCode
    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        unchecked
        {
            hashCode += 1000000007 * Angle.GetHashCode();
            hashCode += 1000000009 * Confidence.GetHashCode();
        }

        return hashCode;
    }
    #endregion

    #region Operators
    /// <summary>Determines whether two skew estimates match.</summary>
    /// <param name="lhs">The first estimate.</param>
    /// <param name="rhs">The second estimate.</param>
    /// <returns><c>true</c> if both angle and confidence match; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Scew lhs, Scew rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>Determines whether two skew estimates differ.</summary>
    /// <param name="lhs">The first estimate.</param>
    /// <param name="rhs">The second estimate.</param>
    /// <returns><c>true</c> if the estimates differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Scew lhs, Scew rhs)
    {
        return !(lhs == rhs);
    }
    #endregion
}