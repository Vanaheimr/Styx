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

using System.Collections.Immutable;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Immutable opening times, the immutable cousin of the OpeningTimes.
    /// </summary>
    public sealed class ImmutableOpeningTimes
    {

        #region Data

        /// <summary>
        /// The regular openings in their original order, as written to JSON.
        /// </summary>
        private readonly ImmutableArray<KeyValuePair<DayOfWeek, ImmutableArray<RegularHours>>> regularOpenings;

        #endregion

        #region Properties

        /// <summary>
        /// The regular openings per weekday.
        /// </summary>
        public ImmutableDictionary<DayOfWeek, ImmutableArray<RegularHours>>  RegularOpenings        { get; }

        /// <summary>
        /// The exceptional openings.
        /// </summary>
        public ImmutableArray<ExceptionalPeriod>                             ExceptionalOpenings    { get; }

        /// <summary>
        /// The exceptional closings.
        /// </summary>
        public ImmutableArray<ExceptionalPeriod>                             ExceptionalClosings    { get; }

        /// <summary>
        /// An optional free text.
        /// </summary>
        public String?                                                       FreeText               { get; }

        /// <summary>
        /// Whether it is open 24 hours a day, 7 days a week.
        /// </summary>
        public Boolean                                                       IsOpen24Hours
            => regularOpenings.IsEmpty;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an immutable copy of the given opening times.
        /// </summary>
        /// <param name="OpeningTimes">Opening times.</param>
        public ImmutableOpeningTimes(OpeningTimes OpeningTimes)
        {

            ArgumentNullException.ThrowIfNull(OpeningTimes);

            this.regularOpenings      = [.. OpeningTimes.RegularOpenings.Select(regularOpening => KeyValuePair.Create(regularOpening.Key,
                                                                                                                      regularOpening.Value.ToImmutableArray()))];
            this.RegularOpenings      = regularOpenings.ToImmutableDictionary();
            this.ExceptionalOpenings  = [.. OpeningTimes.ExceptionalOpenings];
            this.ExceptionalClosings  = [.. OpeningTimes.ExceptionalClosings];
            this.FreeText             = OpeningTimes.FreeText;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of these opening times.
        /// </summary>
        public JObject ToJSON()
            => ToMutable().ToJSON();

        #endregion

        #region ToMutable()

        /// <summary>
        /// Return a mutable copy of these opening times.
        /// </summary>
        public OpeningTimes ToMutable()

            => new (regularOpenings.Select(regularOpening => KeyValuePair.Create(regularOpening.Key,
                                                                                 (IEnumerable<RegularHours>) regularOpening.Value)),
                    ExceptionalOpenings,
                    ExceptionalClosings,
                    FreeText);

        #endregion

        #region AsFreeText()

        /// <summary>
        /// Return a free-text representation of these opening times.
        /// </summary>
        public String AsFreeText()
            => ToMutable().AsFreeText();

        #endregion


        #region Operator overloading

        #region Implicit conversion from OpeningTimes

        /// <summary>
        /// Create an immutable copy of the given opening times.
        /// </summary>
        /// <param name="OpeningTimes">Opening times.</param>
        public static implicit operator ImmutableOpeningTimes(OpeningTimes OpeningTimes)
            => new (OpeningTimes);

        #endregion

        #region Implicit conversion to OpeningTimes

        /// <summary>
        /// Create a mutable copy of the given opening times.
        /// </summary>
        /// <param name="ImmutableOpeningTimes">Immutable opening times.</param>
        public static implicit operator OpeningTimes(ImmutableOpeningTimes ImmutableOpeningTimes)
            => ImmutableOpeningTimes.ToMutable();

        #endregion

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => AsFreeText();

        #endregion

    }

}
