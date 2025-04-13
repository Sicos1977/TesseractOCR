﻿//
// Rect.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2025 Kees van Spelde
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

namespace TesseractOCR
{
    public readonly struct Rect : IEquatable<Rect>
    {
        #region Fields
        /// <summary>
        ///     Returns an empty <see cref="Rect"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static readonly Rect Empty = new Rect();
        #endregion

        #region Properties
        public int X1 { get; }

        public int Y1 { get; }

        public int X2 => X1 + Width;

        public int Y2 => Y1 + Height;

        public int Width { get; }

        public int Height { get; }
        #endregion

        #region Constructors
        public Rect(int x, int y, int width, int height)
        {
            X1 = x;
            Y1 = y;
            Width = width;
            Height = height;
        }
        #endregion

        #region FromCoords
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
}