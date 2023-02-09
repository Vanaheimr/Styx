/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Security;
using System.Runtime.InteropServices;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the SecureString class.
    /// </summary>
    public static class SecureStringExtensions
    {

        public static String UnsecureString(this SecureString SecureText)
        {

            if (SecureText == null)
                throw new ArgumentNullException(nameof(SecureText), "The given secure text must not be null!");

            var UnmanagedString = IntPtr.Zero;

            try
            {
                UnmanagedString = Marshal.SecureStringToGlobalAllocUnicode(SecureText);
                return Marshal.PtrToStringUni(UnmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(UnmanagedString);
            }

        }

    }

}
