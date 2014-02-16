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

#endregion

namespace eu.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// A delegate called whenever the data processor has something to say.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the message.</param>
    /// <param name="LogLevel">The log level of the message.</param>
    /// <param name="Arguments">The message arguments.</param>
    public delegate void LoggerDelegate(DateTime Timestamp, LogLevel LogLevel, params Object[] Arguments);

}
