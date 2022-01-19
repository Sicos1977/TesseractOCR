//
// InteropRuntimeImplementer.cs
//
// Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//
// Copyright (c) 2014 Andrey Akinshin
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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;

namespace TesseractOCR.InteropDotNet
{
    public static class InteropRuntimeImplementer
    {
        #region CreateInstance
        public static T CreateInstance<T>() where T : class
        {
            var interfaceType = typeof(T);
            if (!typeof(T).IsInterface)
                throw new Exception($"The type {interfaceType.Name} should be an interface");
            if (!interfaceType.IsPublic)
                throw new Exception($"The interface {interfaceType.Name} should be public");

            var assemblyName = GetAssemblyName(interfaceType);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

            var typeName = GetImplementationTypeName(assemblyName, interfaceType);
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public, typeof(object), new[] { interfaceType });
            var methods = BuildMethods(interfaceType);

            ImplementDelegates(assemblyName, moduleBuilder, methods);
            ImplementFields(typeBuilder, methods);
            ImplementMethods(typeBuilder, methods);
            ImplementConstructor(typeBuilder, methods);

            var implementationType = typeBuilder.CreateType();
            return (T)Activator.CreateInstance(implementationType, LibraryLoader.Instance);
        }
#endregion

#region BuildMethods
        private static MethodItem[] BuildMethods(Type interfaceType)
        {
            var methodInfoArray = interfaceType.GetMethods();
            var methods = new MethodItem[methodInfoArray.Length];
            for (var i = 0; i < methodInfoArray.Length; i++)
            {
                methods[i] = new MethodItem { Info = methodInfoArray[i] };
                var attribute = GetRuntimeDllImportAttribute(methodInfoArray[i]);
                if (attribute == null)
                    throw new Exception(
                        $"Method '{methodInfoArray[i].Name}' of interface '{interfaceType.Name}' should be marked with the RuntimeDllImport attribute");
                methods[i].DllImportAttribute = attribute;
            }

            return methods;
        }
#endregion

#region ImplementDelegates
        private static void ImplementDelegates(string assemblyName, ModuleBuilder moduleBuilder,
            IEnumerable<MethodItem> methods)
        {
            foreach (var method in methods)
                method.DelegateType = ImplementMethodDelegate(assemblyName, moduleBuilder, method);
        }
#endregion

#region ImplementMethodDelegate
        private static Type ImplementMethodDelegate(string assemblyName, ModuleBuilder moduleBuilder, MethodItem method)
        {
            // Const
            const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                                      MethodAttributes.NewSlot | MethodAttributes.Virtual;

            // Initial
            var delegateName = GetDelegateName(assemblyName, method.Info);
            var delegateBuilder = moduleBuilder.DefineType(delegateName,
                TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.Sealed, typeof(MulticastDelegate));

            // UnmanagedFunctionPointer
            var importAttribute = method.DllImportAttribute;
            var attributeCtor =
                typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) });
            if (attributeCtor == null)
                throw new Exception("There is no the target constructor of the UnmanagedFunctionPointerAttribute");
            var attributeBuilder = new CustomAttributeBuilder(attributeCtor,
                new object[] { importAttribute.CallingConvention },
                new[]
                {
                    typeof(UnmanagedFunctionPointerAttribute).GetField("CharSet"),
                    typeof(UnmanagedFunctionPointerAttribute).GetField("BestFitMapping"),
                    typeof(UnmanagedFunctionPointerAttribute).GetField("ThrowOnUnmappableChar"),
                    typeof(UnmanagedFunctionPointerAttribute).GetField("SetLastError")
                },
                new object[]
                {
                    importAttribute.CharSet,
                    importAttribute.BestFitMapping,
                    importAttribute.ThrowOnUnmappableChar,
                    importAttribute.SetLastError
                });
            delegateBuilder.SetCustomAttribute(attributeBuilder);


            // ctor
            var ctorBuilder = delegateBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig |
                                                                MethodAttributes.SpecialName |
                                                                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                new[] { typeof(object), typeof(IntPtr) });
            ctorBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
            ctorBuilder.DefineParameter(1, ParameterAttributes.HasDefault, "object");
            ctorBuilder.DefineParameter(2, ParameterAttributes.HasDefault, "method");

            // Invoke
            var parameters = GetParameterInfoArray(method.Info);
            var methodBuilder =
                DefineMethod(delegateBuilder, "Invoke", methodAttributes, method.ReturnType, parameters);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // BeginInvoke
            parameters = GetParameterInfoArray(method.Info, InfoArrayMode.BeginInvoke);
            methodBuilder = DefineMethod(delegateBuilder, "BeginInvoke", methodAttributes, typeof(IAsyncResult),
                parameters);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // EndInvoke
            parameters = GetParameterInfoArray(method.Info, InfoArrayMode.EndInvoke);
            methodBuilder = DefineMethod(delegateBuilder, "EndInvoke", methodAttributes, method.ReturnType, parameters);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // Create type
            return delegateBuilder.CreateType();
        }
