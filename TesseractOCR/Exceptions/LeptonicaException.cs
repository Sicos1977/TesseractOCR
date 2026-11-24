//
// LeptonicaException.cs
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
using System.Runtime.Serialization;

namespace TesseractOCR.Exceptions;

/// <summary>Represents an error reported by Leptonica.</summary>
[Serializable]
public class LeptonicaException : Exception
{
    #region Constructors
    /// <summary>Creates a Leptonica exception.</summary>
    public LeptonicaException()
    {
    }

    /// <summary>Creates a Leptonica exception with an error message.</summary>
    /// <param name="message">The error message.</param>
    public LeptonicaException(string message) : base(message)
    {
    }

    /// <summary>Creates a Leptonica exception with an underlying cause.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="inner">The underlying exception.</param>
    public LeptonicaException(string message, Exception inner) : base(message, inner)
    {
    }

    /// <summary>Restores a serialized Leptonica exception.</summary>
    /// <param name="info">The serialized exception data.</param>
    /// <param name="context">The serialization context.</param>
    protected LeptonicaException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
    #endregion
}