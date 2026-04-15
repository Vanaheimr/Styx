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

using System.Numerics;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The common interface of all metrology types.
    /// </summary>
    /// <typeparam name="T">The type of the metrological value.</typeparam>
    public interface IMetrology<T> : IParsable    <T>,
                                     ISpanParsable<T>,
                                     IEquatable   <T>,
                                     IComparable  <T>,
                                     IComparable,
                                     IFormattable,
                                     ISpanFormattable,
                                     IAdditionOperators   <T,  T,       T>,
                                     ISubtractionOperators<T,  T,       T>,
                                     IMultiplyOperators   <T,  Decimal, T>,
                                     IDivisionOperators   <T,  Decimal, T>,
                                     IComparisonOperators <T,  T,       Boolean>,
                                     IEqualityOperators   <T,  T,       Boolean>,
                                     IAdditiveIdentity    <T,  T>

        where T : IMetrology<T>

    {

    }

}