#endregion

#region ImplementFields
        private static void ImplementFields(TypeBuilder typeBuilder, IEnumerable<MethodItem> methods)
        {
            foreach (var method in methods)
            {
                var fieldName = method.Info.Name + "Field";
                var fieldBuilder = typeBuilder.DefineField(fieldName, method.DelegateType, FieldAttributes.Private);
                method.FieldInfo = fieldBuilder;
            }
        }
#endregion

#region ImplementMethods
        private static void ImplementMethods(TypeBuilder typeBuilder, IEnumerable<MethodItem> methods)
        {
            foreach (var method in methods)
            {
                var infoArray = GetParameterInfoArray(method.Info);
                var methodBuilder = DefineMethod(typeBuilder, method.Name,
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                    MethodAttributes.Final | MethodAttributes.Virtual,
                    method.ReturnType, infoArray);

                var ilGen = methodBuilder.GetILGenerator();

                // Load this
                ilGen.Emit(OpCodes.Ldarg_0);
                // Load field
                ilGen.Emit(OpCodes.Ldfld, method.FieldInfo);
                // Load arguments
                for (var i = 0; i < infoArray.Length; i++)
                    LdArg(ilGen, i + 1);
                // Invoke delegate
                ilGen.Emit(OpCodes.Callvirt, method.DelegateType.GetMethod("Invoke"));
                // Return value
                ilGen.Emit(OpCodes.Ret);

                // Associate the method body with the interface method
                typeBuilder.DefineMethodOverride(methodBuilder, method.Info);
            }
        }
#endregion

#region ImplementConstructor
        private static void ImplementConstructor(TypeBuilder typeBuilder, MethodItem[] methods)
        {
            // Preparing
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, new[] { typeof(LibraryLoader) });
            ctorBuilder.DefineParameter(1, ParameterAttributes.HasDefault, "loader");
            if (typeBuilder.BaseType == null)
                throw new Exception("There is no a BaseType of typeBuilder");
            var baseCtor = typeBuilder.BaseType.GetConstructor(new Type[0]);
            if (baseCtor == null)
                throw new Exception("There is no a default constructor of BaseType of typeBuilder");

            // Build list of library names
            var libraries = new List<string>();
            foreach (var method in methods)
            {
                var libraryName = method.DllImportAttribute.LibraryFileName;
                if (!libraries.Contains(libraryName))
                    libraries.Add(libraryName);
            }

            // Create ILGenerator
            var ilGen = ctorBuilder.GetILGenerator();
            // Declare locals for library handles
            for (var i = 0; i < libraries.Count; i++)
                ilGen.DeclareLocal(typeof(IntPtr));
            // Declare locals for a method handle
            ilGen.DeclareLocal(typeof(IntPtr));
            // Load this
            ilGen.Emit(OpCodes.Ldarg_0);
            // Run objector..ctor()
            ilGen.Emit(OpCodes.Call, baseCtor);
            for (var i = 0; i < libraries.Count; i++)
            {
                // Preparing
                var library = libraries[i];
                // Load LibraryLoader
                ilGen.Emit(OpCodes.Ldarg_1);
                // Load libraryName
                ilGen.Emit(OpCodes.Ldstr, library);
                // Load null
                ilGen.Emit(OpCodes.Ldnull);
                // Call LibraryLoader.LoadLibrary(libraryName, null)
                ilGen.Emit(OpCodes.Callvirt, typeof(LibraryLoader).GetMethod("LoadLibrary"));
                // Store libraryHandle in locals[i]
                ilGen.Emit(OpCodes.Stloc, i);
            }

            foreach (var method in methods)
            {
                // Preparing
                var libraryIndex = libraries.IndexOf(method.DllImportAttribute.LibraryFileName);
                var methodName = method.DllImportAttribute.EntryPoint ?? method.Info.Name;
                // Load Library Loader
                ilGen.Emit(OpCodes.Ldarg_1);
                // Load libraryHandle (locals[libraryIndex])
                ilGen.Emit(OpCodes.Ldloc, libraryIndex);
                // Load methodName
                ilGen.Emit(OpCodes.Ldstr, methodName);
                // Call LibraryLoader.GetProcAddress(libraryHandle, methodName)
                ilGen.Emit(OpCodes.Callvirt, typeof(LibraryLoader).GetMethod("GetProcAddress"));
                // Store methodHandle in locals
                ilGen.Emit(OpCodes.Stloc, libraries.Count);
                // Load this
                ilGen.Emit(OpCodes.Ldarg_0);
                // Load methodHandle from locals
                ilGen.Emit(OpCodes.Ldloc_1);
                // Load methodDelegate token
                ilGen.Emit(OpCodes.Ldtoken, method.DelegateType);
                // Call typeof(methodDelegate)                                
                ilGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                // Call Marshal.GetDelegateForFunctionPointer(methodHandle, typeof(methodDelegate))
                ilGen.Emit(OpCodes.Call, typeof(Marshal).GetMethod("GetDelegateForFunctionPointer",
                    new[] { typeof(IntPtr), typeof(Type) }));
                // Cast result to typeof(methodDelegate)
                ilGen.Emit(OpCodes.Castclass, method.DelegateType);
                // Store result in methodField
                ilGen.Emit(OpCodes.Stfld, method.FieldInfo);
            }

            // Return
            ilGen.Emit(OpCodes.Ret);
        }
