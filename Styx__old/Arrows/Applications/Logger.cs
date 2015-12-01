/*
 * Copyright (c) 2011-2013, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of Styx <http://www.github.com/Vanaheimr/Styx>
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
using System.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    public class Logger
    {

        public event LoggerDelegate OnOutput;

        #region Log(LogLevel, params Arguments)

        public void Log(LogLevel LogLevel, params Object[] Arguments)
        {
            Log(DateTime.Now, LogLevel, Arguments);
        }

        #endregion

        #region Log(Timestamp, LogLevel, params Arguments)

        public void Log(DateTime Timestamp, LogLevel LogLevel, params Object[] Arguments)
        {

            var OnOutputLocal = OnOutput;
            if (OnOutputLocal != null)
                OnOutputLocal(Timestamp, LogLevel, Arguments);

        }

        #endregion

    }

    public class Logger<TEnum> : ILogger<TEnum>
        where TEnum : struct, IComparable, IFormattable, IConvertible
    {

        public event LoggerDelegate<TEnum> OnOutput;

        #region Log(LogLevel, params Arguments)

        public void Log(TEnum Topic, params Object[] Arguments)
        {
            Log(DateTime.Now, Topic, Arguments);
        }

        #endregion

        #region Log(Timestamp, Topic, params Arguments)

        public void Log(DateTime Timestamp, TEnum Topic, params Object[] Arguments)
        {

            var OnOutputLocal = OnOutput;
            if (OnOutputLocal != null)
                OnOutputLocal(Timestamp, Topic, Arguments);

        }

        #endregion

    }

}
