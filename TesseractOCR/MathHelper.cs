//
// MathHelper.cs
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

using System;
// ReSharper disable UnusedMember.Global

namespace TesseractOCR
{
    /// <summary>
    ///     Math helpers
    /// </summary>
    public static class MathHelper
    {
        #region ToRadians
        /// <summary>
        ///     Convert a degrees to radians.
        /// </summary>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static float ToRadians(float angleInDegrees)
        {
            return (float)ToRadians((double)angleInDegrees);
        }

        /// <summary>
        ///     Convert a degrees to radians.
        /// </summary>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static double ToRadians(double angleInDegrees)
        {
            return angleInDegrees * Math.PI / 180.0;
        }
        #endregion

        #region DivRoundUp
        /// <summary>
        ///     Calculates the smallest integer greater than the quot-ant of dividend and divisor.
        /// </summary>
        /// <see href="http://stackoverflow.com/questions/921180/how-can-i-ensure-that-a-division-of-integers-is-always-rounded-up" />
        public static int DivRoundUp(int dividend, int divisor)
        {
            var result = dividend / divisor;

            return dividend % divisor != 0 && divisor > 0 == dividend > 0 ? result + 1 : result;
        }
        #endregion
    }
}