//
// Scew.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2022 Kees van Spelde
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

namespace TesseractOCR
{
    public readonly struct Scew
    {
        #region Properties
        public float Angle { get; }

        public float Confidence { get; }

        public Scew(float angle, float confidence)
        {
            Angle = angle;
            Confidence = confidence;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return $"Scew: {Angle} [conf: {Confidence}]";
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            return obj is Scew scew && Equals(scew);
        }

        public bool Equals(Scew other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return Confidence == other.Confidence && Angle == other.Angle;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }
        #endregion

        #region GetHashCode
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
        public static bool operator ==(Scew lhs, Scew rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Scew lhs, Scew rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}