#endregion

#region GetRuntimeDllImportAttribute
        private static RuntimeDllImportAttribute GetRuntimeDllImportAttribute(MethodInfo methodInfo)
        {
            var attributes = methodInfo.GetCustomAttributes(typeof(RuntimeDllImportAttribute), true);
            if (attributes.Length == 0)
                throw new Exception($"RuntimeDllImportAttribute for method '{methodInfo.Name}' not found");
            return (RuntimeDllImportAttribute)attributes[0];
        }
#endregion

#region LdArg
        private static void LdArg(ILGenerator ilGen, int index)
        {
            switch (index)
            {
                case 0:
                    ilGen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    ilGen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilGen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    ilGen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    ilGen.Emit(OpCodes.Ldarg, index);
                    break;
            }
        }
#endregion

#region DefineMethod
        private static MethodBuilder DefineMethod(TypeBuilder typeBuilder, string name,
            MethodAttributes attributes, Type returnType, LightParameterInfo[] infoArray)
        {
            var methodBuilder =
                typeBuilder.DefineMethod(name, attributes, returnType, GetParameterTypeArray(infoArray));
            for (var parameterIndex = 0; parameterIndex < infoArray.Length; parameterIndex++)
                methodBuilder.DefineParameter(parameterIndex + 1,
                    infoArray[parameterIndex].Attributes, infoArray[parameterIndex].Name);
            return methodBuilder;
        }
#endregion

#region Method helpers
        private class MethodItem
        {
            public MethodInfo Info { get; set; }
            public RuntimeDllImportAttribute DllImportAttribute { get; set; }

            public Type DelegateType { get; set; }
            public FieldInfo FieldInfo { get; set; }

            public string Name => Info.Name;
            public Type ReturnType => Info.ReturnType;
        }

        private class LightParameterInfo
        {
            public LightParameterInfo(ParameterInfo info)
            {
                Type = info.ParameterType;
                Name = info.Name;
                Attributes = info.Attributes;
            }

            public LightParameterInfo(Type type, string name)
            {
                Type = type;
                Name = name;
                Attributes = ParameterAttributes.HasDefault;
            }

            public Type Type { get; }
            public string Name { get; }
            public ParameterAttributes Attributes { get; }
        }

        private enum InfoArrayMode
        {
            Invoke,
            BeginInvoke,
            EndInvoke
        }

        private static LightParameterInfo[] GetParameterInfoArray(MethodInfo methodInfo,
            InfoArrayMode mode = InfoArrayMode.Invoke)
        {
            var parameters = methodInfo.GetParameters();
            var infoList = new List<LightParameterInfo>();
            
            foreach (var parameter in parameters)
                if (mode != InfoArrayMode.EndInvoke || parameter.ParameterType.IsByRef)
                    infoList.Add(new LightParameterInfo(parameter));

            if (mode == InfoArrayMode.BeginInvoke)
            {
                infoList.Add(new LightParameterInfo(typeof(AsyncCallback), "callback"));
                infoList.Add(new LightParameterInfo(typeof(object), "object"));
            }

            if (mode == InfoArrayMode.EndInvoke)
                infoList.Add(new LightParameterInfo(typeof(IAsyncResult), "result"));

            var infoArray = new LightParameterInfo[infoList.Count];

            for (var i = 0; i < infoList.Count; i++)
                infoArray[i] = infoList[i];
            
            return infoArray;
        }

        private static Type[] GetParameterTypeArray(LightParameterInfo[] infoArray)
        {
            var typeArray = new Type[infoArray.Length];
            for (var i = 0; i < infoArray.Length; i++)
                typeArray[i] = infoArray[i].Type;
            return typeArray;
        }
#endregion

#region GetAssemblyName
        private static string GetAssemblyName(Type interfaceType)
        {
            return $"InteropRuntimeImplementer.{GetSubstantialName(interfaceType)}Instance";
        }
#endregion

#region GetImplementationTypeName
        private static string GetImplementationTypeName(string assemblyName, Type interfaceType)
        {
            return $"{assemblyName}.{GetSubstantialName(interfaceType)}Implementation";
        }
#endregion

#region GetSubstantialName
        private static string GetSubstantialName(Type interfaceType)
        {
            var name = interfaceType.Name;
            if (name.StartsWith("I"))
                name = name.Substring(1);
            return name;
        }
#endregion

#region GetDelegateName
        private static string GetDelegateName(string assemblyName, MethodInfo methodInfo)
        {
            return $"{assemblyName}.{methodInfo.Name}Delegate";
        }
#endregion
    }
}