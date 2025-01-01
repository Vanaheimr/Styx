﻿/*
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

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Represents the method that defines a set of criteria and determines whether
    /// the specified object meets those criteria.
    /// </summary>
    /// <typeparam name="T">The type of the object to compare.</typeparam>
    /// <param name="Object">The object to compare.</param>
    /// <param name="Counter">An object counter. Starts with 1!</param>
    public delegate Boolean CountedPredicate<in T>(T Object, UInt64 Counter);

}
