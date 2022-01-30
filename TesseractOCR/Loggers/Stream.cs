//
// Stream.cs
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

using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TesseractOCR.Loggers
{
    /// <summary>
    ///     Writes log information to a stream at the <see cref="LogLevel.Debug"/>, <see cref="LogLevel.Error"/>
    ///     and <see cref="LogLevel.Information"/> <see cref="LogLevel"/>
    /// </summary>
    /// <remarks>
    ///     Other levels are ignored
    /// </remarks>
    public class Stream : ILogger, IDisposable
    {
        #region Fields
        private System.IO.Stream _stream;
        private string _instanceId;
        #endregion

        #region Constructors
        /// <summary>
        ///     Logs information to the given <paramref name="stream"/>
        /// </summary>
        /// <param name="stream"></param>
        public Stream(System.IO.Stream stream)
        {
            _stream = stream;
        }
        #endregion

        #region BeginScope
        public IDisposable BeginScope<TState>(TState state)
        {
            _instanceId = state?.ToString();
            return null;
        }
        #endregion

        #region IsEnabled
        /// <summary>
        ///     Will always return <c>true</c>
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        #endregion

        #region Log
        /// <summary>
        ///     Writes logging to the log
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Warning:
                case LogLevel.Critical:
                case LogLevel.None:
                    return;

                case LogLevel.Debug:
                case LogLevel.Information:
                case LogLevel.Error:
                    break;
            }

            var message = $"{formatter(state, exception)}";

            if (_stream == null || !_stream.CanWrite) return;
            var line = $"{DateTime.Now:yyyy-MM-ddTHH:mm:ss.fff}{(_instanceId != null ? $" - {_instanceId}" : string.Empty)} - {message}{Environment.NewLine}";
            var bytes = Encoding.UTF8.GetBytes(line);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Flush();
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (_stream == null)
                return;

            _stream.Dispose();
            _stream = null;
        }
        #endregion
    }
}
