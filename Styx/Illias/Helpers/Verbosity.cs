/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

#region Usings

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class VerbosityExtentions
    {

        public static Verbosity Parse(String Text)
        {

            switch (Text)
            {

                case "quiet":
                    return Verbosity.quiet;

                case "simple":
                    return Verbosity.simple;

                case "brief":
                    return Verbosity.brief;

                case "verbose":
                    return Verbosity.verbose;

                case "veryVerbose":
                    return Verbosity.veryVerbose;

                case "debug":
                    return Verbosity.debug;

                default:
                    return Verbosity.normal;

            }

        }

    }



    /// <summary>
    /// The verbosity.
    /// </summary>
    public enum Verbosity
    {

        quiet,

        simple,

        brief,

        normal,

        verbose,

        veryVerbose,

        debug

    }

}
