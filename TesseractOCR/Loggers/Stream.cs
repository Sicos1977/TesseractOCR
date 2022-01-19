//
// Stream.cs
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
using System.Text;
using Microsoft.Extensions.Logging;

namespace TesseractOCR.Loggers
{
    /// <summary>
    ///     Writes log information to a stream
    /// </summary>
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
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
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
