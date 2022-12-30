//
// Language.cs
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
    #region Enum Language
    /// <summary>
    ///     All the available Tesseract languages
    /// </summary>
    public enum Language
    {
        /// <summary>
        ///     The language is unknown
        /// </summary>
        [String("unknown")]
        Unknown,

        /// <summary>
        ///     Afrikaans
        /// </summary>
        [String("afr")] 
        Afrikaans,

        /// <summary>
        ///     Amharic
        /// </summary>
        [String("amh")] 
        Amharic,

        /// <summary>
        ///     Arabic
        /// </summary>
        [String("ara")] 
        Arabic,

        /// <summary>
        ///     Assamese
        /// </summary>
        [String("asm")] 
        Assamese,

        /// <summary>
        ///     Azerbaijani
        /// </summary>
        [String("aze")] 
        Azerbaijani,

        /// <summary>
        ///     Azerbaijani - Cyrilic
        /// </summary>
        [String("aze_cyrl")] 
        AzerbaijaniCyrilic,

        /// <summary>
        ///     Belarusian
        /// </summary>
        [String("bel")] 
        Belarusian,

        /// <summary>
        ///     Bengali
        /// </summary>
        [String("ben")] 
        Bengali,

        /// <summary>
        ///     Tibetan
        /// </summary>
        [String("bod")] 
        Tibetan,

        /// <summary>
        ///     Bosnian
        /// </summary>
        [String("bos")] 
        Bosnian,

        /// <summary>
        ///     Breton
        /// </summary>
        [String("bre")] 
        Breton,

        /// <summary>
        ///     Bulgarian
        /// </summary>
        [String("bul")] 
        Bulgarian,

        /// <summary>
        ///     Catalan; Valencian
        /// </summary>
        [String("cat")] 
        CatalanValencian,

        /// <summary>
        ///     Cebuano
        /// </summary>
        [String("ceb")] 
        Cebuano,

        /// <summary>
        ///     Czech
        /// </summary>
        [String("ces")] 
        Czech,

        /// <summary>
        ///     Chinese - Simplified
        /// </summary>
        [String("chi_sim")] 
        ChineseSimplified,

        /// <summary>
        ///     Chinese - Traditional
        /// </summary>
        [String("chi_tra")] 
        ChineseTraditional,

        /// <summary>
        ///     Cherokee
        /// </summary>
        [String("chr")] 
        Cherokee,

        /// <summary>
        ///     Corsican
        /// </summary>
        [String("cos")] 
        Corsican,

        /// <summary>
        ///     Welsh
        /// </summary>
        [String("cym")] 
        Welsh,

        /// <summary>
        ///     Danish
        /// </summary>
        [String("dan")] 
        Danish,

        /// <summary>
        ///     Danish - Fraktur (contrib)
        /// </summary>
        [String("dan_frak")] 
        DanishFraktur,

        /// <summary>
        ///     German
        /// </summary>
        [String("deu")] 
        German,

        /// <summary>
        ///     German - Fraktur (contrib)
        /// </summary>
        [String("deu_frak")] 
        GermanFrakturContrib,

        /// <summary>
        ///     Dzongkha
        /// </summary>
        [String("dzo")] 
        Dzongkha,

        /// <summary>
        ///     Greek, Modern (1453-)
        /// </summary>
        [String("ell")] 
        GreekModern,

        /// <summary>
        ///     English
        /// </summary>
        [String("eng")] 
        English,

        /// <summary>
        ///     English, Middle (1100-1500)
        /// </summary>
        [String("enm")] 
        EnglishMiddle,

        /// <summary>
        ///     Esperanto
        /// </summary>
        [String("epo")] 
        Esperanto,

        /// <summary>
        ///     Math / equation detection module
        /// </summary>
        [String("equ")] 
        Math,

        /// <summary>
        ///     Estonian
        /// </summary>
        [String("est")] 
        Estonian,

        /// <summary>
        ///     Basque
        /// </summary>
        [String("eus")] 
        Basque,

        /// <summary>
        ///     Faroese
        /// </summary>
        [String("fao")] 
        Faroese,

        /// <summary>
        ///     Persian
        /// </summary>
        [String("fas")] 
        Persian,

        /// <summary>
        ///     Filipino (old - Tagalog)
        /// </summary>
        [String("fil")] 
        Filipino,

        /// <summary>
        ///     Finnish
        /// </summary>
        [String("fin")] 
        Finnish,

        /// <summary>
        ///     French
        /// </summary>
        [String("fra")] 
        French,

        /// <summary>
        ///     German - Fraktur
        /// </summary>
        [String("frk")] 
        GermanFraktur,

        /// <summary>
        ///     French, Middle (ca.1400-1600)
        /// </summary>
        [String("frm")] 
        FrenchMiddle,

        /// <summary>
        ///     Western Frisian
        /// </summary>
        [String("fry")] 
        WesternFrisian,

        /// <summary>
        ///     Scottish Gaelic
        /// </summary>
        [String("gla")] 
        ScottishGaelic,

        /// <summary>
        ///     Irish
        /// </summary>
        [String("gle")] 
        Irish,

        /// <summary>
        ///     Galician
        /// </summary>
        [String("glg")] 
        Galician,

        /// <summary>
        ///     Greek, Ancient (to 1453) (contrib)
        /// </summary>
        [String("grc")] 
        GreekAncientContrib,

        /// <summary>
        ///     Gujarati
        /// </summary>
        [String("guj")] 
        Gujarati,

        /// <summary>
        ///     Haitian; Haitian Creole
        /// </summary>
        [String("hat")] 
        Haitian,

        /// <summary>
        ///     Hebrew
        /// </summary>
        [String("heb")] 
        Hebrew,

        /// <summary>
        ///     Hindi
        /// </summary>
        [String("hin")] 
        Hindi,

        /// <summary>
        ///     Croatian
        /// </summary>
        [String("hrv")] 
        Croatian,

        /// <summary>
        ///     Hungarian
        /// </summary>
        [String("hun")] 
        Hungarian,

        /// <summary>
        ///     Armenian
        /// </summary>
        [String("hye")] 
        Armenian,

        /// <summary>
        ///     Inuktitut
        /// </summary>
        [String("iku")] 
        Inuktitut,

        /// <summary>
        ///     Indonesian
        /// </summary>
        [String("ind")] 
        Indonesian,

        /// <summary>
        ///     Icelandic
        /// </summary>
        [String("isl")] 
        Icelandic,

        /// <summary>
        ///     Italian
        /// </summary>
        [String("ita")] 
        Italian,

        /// <summary>
        ///     Italian - Old
        /// </summary>
        [String("ita_old")] 
        ItalianOld,

        /// <summary>
        ///     Javanese
        /// </summary>
        [String("jav")] 
        Javanese,

        /// <summary>
        ///     Japanese
        /// </summary>
        [String("jpn")] 
        Japanese,
        
        /// <summary>
        ///     Japanese (vertical)
        /// </summary>
        [String("jpn_vert")] 
        JapaneseVertical,

        /// <summary>
        ///     Kannada
        /// </summary>
        [String("kan")] 
        Kannada,

        /// <summary>
        ///     Georgian
        /// </summary>
        [String("kat")] 
        Georgian,

        /// <summary>
        ///     Georgian - Old
        /// </summary>
        [String("kat_old")] 
        GeorgianOld,

        /// <summary>
        ///     Kazakh
        /// </summary>
        [String("kaz")] 
        Kazakh,

        /// <summary>
        ///     Central Khmer
        /// </summary>
        [String("khm")] 
        CentralKhmer,

        /// <summary>
        ///     Kirghiz; Kyrgyz
        /// </summary>
        [String("kir")] 
        KirghizKyrgyz,

        /// <summary>
        ///     Kurmanji (Kurdish - Latin Script)
        /// </summary>
        [String("kmr")] 
        Kurmanji,

        /// <summary>
        ///     Korean
        /// </summary>
        [String("kor")] 
        Korean,

        /// <summary>
        ///     Korean (vertical)
        /// </summary>
        [String("kor_vert")] 
        KoreanVertical,

        /// <summary>
        ///     Kurdish (Arabic Script)
        /// </summary>
        [String("kur")] 
        KurdishArabicScript,

        /// <summary>
        ///     Lao
        /// </summary>
        [String("lao")] 
        Lao,

        /// <summary>
        ///     Latin
        /// </summary>
        [String("lat")]
        Latin,

        /// <summary>
        ///     Latvian
        /// </summary>
        [String("lav")] 
        Latvian,

        /// <summary>
        ///     Lithuanian
        /// </summary>
        [String("lit")] 
        Lithuanian,

        /// <summary>
        ///     Luxembourgish
        /// </summary>
        [String("ltz")] 
        Luxembourgish,

        /// <summary>
        ///     Malayalam
        /// </summary>
        [String("mal")] 
        Malayalam,

        /// <summary>
        ///     Marathi
        /// </summary>
        [String("mar")] 
        Marathi,

        /// <summary>
        ///     Macedonian
        /// </summary>
        [String("mkd")] 
        Macedonian,

        /// <summary>
        ///     Maltese
        /// </summary>
        [String("mlt")] 
        Maltese,

        /// <summary>
        ///     Mongolian
        /// </summary>
        [String("mon")] 
        Mongolian,

        /// <summary>
        ///     Maori
        /// </summary>
        [String("mri")] 
        Maori,

        /// <summary>
        ///     Malay
        /// </summary>
        [String("msa")] 
        Malay,

        /// <summary>
        ///     Burmese
        /// </summary>
        [String("mya")] 
        Burmese,

        /// <summary>
        ///     Nepali
        /// </summary>
        [String("nep")] 
        Nepali,

        /// <summary>
        ///     Dutch; Flemish
        /// </summary>
        [String("nld")] 
        Dutch,

        /// <summary>
        ///     Norwegian
        /// </summary>
        [String("nor")] Norwegian,

        /// <summary>
        ///     Occitan (post 1500)
        /// </summary>
        [String("oci")] 
        Occitan,

        /// <summary>
        ///     Oriya
        /// </summary>
        [String("ori")] Oriya,

        /// <summary>
        ///     Orientation and script detection module
        /// </summary>
        [String("osd")] 
        Osd,

        /// <summary>
        ///     Panjabi; Punjabi
        /// </summary>
        [String("pan")] 
        Panjabi,

        /// <summary>
        ///     Polish
        /// </summary>
        [String("pol")] Polish,

        /// <summary>
        ///     Portuguese
        /// </summary>
        [String("por")] Portuguese,

        /// <summary>
        ///     Pushto; Pashto
        /// </summary>
        [String("pus")] 
        Pushto,

        /// <summary>
        ///     Quechua
        /// </summary>
        [String("que")] 
        Quechua,

        /// <summary>
        ///     Romanian; Moldavian; Moldovan
        /// </summary>
        [String("ron")] 
        Romanian,

        /// <summary>
        ///     Russian
        /// </summary>
        [String("rus")] 
        Russian,

        /// <summary>
        ///     Sanskrit
        /// </summary>
        [String("san")] 
        Sanskrit,

        /// <summary>
        ///     Sinhala; Sinhalese
        /// </summary>
        [String("sin")] 
        Sinhala,

        /// <summary>
        ///     Slovak
        /// </summary>
        [String("slk")] 
        Slovak,

        /// <summary>
        ///     Slovak - Fraktur (contrib)
        /// </summary>
        [String("slk_frak")] 
        SlovakFrakturContrib,

        /// <summary>
        ///     Slovenian
        /// </summary>
        [String("slv")] 
        Slovenian,

        /// <summary>
        ///     Sindhi
        /// </summary>
        [String("snd")] 
        Sindhi,

        /// <summary>
        ///     Spanish; Castilian
        /// </summary>
        [String("spa")] 
        SpanishCastilian,

        /// <summary>
        ///     Spanish; Castilian - Old
        /// </summary>
        [String("spa_old")] 
        SpanishCastilianOld,

        /// <summary>
        ///     Albanian
        /// </summary>
        [String("sqi")] 
        Albanian,

        /// <summary>
        ///     Serbian
        /// </summary>
        [String("srp")] 
        Serbian,

        /// <summary>
        ///     Serbian - Latin
        /// </summary>
        [String("srp_latn")] 
        SerbianLatin,

        /// <summary>
        ///     Sundanese
        /// </summary>
        [String("sun")] 
        Sundanese,

        /// <summary>
        ///     Swahili
        /// </summary>
        [String("swa")] 
        Swahili,

        /// <summary>
        ///     Swedish
        /// </summary>
        [String("swe")] 
        Swedish,

        /// <summary>
        ///     Syriac
        /// </summary>
        [String("syr")] 
        Syriac,

        /// <summary>
        ///     Tamil
        /// </summary>
        [String("tam")] 
        Tamil,

        /// <summary>
        ///     Tatar
        /// </summary>
        [String("tat")] 
        Tatar,

        /// <summary>
        ///     Telugu
        /// </summary>
        [String("tel")] 
        Telugu,

        /// <summary>
        ///     Tajik
        /// </summary>
        [String("tgk")] 
        Tajik,

        /// <summary>
        ///     Tagalog (new - Filipino)
        /// </summary>
        [String("tgl")] Tagalog,

        /// <summary>
        ///     Thai
        /// </summary>
        [String("tha")] Thai,

        /// <summary>
        ///     Tigrinya
        /// </summary>
        [String("tir")] 
        Tigrinya,

        /// <summary>
        ///     Tonga
        /// </summary>
        [String("ton")]
        Tonga,

        /// <summary>
        ///     Turkish
        /// </summary>
        [String("tur")] 
        Turkish,

        /// <summary>
        ///     Uighur; Uyghur
        /// </summary>
        [String("uig")] 
        Uighur,

        /// <summary>
        ///     Ukrainian
        /// </summary>
        [String("ukr")] 
        Ukrainian,

        /// <summary>
        ///     Urdu
        /// </summary>
        [String("urd")] 
        Urdu,

        /// <summary>
        ///     Uzbek
        /// </summary>
        [String("uzb")]
        Uzbek,

        /// <summary>
        ///     Uzbek - Cyrilic
        /// </summary>
        [String("uzb_cyrl")] 
        UzbekCyrilic,

        /// <summary>
        ///     Vietnamese
        /// </summary>
        [String("vie")] 
        Vietnamese,

        /// <summary>
        ///     Yiddish
        /// </summary>
        [String("yid")] 
        Yiddish,

        /// <summary>
        ///     Yoruba
        /// </summary>
        [String("yor")] 
        Yoruba
    }
    #endregion

    /// <summary>
    ///     A helper class for working with the <see cref="Language"/>
    /// </summary>
    public static class LanguageHelper
    {
        #region StringToEnum
        /// <summary>
        ///     Returns the <see cref="Language"/> enum that has the <see cref="StringAttribute"/>
        ///     with the corresponding <paramref name="languageString"/>
        /// </summary>
        /// <param name="languageString">The language string</param>
        /// <returns><see cref="Language"/></returns>
        public static Language StringToEnum(string languageString)
        {
            foreach (var language in (Language[])Enum.GetValues(typeof(Language)))
            {
                var value = language.GetAttributeOfType<StringAttribute>().Value;

                if (value == languageString)
                    return language;
            }

            return Language.Unknown;
        }
        #endregion

        #region EnumToString
        /// <summary>
        ///     Returns the <see cref="StringAttribute"/> for the given <paramref name="language"/> enum
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string EnumToString(Language language)
        {
            return language.GetAttributeOfType<StringAttribute>().Value;
        }
        #endregion

        #region GetAttributeOfType
        /// <summary>
        ///     Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>
        ///     <code>
        ///         var value = enum.GetAttributeOfType&lt;StringAttribute>().Value;&gt;
        ///     </code>
        /// </example>
        internal static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? (T)attributes[0] : null;
        }
        #endregion
    }
}