//
// Logger.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2022 Magic-Sessions. (www.magic-sessions.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using Microsoft.Extensions.Logging;

namespace TesseractOCR.Helpers
{
    internal static class Logger
    {
        #region Fields
        /// <summary>
        ///     When set then logging is written to this ILogger instance
        /// </summary>
        [ThreadStatic]
        private static ILogger _logger;
        #endregion

        #region Properties
        /// <summary>
        ///     An unique id that can be used to identify the logging of the converter when
        ///     calling the code from multiple threads and writing all the logging to the same file
        /// </summary>
        public static string InstanceId { get; set; }

        /// <summary>
        ///     Sets the logger interface
        /// </summary>
        public static ILogger LoggerInterface
        {
            set => _logger = value;
        }
        #endregion

        #region LogInformation
        /// <summary>
        ///     Writes an information line to the <see cref="_logger" />
        /// </summary>
        /// <param name="message">The message to write</param>
        internal static void LogInformation(string message)
        {
            try
            {
                if (_logger == null) return;
                using (_logger.BeginScope(InstanceId))
                    _logger.LogInformation(message);
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
        }
        #endregion

        #region LogError
        /// <summary>
        ///     Writes an error line to the <see cref="_logger" />
        /// </summary>
        /// <param name="message">The message to write</param>
        internal static void LogError(string message)
        {
            try
            {
                if (_logger == null) return;
                using (_logger.BeginScope(InstanceId))
                    _logger.LogError(message);
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
        }
        #endregion

        #region LogDebug
        /// <summary>
        ///     Writes a debug line to the <see cref="_logger" />
        /// </summary>
        /// <param name="message">The message to write</param>
        internal static void LogDebug(string message)
        {
            try
            {
                if (_logger == null) return;
                using (_logger.BeginScope(InstanceId))
                    _logger.LogDebug(message);
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
        }
        #endregion
    }
}