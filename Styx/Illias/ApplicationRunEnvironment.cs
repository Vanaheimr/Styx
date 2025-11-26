/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Runtime.InteropServices;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public readonly struct RunEnvironment
    {

        public String   Platform       { get; }
        public Boolean  IsWindows      { get; }
        public Boolean  IsLinux        { get; }
        public Boolean  IsMacOS        { get; }
        public Boolean  IsFreeBSD      { get; }

        public Boolean  IsConsole      { get; }
        public Boolean  IsService          => !IsConsole;

        public Boolean  IsWindowsConsole   =>  IsConsole && IsWindows;
        public Boolean  IsLinuxConsole     =>  IsConsole && IsLinux;
        public Boolean  IsMacOSConsole     =>  IsConsole && IsMacOS;
        public Boolean  IsFreeBSDConsole   =>  IsConsole && IsFreeBSD;


        public Boolean  IsWindowsService   => !IsConsole && IsWindows;
        public Boolean  IsLinuxService     => !IsConsole && IsLinux;
        public Boolean  IsMacOSService     => !IsConsole && IsMacOS;
        public Boolean  IsFreeBSDService   => !IsConsole && IsFreeBSD;

        public String   Description    { get; }


        public RunEnvironment()
        {

            this.IsWindows    = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            this.IsLinux      = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            this.IsMacOS      = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            this.IsFreeBSD    = RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);

            this.Platform     = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
                                RuntimeInformation.IsOSPlatform(OSPlatform.Linux)   ? "Linux"   :
                                RuntimeInformation.IsOSPlatform(OSPlatform.OSX)     ? "MacOS"  :
                                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) ? "FreeBSD" :
                                                                                      "Unknown";

            this.IsConsole    = Environment.UserInteractive;

            this.Description  = $"{Platform}{(IsConsole ? "Console" : "Service")}";

        }

        public override String ToString()
            => Description;

    }

    public static class Application
    {
        public static readonly RunEnvironment RunEnvironment = new();
    }

}
