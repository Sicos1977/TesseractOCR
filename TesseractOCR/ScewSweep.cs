//
// ScewSweep.cs
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
    /// <summary>
    ///     Represents the parameters for a sweep search used by scew algorithms
    /// </summary>
    public struct ScewSweep
    {
        #region Constants
        public const int DefaultReduction = 4; // Sweep part; 4 is good
        public const float DefaultRange = 7.0F;
        public const float DefaultDelta = 1.0F;
        #endregion

        #region Fields
        public static ScewSweep Default = new ScewSweep(DefaultReduction);
        #endregion

        #region Properties
        public int Reduction { get; }

        public float Range { get; }

        public float Delta { get; }
        #endregion

        #region Constructor
        public ScewSweep(int reduction = DefaultReduction, float range = DefaultRange, float delta = DefaultDelta)
        {
            Reduction = reduction;
            Range = range;
            Delta = delta;
        }
        #endregion
    }
}