//
// Script.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2021-2023 Kees van Spelde
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

// ReSharper disable UnusedMember.Global

using System;

namespace TesseractOCR.Enums
{
    #region Enum ScriptName
    /// <summary>
    ///     All the available Tesseract scripts
    /// </summary>
    public enum ScriptName
    {
        /// <summary>
        ///     Arabic
        /// </summary>
        [String("Unknown")]
        Unknown,

        /// <summary>
        ///     Arabic
        /// </summary>
        [String("Arabic")] 
        Arabic,

        /// <summary>
        ///     Armenian
        /// </summary>
        [String("Armenian")] 
        Armenian,

        /// <summary>
        ///     Bengali
        /// </summary>
        [String("Bengali")] 
        Bengali,

        /// <summary>
        ///     Canadian_Aboriginal
        /// </summary>
        [String("Canadian_Aboriginal")] 
        CanadianAboriginal,

        /// <summary>
        ///     Cherokee
        /// </summary>
        [String("Cherokee")] 
        Cherokee,

        /// <summary>
        ///     Cyrillic
        /// </summary>
        [String("Cyrillic")] 
        Cyrillic,

        /// <summary>
        ///     Devanagari
        /// </summary>
        [String("Devanagari")]
        Devanagari,

        /// <summary>
        ///     Ethiopic
        /// </summary>
        [String("Ethiopic")] 
        Ethiopic,

        /// <summary>
        ///     Fraktur
        /// </summary>
        [String("Fraktur")] 
        Fraktur,

        /// <summary>
        ///     Georgian
        /// </summary>
        [String("Georgian")] 
        Georgian,

        /// <summary>
        ///     Greek
        /// </summary>
        [String("Greek")] Greek,

        /// <summary>
        ///     Gujarati
        /// </summary>
        [String("Gujarati")] 
        Gujarati,

        /// <summary>
        ///     Gurmukhi
        /// </summary>
        [String("Gurmukhi")] 
        Gurmukhi,

        /// <summary>
        ///     HanS (Han simplified)
        /// </summary>
        [String("HanS")]
        HanS,

        /// <summary>
        ///     HanS_vert (Han simplified vertical)
        /// </summary>
        [String("HanS_vert")]
        HanSVert,

        /// <summary>
        ///     HanT (Han traditional)
        /// </summary>
        [String("HanT")] 
        HanT,

        /// <summary>
        ///     HanT_vert (Han traditional vertical)
        /// </summary>
        [String("HanT_vert")]
        HanTVert,

        /// <summary>
        ///     Hangul
        /// </summary>
        [String("Hangul")] 
        Hangul,

        /// <summary>
        ///     Hangul_vert (Hangul vertical)
        /// </summary>
        [String("Hangul_vert(Hangul vertical)")]
        HangulVert,

        /// <summary>
        ///     Hebrew
        /// </summary>
        [String("Hebrew")] 
        Hebrew,

        /// <summary>
        ///     Japanese
        /// </summary>
        [String("Japanese")] 
        Japanese,

        /// <summary>
        ///     Japanese_vert (Japanese vertical)
        /// </summary>
        [String("Japanese_vert")]
        JapaneseVert,

        /// <summary>
        ///     Kannada
        /// </summary>
        [String("Kannada")] 
        Kannada,

        /// <summary>
        ///     Khmer
        /// </summary>
        [String("Khmer")] 
        Khmer,

        /// <summary>
        ///     Lao
        /// </summary>
        [String("Lao")] 
        Lao,

        /// <summary>
        ///     Latin
        /// </summary>
        [String("Latin")] 
        Latin,

        /// <summary>
        ///     Malayalam
        /// </summary>
        [String("Malayalam")] 
        Malayalam,

        /// <summary>
        ///     Myanmar
        /// </summary>
        [String("Myanmar")] 
        Myanmar,

        /// <summary>
        ///     Oriya (Odia)
        /// </summary>
        [String("Oriya")] 
        Oriya,

        /// <summary>
        ///     Sinhala
        /// </summary>
        [String("Sinhala")] 
        Sinhala,

        /// <summary>
        ///     Syriac
        /// </summary>
        [String("Syriac")] 
        Syriac,

        /// <summary>
        ///     Tamil
        /// </summary>
        [String("Tamil")] 
        Tamil,

        /// <summary>
        ///     Telugu
        /// </summary>
        [String("Telugu")] 
        Telugu,

        /// <summary>
        ///     Thaana
        /// </summary>
        [String("Thaana")] 
        Thaana,

        /// <summary>
        ///     Thai
        /// </summary>
        [String("Thai")] 
        Thai,

        /// <summary>
        ///     Tibetan
        /// </summary>
        [String("Tibetan")] 
        Tibetan,

        /// <summary>
        ///     Vietnamese
        /// </summary>
        [String("Vietnamese")] 
        Vietnamese
    }
    #endregion

    /// <summary>
    ///     A helper class to work with the <see cref="ScriptName"/>
    /// </summary>
    public static class ScriptNameHelper
    {
        #region StringToEnum
        /// <summary>
        ///     Returns the <see cref="ScriptName"/> enum that has the <see cref="StringAttribute"/>
        ///     with the corresponding <paramref name="scriptNameString"/>
        /// </summary>
        /// <param name="scriptNameString">The language string</param>
        /// <returns><see cref="Language"/></returns>

        public static ScriptName StringToEnum(string scriptNameString)
        {
            foreach (var scriptName in (ScriptName[])Enum.GetValues(typeof(ScriptName)))
            {
                var value = scriptName.GetAttributeOfType<StringAttribute>().Value;

                if (value == scriptNameString)
                    return scriptName;
            }

            return ScriptName.Unknown;
        }
        #endregion
    }
}