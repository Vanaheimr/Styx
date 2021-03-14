/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

using System;

namespace org.GraphDefined.Vanaheimr.Illias
{

    public enum ApplicationRunTypes
    {

        None,

        MonoConsole,
        MonoService,

        WindowsConsole,
        WindowsService

    }


    public static class ApplicationRunType
    {

        public static ApplicationRunTypes GetRunType()
        {

            if (Type.GetType("Mono.Runtime") != null)
            {

                // It's a console application if 'bool Mono.Unix.Native.Syscall.isatty(0)' in Mono.Posix.dll
                var monoPosix    = System.Reflection.Assembly.Load("Mono.Posix, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
                var syscallType  = monoPosix.GetType("Mono.Unix.Native.Syscall");
                var method       = syscallType.GetMethod("isatty");

                return (Boolean) method.Invoke(null, new Object[] { 0 })
                    ? ApplicationRunTypes.MonoConsole
                    : ApplicationRunTypes.MonoService;

            }

            else
                return Environment.UserInteractive
                    ? ApplicationRunTypes.WindowsConsole
                    : ApplicationRunTypes.WindowsService;

        }


    }

}
