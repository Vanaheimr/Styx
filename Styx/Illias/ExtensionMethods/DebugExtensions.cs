/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

using System.Diagnostics;
using System.Runtime.CompilerServices;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Helpers for the normal Debug class.
    /// </summary>
    public static class DebugX
    {

        #region Log(params Text)

        /// <summary>
        /// Write the current timestamp and given text to Debug.
        /// </summary>
        /// <param name="Text">The text to be logged.</param>
        public static void Log(params String[] Text)
        {
            if (Text.IsNeitherNullNorEmpty())
                Debug.WriteLine(String.Concat("[", Timestamp.Now, "] ", String.Concat(Text)));
        }

        #endregion

        #region LogT(params Text)

        /// <summary>
        /// Write the current timestamp and given text to Debug.
        /// </summary>
        /// <param name="Text">The text to be logged.</param>
        public static void LogT(params String[] Text)
        {
            if (Text.IsNeitherNullNorEmpty())
                Debug.WriteLine(String.Concat("[", Timestamp.Now, " T:", Thread.CurrentThread.ManagedThreadId, "] ", String.Concat(Text)));
        }

        #endregion

        #region Log(Message, Exception)

        /// <summary>
        /// Write the current timestamp and given exception to Debug.
        /// </summary>
        /// <param name="Message">An exception message..</param>
        /// <param name="Exception">The exception.</param>
        public static void Log(String Message, Exception Exception)
        {
            if (Exception is not null)
                Debug.WriteLine(String.Concat("[", Timestamp.Now, "] ", Message ?? "", Environment.NewLine, Exception.Message, Environment.NewLine,
                                              Exception.StackTrace, Environment.NewLine));
        }

        #endregion

        #region Log(Exception, Source)

        /// <summary>
        /// Write the current timestamp and given exception to Debug.
        /// </summary>
        /// <param name="Exception">The exception.</param>
        /// <param name="Source">The source of the exception.</param>
        public static void Log(Exception Exception, String Source)
        {
            if (Exception is not null)
                Debug.WriteLine(String.Concat("[", Timestamp.Now, "] ", Source ?? "", " led to an exception: ", Exception.Message, Environment.NewLine,
                                              Exception.StackTrace, Environment.NewLine));
        }

        #endregion

        #region LogException(Exception, Source)

        /// <summary>
        /// Write the current timestamp and given exception to Debug.
        /// </summary>
        /// <param name="Exception">The exception.</param>
        /// <param name="Source">The source of the exception.</param>
        public static void LogException(Exception Exception, [CallerMemberName] String Source = "")
        {
            if (Exception is not null)
                Debug.WriteLine(String.Concat("[", Timestamp.Now, "] ", Source ?? "", " led to an exception: ", Environment.NewLine,
                                Exception.Message,    Environment.NewLine,
                                Exception.StackTrace, Environment.NewLine));
        }

        #endregion

    }

}
