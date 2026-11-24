//
// BaseApi.cs
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TesseractOCR.Enums;
using TesseractOCR.Internal;
using TesseractOCR.InteropDotNet;
#pragma warning disable CS1591

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace TesseractOCR.Interop;

internal static class TessApi
{
    #region Fields
    private static ITessApiSignatures native;
    #endregion

    #region Properties
    public static ITessApiSignatures Native
    {
        get
        {
            if (native == null)
                Initialize();
            return native;
        }
    }
    #endregion

    #region BaseApiGetVersion
    public static string BaseApiGetVersion()
    {
        var versionHandle = Native.GetVersion();
        if (versionHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(versionHandle, Encoding.UTF8);
        return result;
    }
    #endregion

    #region BaseApiLoadedLanguages
    public static List<Language> BaseApiLoadedLanguages(HandleRef handle)
    {
        var languageHandle = Native.BaseAPIGetLoadedLanguagesAsVector(handle);
        var result = new List<Language>();
        var i = 0;
        var size = Marshal.SizeOf(typeof(IntPtr));
        var p = Marshal.ReadIntPtr(languageHandle, i);

        while (p != IntPtr.Zero)
        {
            var str = MarshalHelper.PtrToString(p, Encoding.UTF8);
            result.Add(LanguageHelper.StringToEnum(str));
            p = Marshal.ReadIntPtr(languageHandle, i += size);
        }

        return result;
    }
    #endregion

    #region BaseAPIGetAvailableLanguagesAsVector
    public static List<Language> BaseAPIGetAvailableLanguagesAsVector(HandleRef handle)
    {
        var languageHandle = Native.BaseAPIGetAvailableLanguagesAsVector(handle);
        var result = new List<Language>();
        var i = 0;
        var size = Marshal.SizeOf(typeof(IntPtr));
        var p = Marshal.ReadIntPtr(languageHandle, i);

        while(p != IntPtr.Zero)
        {
            var str = MarshalHelper.PtrToString(p, Encoding.UTF8);
            result.Add(LanguageHelper.StringToEnum(str));
            p = Marshal.ReadIntPtr(languageHandle, i += size);
        }

        return result;
    }
    #endregion

    #region BaseApiGetHOcrText
    public static string BaseApiGetHOcrText(HandleRef handle, int pageNum)
    {
        var txtHandle = Native.BaseApiGetHOcrTextInternal(handle, pageNum);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region BaseApiGetAltoText
    public static string BaseApiGetAltoText(HandleRef handle, int pageNum)
    {
        var txtHandle = Native.BaseApiGetAltoTextInternal(handle, pageNum);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region BaseApiGetTsvText
    public static string BaseApiGetTsvText(HandleRef handle, int pageNum)
    {
        var txtHandle = Native.BaseApiGetTsvTextInternal(handle, pageNum);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region BaseApiGetBoxText
    public static string BaseApiGetBoxText(HandleRef handle, int pageNum)
    {
        var txtHandle = Native.BaseApiGetBoxTextInternal(handle, pageNum);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region BaseApiGetLSTMBoxText
    public static string BaseApiGetLSTMBoxText(HandleRef handle, int pageNum)
    {
        var txtHandle = Native.BaseApiGetLstmBoxTextInternal(handle, pageNum);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region BaseApiGetWordStrBoxText
    public static string BaseApiGetWordStrBoxText(HandleRef handle, int pageNum)
    {
        var txtHandle = Native.BaseApiGetWordStrBoxTextInternal(handle, pageNum);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region BaseApiGetUNLVText
    public static string BaseApiGetUnlvText(HandleRef handle)
    {
        var txtHandle = Native.BaseApiGetUnlvTextInternal(handle);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region BaseApiGetStringVariable
    public static string BaseApiGetStringVariable(HandleRef handle, string name)
    {
        var resultHandle = Native.BaseApiGetStringVariableInternal(handle, name);
        return resultHandle != IntPtr.Zero ? MarshalHelper.PtrToString(resultHandle, Encoding.UTF8) : null;
    }
    #endregion

    #region BaseApiGetUTF8Text
    public static string BaseApiGetUTF8Text(HandleRef handle)
    {
        var txtHandle = Native.BaseApiGetUTF8TextInternal(handle);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region BaseApiInit
    public static int BaseApiInit(HandleRef handle, string datapath, string language, int mode,
        IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugParams)
    {
        Guard.Require("handle", handle.Handle != IntPtr.Zero, "Handle for BaseApi, created through BaseApiCreate is required");
        Guard.RequireNotNullOrEmpty("language", language);
        Guard.RequireNotNull("configFiles", configFiles);
        Guard.RequireNotNull("initialValues", initialValues);

        var configFilesArray = new List<string>(configFiles).ToArray();
        var varNames = new string[initialValues.Count];
        var varValues = new string[initialValues.Count];
        var i = 0;

        foreach (var pair in initialValues)
        {
            Guard.Require("initialValues", !string.IsNullOrEmpty(pair.Key), "Variable must have a name");
            Guard.Require("initialValues", pair.Value != null, "Variable '{0}': The type '{1}' is not supported", pair.Key, pair.Value?.GetType());
            
            varNames[i] = pair.Key;

            if (TessConvert.TryToString(pair.Value, out var varValue))
                varValues[i] = varValue;
            else
                throw new ArgumentException(
                    $"Variable '{pair.Key}': The type '{pair.Value?.GetType()}' is not supported",
                    nameof(initialValues));
            i++;
        }

        return Native.BaseApiInit4(handle, datapath, language, mode,
            configFilesArray, configFilesArray.Length,
            varNames, varValues, new UIntPtr((uint)varNames.Length), setOnlyNonDebugParams);
    }
    #endregion

    #region BaseApiSetDebugVariable
    public static int BaseApiSetDebugVariable(HandleRef handle, string name, string value)
    {
        var valuePtr = IntPtr.Zero;
        try
        {
            valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
            return Native.BaseApiSetDebugVariable(handle, name, valuePtr);
        }
        finally
        {
            if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
        }
    }
    #endregion

    #region BaseApiSetVariable
    public static int BaseApiSetVariable(HandleRef handle, string name, string value)
    {
        var valuePtr = IntPtr.Zero;

        try
        {
            valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
            return Native.BaseApiSetVariable(handle, name, valuePtr);
        }
        finally
        {
            if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
        }
    }
    #endregion

    #region Initialize
    public static void Initialize()
    {
        if (native != null) return;
        LeptonicaApi.Initialize();
        Helper.SetPath();
        native = InteropRuntimeImplementer.CreateInstance<ITessApiSignatures>();
    }
    #endregion

    #region ResultIteratorWordRecognitionLanguage
    public static string ResultIteratorWordRecognitionLanguage(HandleRef handle)
    {
        var txtHandle = Native.ResultIteratorWordRecognitionLanguageInternal(handle);

        return txtHandle != IntPtr.Zero
            ? MarshalHelper.PtrToString(txtHandle, Encoding.UTF8)
            : null;
    }
    #endregion

    #region ResultIteratorGetUTF8Text
    public static string ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level)
    {
        var txtHandle = Native.ResultIteratorGetUTF8TextInternal(handle, level);
        if (txtHandle == IntPtr.Zero) return null;
        var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
        Native.DeleteText(txtHandle);
        return result;
    }
    #endregion

    #region ChoiceIteratorGetUTF8Text
    /// <summary>
    ///     Returns the null terminated UTF-8 encoded text string for the current choice
    /// </summary>
    /// <remarks>
    ///     NOTE: Unlike LTRResultIterator::GetUTF8Text, the return points to an
    ///     internal structure and should NOT be delete[]ed to free after use.
    /// </remarks>
    /// <param name="choiceIteratorHandle"></param>
    /// <returns>string</returns>
    internal static string ChoiceIteratorGetUTF8Text(HandleRef choiceIteratorHandle)
    {
        Guard.Require("choiceIteratorHandle", choiceIteratorHandle.Handle != IntPtr.Zero, "ChoiceIterator Handle cannot be a null IntPtr and is required");
        var txtChoiceHandle = Native.ChoiceIteratorGetUTF8TextInternal(choiceIteratorHandle);
        return MarshalHelper.PtrToString(txtChoiceHandle, Encoding.UTF8);
    }
    #endregion
}