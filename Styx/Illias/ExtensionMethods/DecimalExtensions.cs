/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

using System.Globalization;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Decimals from doubles, and from JSON numbers read as doubles, as they are written.
    ///
    /// Since .NET 11 a cast or a Convert.ToDecimal of a double is exact: 11.9 becomes
    /// 11.900000000000000355271367880, the value the double really holds, where .NET 10
    /// rounded to 15 significant digits and gave 11.9. Newtonsoft's Value&lt;Decimal&gt;()
    /// and (Decimal) JToken convert a JSON number that was read as a double in just that
    /// way, so an "inclTax": 11.9 would be read, and written again, as
    /// 11.900000000000000355271367880. These conversions go through the shortest text that
    /// reads back as the same double instead - "11.9" - on every runtime.
    /// </summary>
    public static class DecimalExtensions
    {

        #region AsDecimal(this Value)

        /// <summary>
        /// The decimal the given double is written as: the shortest text that reads back
        /// as the same double, read as a decimal - 11.9, not 11.900000000000000355271367880.
        /// </summary>
        /// <param name="Value">A double.</param>
        /// <exception cref="OverflowException">For NaN, an infinity, or a double beyond the range of a decimal, as for a cast.</exception>
        public static Decimal AsDecimal(this Double Value)

            => Double.IsFinite(Value)
                   ? Decimal.Parse(Value.ToString("R", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture)
                   : throw new OverflowException($"{Value.ToString(CultureInfo.InvariantCulture)} is no decimal!");


        /// <summary>
        /// The decimal the given single is written as: the shortest text that reads back
        /// as the same single, read as a decimal - 0.1, not 0.100000001490116119384765625.
        /// </summary>
        /// <param name="Value">A single.</param>
        /// <exception cref="OverflowException">For NaN, an infinity, or a single beyond the range of a decimal, as for a cast.</exception>
        public static Decimal AsDecimal(this Single Value)

            => Single.IsFinite(Value)
                   ? Decimal.Parse(Value.ToString("R", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture)
                   : throw new OverflowException($"{Value.ToString(CultureInfo.InvariantCulture)} is no decimal!");

        #endregion

        #region AsDecimal      (this Token)

        /// <summary>
        /// The decimal the given JSON number is written as. A number Newtonsoft read as a
        /// double, or as a single, goes through the shortest text of that number; any other
        /// token - a decimal, an integer, a string - is converted as Value&lt;Decimal&gt;()
        /// converts it.
        /// </summary>
        /// <param name="Token">A JSON number, or a string holding one.</param>
        public static Decimal AsDecimal(this JToken Token)

            => Token switch {
                   JValue { Value: Double doubleValue } => doubleValue.AsDecimal(),
                   JValue { Value: Single singleValue } => singleValue.AsDecimal(),
                   _                                    => Token.Value<Decimal>()
               };

        #endregion

        #region AsDecimalOrNull(this Token)

        /// <summary>
        /// The decimal the given JSON number is written as, or null for a missing token
        /// or a JSON null - as Value&lt;Decimal?&gt;() gives, but for a number read as a
        /// double the decimal it is written as.
        /// </summary>
        /// <param name="Token">A JSON number, a string holding one, a JSON null, or null.</param>
        public static Decimal? AsDecimalOrNull(this JToken? Token)

            => Token is null || Token.Type is JTokenType.Null or JTokenType.Undefined
                   ? null
                   : Token.AsDecimal();

        #endregion

    }

}
