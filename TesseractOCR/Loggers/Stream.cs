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

namespace TesseractOCR.Loggers;

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
    /// <param name="stream">The stream to write log messages to; it is disposed with the logger.</param>
    public Stream(System.IO.Stream stream)
    {
        _stream = stream;
    }
    #endregion

    #region BeginScope
    /// <summary>
    ///     Sets the current instance identifier from the state.
    /// </summary>
    /// <typeparam name="TState">The type of the scope state.</typeparam>
    /// <param name="state">The state used to set the current instance identifier.</param>
    /// <returns>Always null.</returns>
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
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>Always <c>true</c>, regardless of whether <see cref="Log{TState}"/> writes that level.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }
    #endregion

    #region Log
    /// <summary>
    ///     Writes debug, information, or error messages to the stream when it is writable.
    /// </summary>
    /// <typeparam name="TState">The type of the log state.</typeparam>
    /// <param name="logLevel">The severity of the message.</param>
    /// <param name="eventId">The event identifier; it is not included in the output.</param>
    /// <param name="state">The state passed to the formatter.</param>
    /// <param name="exception">The exception passed to the formatter, if any.</param>
    /// <param name="formatter">Formats the state and exception into a message.</param>
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
    /// <summary>
    ///     Disposes the underlying stream and releases its resources.
    /// </summary>
    public void Dispose()
    {
        if (_stream == null)
            return;

        _stream.Dispose();
        _stream = null;
    }
    #endregion
}