//
// Rect.cs
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

using System;

namespace TesseractOCR;

/// <summary>
///     Represents a rectangle defined by an origin, width, and height.
/// </summary>
public readonly struct Rect : IEquatable<Rect>
{
    #region Fields
    /// <summary>
    ///     Returns an empty <see cref="Rect"/>
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static readonly Rect Empty = new();
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the x-coordinate of the rectangle's origin.
    /// </summary>
    public int X1 { get; }

    /// <summary>
    ///     Gets the y-coordinate of the rectangle's origin.
    /// </summary>
    public int Y1 { get; }
    
    /// <summary>
    ///     Gets the x-coordinate of the rectangle's bottom-right corner.
    /// </summary>  
    public int X2 => X1 + Width;

    /// <summary>
    ///     Gets the y-coordinate of the rectangle's bottom-right corner.
    /// </summary>
    public int Y2 => Y1 + Height;
    
    /// <summary>
    ///     Gets the width of the rectangle.
    /// </summary>  
    public int Width { get; }
    
    /// <summary>
    ///     Gets the height of the rectangle.
    /// </summary>  
    public int Height { get; }
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new rectangle with the specified position and dimensions.
    /// </summary>
    /// <param name="x">The x-coordinate of the rectangle.</param>
    /// <param name="y">The y-coordinate of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public Rect(int x, int y, int width, int height)
    {
        X1 = x;
        Y1 = y;
        Width = width;
        Height = height;
    }
    #endregion

    #region FromCoords
    /// <summary>
    ///     Creates a new <see cref="Rect"/> from the specified coordinates.
    /// </summary>
    /// <param name="x1">The x-coordinate of the top-left corner.</param>
    /// <param name="y1">The y-coordinate of the top-left corner.</param>
    /// <param name="x2">The x-coordinate of the bottom-right corner.</param>
    /// <param name="y2">The y-coordinate of the bottom-right corner.</param>
    /// <returns>A new <see cref="Rect"/> instance.</returns>   
    public static Rect FromCoords(int x1, int y1, int x2, int y2)
    {
        return new Rect(x1, y1, x2 - x1, y2 - y1);
    }
    #endregion

    #region Equals
    /// <summary>
    ///     Returns <c>true</c> when this object equals the given <paramref name="obj"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return obj is Rect rect && Equals(rect);
    }

    /// <summary>
    ///     Returns <c>true</c> when this object equals the <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Rect other)
    {
        return X1 == other.X1 && Y1 == other.Y1 && Width == other.Width && Height == other.Height;
    }
    #endregion

    #region GetHashCode
    /// <summary>
    ///     Returns a hash code for this object
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = 0;
        unchecked
        {
            hashCode += 1000000007 * X1.GetHashCode();
            hashCode += 1000000009 * Y1.GetHashCode();
            hashCode += 1000000021 * Width.GetHashCode();
            hashCode += 1000000033 * Height.GetHashCode();
        }

        return hashCode;
    }
    #endregion

    #region Operators
    /// <summary>
    ///     Returns <c>true</c> when equal
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator ==(Rect lhs, Rect rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>
    ///     Returns <c>true</c> when not equal
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator !=(Rect lhs, Rect rhs)
    {
        return !(lhs == rhs);
    }
    #endregion

    #region ToString
    /// <summary>
    ///     Returns a string interpretation of this object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"[Rect X={X1}, Y={Y1}, Width={Width}, Height={Height}]";
    }
    #endregion
}