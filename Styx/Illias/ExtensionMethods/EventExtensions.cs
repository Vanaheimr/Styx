/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Reflection;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions for events.
    /// </summary>
    public static class EventExtensions
    {

        #region RemoveAllEventHandlers(this Class, EventName)

        public static void RemoveAllEventHandlers(this Object  Class,
                                                  String       EventName)
        {

            var objType     = Class.GetType();
            var eventField  = objType.GetField(EventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                ?? throw new ArgumentException($"Event '{EventName}' not found on type '{objType.FullName}'.");

            eventField.SetValue(Class, null);

        }

        #endregion

    }

}
