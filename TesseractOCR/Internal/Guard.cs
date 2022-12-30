//
// Guard.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
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

using System;
using System.Diagnostics;
using TesseractOCR.Helpers;
using TesseractOCR.Loggers;

// ReSharper disable UnusedMember.Global

namespace TesseractOCR.Internal
{
    /// <summary>
    ///     Generic pre-condition checks
    /// </summary>
    internal static class Guard
    {
        #region Require
        /// <summary>
        ///     Ensures the given <paramref name="condition" /> is true.
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="condition" /> is not true.</exception>
        /// <param name="paramName">The name of the parameter, used when generating the exception.</param>
        /// <param name="condition">The value of the parameter to check.</param>
        /// <param name="message">The error message.</param>
        [DebuggerHidden]
        public static void Require(string paramName, bool condition, string message)
        {
            if (condition) return;
            Logger.LogError(message);
            throw new ArgumentException(message, paramName);
        }

        /// <summary>
        ///     Ensures the given <paramref name="condition" /> is true.
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="condition" /> is not true.</exception>
        /// <param name="paramName">The name of the parameter, used when generating the exception.</param>
        /// <param name="condition">The value of the parameter to check.</param>
        /// <param name="message">The error message.</param>
        /// <param name="args">The message argument used to format <paramref name="message" />.</param>
        [DebuggerHidden]
        public static void Require(string paramName, bool condition, string message, params object[] args)
        {
            if (condition) return;
            var error = string.Format(message, args);
            Logger.LogError(error);
            throw new ArgumentException(error, paramName);
        }
        #endregion

        #region RequireNotNull
        [DebuggerHidden]
        public static void RequireNotNull(string argName, object value)
        {
            if (value != null) return;
            var error = $"Argument '{argName}' must not be null";
            Logger.LogError(error);
            throw new ArgumentException(error);
        }
        #endregion

        #region RequireNotNullOrEmpty
        /// <summary>
        ///     Ensures the given <paramref name="value" /> is not null or empty.
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="value" /> is null or empty.</exception>
        /// <param name="paramName">The name of the parameter, used when generating the exception.</param>
        /// <param name="value">The value of the parameter to check.</param>
        [DebuggerHidden]
        public static void RequireNotNullOrEmpty(string paramName, string value)
        {
            RequireNotNull(paramName, value);
            if (value.Length != 0) return;
            var error = $"The argument '{paramName}' must not be null or empty";
            Logger.LogError(error);
            throw new ArgumentException(paramName, error);
        }
        #endregion

        #region Verify
        /// <summary>
        ///     Verifies the given <paramref name="condition" /> is <c>True</c>; throwing an
        ///     <see cref="InvalidOperationException" /> when the condition is not met.
        /// </summary>
        /// <param name="condition">The condition to be tested.</param>
        /// <param name="message">The error message to raise if <paramref name="condition" /> is <c>False</c>.</param>
        /// <param name="args">Optional formatting arguments.</param>
        [DebuggerHidden]
        public static void Verify(bool condition, string message, params object[] args)
        {
            if (condition) return;
            var error = string.Format(message, args);
            Logger.LogError(error);
            throw new InvalidOperationException(error);
        }
        #endregion
    }
}