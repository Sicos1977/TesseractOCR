//
// DisposableBase.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright 2012-2019 Charles Weld
// Copyright 2021-2027 Kees van Spelde
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
using TesseractOCR.Helpers;

namespace TesseractOCR;

/// <summary>
///     Provides disposal state, a disposal event, and a finalizer for derived objects.
/// </summary>
public abstract class DisposableBase : IDisposable
{
    #region Events
    /// <summary>
    ///     Occurs after the object is explicitly disposed.
    /// </summary>
    public event EventHandler<EventArgs> Disposed;
    #endregion

    #region Properties
    /// <summary>
    ///     Returns <c>true</c> when this object is disposed
    /// </summary>
    public bool IsDisposed { get; private set; }
    #endregion

    #region Destructor
    /// <summary>
    ///     Releases resources when the object was not explicitly disposed.
    /// </summary>
    ~DisposableBase()
    {
        Dispose(false);
        Logger.LogError($"{this} was not disposed");
    }
    #endregion

    #region Dispose
    /// <summary>
    ///     Releases resources, marks the object as disposed, and raises <see cref="Disposed"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        IsDisposed = true;
        GC.SuppressFinalize(this);

        Disposed?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region VerifyNotDisposed
    /// <summary>
    ///     Verifies that this object has not been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    protected virtual void VerifyNotDisposed()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(ToString());
    }
    #endregion

    #region Abstracts
    /// <summary>
    ///     Releases resources owned by the derived object.
    /// </summary>
    /// <param name="disposing"><c>true</c> for explicit disposal; <c>false</c> during finalization.</param>
    protected abstract void Dispose(bool disposing);
    #endregion
}