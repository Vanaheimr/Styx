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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for percentages of values.
    /// </summary>
    public static class PercentageOfExtensions
    {

        #region Parse    (JSON, PropertyKey, NameOfItems, TryParser)

        /// <summary>
        /// Parse the mandatory array of [value, percentage] pairs in the given property,
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="JSON">The JSON object containing the array.</param>
        /// <param name="PropertyKey">The property key of the array.</param>
        /// <param name="NameOfItems">The name of the values, e.g. "energy source".</param>
        /// <param name="TryParser">The delegate to parse a value.</param>
        public static IEnumerable<PercentageOf<TValue>> Parse<TValue>(JObject            JSON,
                                                                      String             PropertyKey,
                                                                      String             NameOfItems,
                                                                      TryParser<TValue>  TryParser)

            where TValue : notnull, IEquatable<TValue>

        {

            if (TryParse(JSON,
                         PropertyKey,
                         NameOfItems,
                         TryParser,
                         out var percentageOfs,
                         out var errorResponse))
            {
                return percentageOfs;
            }

            throw new ArgumentException("The given JSON representation of an array of percentages of values is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region TryParse (JSON, PropertyKey, NameOfItems, TryParser, out PercentageOfs, out ErrorResponse)

        /// <summary>
        /// Try to parse the mandatory array of [value, percentage] pairs in the given property,
        /// e.g. "energySources": [["solar", "60 %"], ["wind", "40 %"]]. A value must not be given twice.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="JSON">The JSON object containing the array.</param>
        /// <param name="PropertyKey">The property key of the array.</param>
        /// <param name="NameOfItems">The name of the values, e.g. "energy source".</param>
        /// <param name="TryParser">The delegate to parse a value.</param>
        /// <param name="PercentageOfs">The parsed percentages of values, in their order.</param>
        /// <param name="ErrorResponse">An error response naming the path of the invalid element.</param>
        public static Boolean TryParse<TValue>(JObject                                                     JSON,
                                               String                                                      PropertyKey,
                                               String                                                      NameOfItems,
                                               TryParser<TValue>                                           TryParser,
                                               [NotNullWhen(true)]  out IEnumerable<PercentageOf<TValue>>  PercentageOfs,
                                               [NotNullWhen(false)] out String?                            ErrorResponse)

            where TValue : notnull, IEquatable<TValue>

        {

            PercentageOfs = [];

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (JSON[PropertyKey] is not JArray jsonArray)
            {
                ErrorResponse = $"The '{PropertyKey}' property must be an array!";
                return false;
            }

            var percentageOfs = new List<PercentageOf<TValue>>();

            for (var index = 0; index < jsonArray.Count; index++)
            {

                if (jsonArray[index] is not JArray valuePercentageArray)
                {
                    ErrorResponse = $"{PropertyKey}[{index}]: expected an array of the {NameOfItems} and its percentage!";
                    return false;
                }

                if (!PercentageOf<TValue>.TryParse(valuePercentageArray,
                                                   TryParser,
                                                   out var percentageOf,
                                                   out var errorResponse))
                {
                    ErrorResponse = $"{PropertyKey}[{index}]{(errorResponse.StartsWith('[') ? "" : ": ")}{errorResponse}";
                    return false;
                }

                if (percentageOfs.Any(existing => existing.Value.Equals(percentageOf.Value)))
                {
                    ErrorResponse = $"{PropertyKey}[{index}][0]: the {NameOfItems} '{percentageOf.Value}' must not be given twice!";
                    return false;
                }

                percentageOfs.Add(percentageOf);

            }

            PercentageOfs  = percentageOfs;
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ToJSON   (PercentageOfs)

        /// <summary>
        /// Return a JSON representation of the given enumeration of percentages of values.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="PercentageOfs">An enumeration of percentages of values.</param>
        public static JArray ToJSON<TValue>(this IEnumerable<PercentageOf<TValue>> PercentageOfs)

            where TValue : notnull, IEquatable<TValue>

                => new (PercentageOfs.Select(percentageOf => percentageOf.ToJSON()));

        #endregion

    }


    /// <summary>
    /// A percentage of a value.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <param name="Percent">The percentage.</param>
    public readonly struct PercentageOf<TValue>(TValue  Value,
                                                Single  Percent)

        : IEquatable<PercentageOf<TValue>>

        where TValue : notnull, IEquatable<TValue>

    {

        #region Properties

        /// <summary>
        /// The value.
        /// </summary>
        public TValue  Value      { get; } = Value;

        /// <summary>
        /// The percentage.
        /// </summary>
        public Single  Percent    { get; } = Percent;

        #endregion


        #region Parse    (JSON, Parser)

        /// <summary>
        /// Parse the given JSON representation of a percentage of a value.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Parser">The delegate to parse the value.</param>
        public static PercentageOf<TValue> Parse(JArray             JSON,
                                                 TryParser<TValue>  Parser)
        {

            if (TryParse(JSON,
                         Parser,
                         out var percentageOf,
                         out var errorResponse))
            {
                return percentageOf;
            }

            throw new ArgumentException("The given JSON representation of a percentage of a value is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region TryParse (JSON, Parser, out PercentageOf, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a percentage of a value.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Parser">The delegate to parse the value.</param>
        /// <param name="PercentageOf">The percentage of the value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JArray                            JSON,
                                       TryParser<TValue>                 Parser,
                                       out PercentageOf<TValue>          PercentageOf,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            PercentageOf   = default;
            ErrorResponse  = null;

            try
            {

                if (JSON.Count != 2)
                {
                    ErrorResponse = "Expected an array of exactly two elements: the value and the percentage, e.g. [\"solar\", \"42.5 %\"]!";
                    return false;
                }

                if (JSON[0].Type != JTokenType.String ||
                    !Parser(JSON[0].Value<String>()!, out var value))
                {
                    ErrorResponse = $"[0]: expected a valid {typeof(TValue).Name} string!";
                    return false;
                }

                var percentText = JSON[1].Type == JTokenType.String
                                      ? JSON[1].Value<String>()!.Trim()
                                      : null;

                if (percentText is null || !percentText.EndsWith('%') ||
                    !Single.TryParse(percentText[..^1].TrimEnd(), NumberStyles.Float, CultureInfo.InvariantCulture, out var percent))
                {
                    ErrorResponse = "[1]: expected a percentage string with '%' unit, e.g. \"42.5 %\"!";
                    return false;
                }

                if (!Single.IsFinite(percent) || percent < 0 || percent > 100)
                {
                    ErrorResponse = "[1]: expected a percentage between 0 and 100!";
                    return false;
                }

                PercentageOf = new PercentageOf<TValue>(
                                   value,
                                   percent
                               );

                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given JSON representation of a percentage of a value is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region PercentText

        /// <summary>
        /// The percentage with its unit, e.g. "42.5 %": culture invariant and
        /// round-trippable, so that parsing it yields exactly this percentage again.
        /// </summary>
        public String PercentText

            => Percent.ToString("R", CultureInfo.InvariantCulture) + " %";

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JArray ToJSON()

            => new (
                   Value.ToString() ?? String.Empty,
                   PercentText
               );

        #endregion


        #region Deconstruct(out Value, out Percent)

        public void Deconstruct(out TValue Value, out Single Percent)
        {
            Value    = this.Value;
            Percent  = this.Percent;
        }

        #endregion


        #region Operator overloading

        #region Operator == (Left, Right)

        /// <summary>
        /// Compares two percentages for equality.
        /// </summary>
        /// <param name="Left">A percentage.</param>
        /// <param name="Right">Another percentage.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PercentageOf<TValue> Left,
                                           PercentageOf<TValue> Right)

            => Left.Equals(Right);

        #endregion

        #region Operator != (Left, Right)

        /// <summary>
        /// Compares two percentages for inequality.
        /// </summary>
        /// <param name="Left">A percentage.</param>
        /// <param name="Right">Another percentage.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PercentageOf<TValue> Left,
                                           PercentageOf<TValue> Right)

            => !Left.Equals(Right);

        #endregion

        #endregion

        #region IEquatable<PercentageOf<TValue>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two percentages for equality.
        /// </summary>
        /// <param name="Object">A percentage to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PercentageOf<TValue> percentage &&
               Equals(percentage);

        #endregion

        #region Equals(PercentageOf)

        /// <summary>
        /// Compares two percentages for equality.
        /// </summary>
        /// <param name="PercentageOf">A percentage to compare with.</param>
        public Boolean Equals(PercentageOf<TValue> PercentageOf)

            => Value.  Equals(PercentageOf.Value) &&
               Percent.Equals(PercentageOf.Percent);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Value.  GetHashCode() * 3 ^
                       Percent.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Value}: {PercentText}";

        #endregion

    }

}
