//
// Console.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2022 Magic-Sessions. (www.magic-sessions.com)
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

using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Loggers
{
    /// <summary>
    ///     Writes log information to console at the <see cref="LogLevel.Debug"/>, <see cref="LogLevel.Error"/>
    ///     and <see cref="LogLevel.Information"/> <see cref="LogLevel"/>
    /// </summary>
    /// <remarks>
    ///     Other levels are ignored
    /// </remarks>
    public class Console : Stream
    {
        public Console() : base(System.Console.OpenStandardOutput())
        {
        }
    }
}
