/*
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

namespace org.GraphDefined.Vanaheimr.CLI
{

    /// <summary>
    /// A CLI suggestion response.
    /// </summary>
    public class SuggestionResponse : IEquatable<SuggestionResponse>,
                                      IComparable<SuggestionResponse>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The suggested string.
        /// </summary>
        public String          Suggestion    { get; set; } = "";

        /// <summary>
        /// Suggestions infos.
        /// </summary>
        public SuggestionInfo  Info          { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CLI suggestion response.
        /// </summary>
        /// <param name="Suggestion">The suggested string.</param>
        /// <param name="Info">Suggestions infos.</param>
        private SuggestionResponse(String          Suggestion,
                                   SuggestionInfo  Info)
        {
            this.Suggestion  = Suggestion;
            this.Info        = Info;
        }

        #endregion


        #region (static) CommandPrefix      (Suggestion)

        public static SuggestionResponse CommandPrefix(String Suggestion)

            => new (Suggestion,
                    SuggestionInfo.CommandPrefix);

        #endregion

        #region (static) ParameterPrefix    (Suggestion)

        public static SuggestionResponse ParameterPrefix(String Suggestion)

            => new (Suggestion,
                    SuggestionInfo.ParameterPrefix);

        #endregion


        #region (static) ParameterCompleted (Suggestion)
        public static SuggestionResponse ParameterCompleted(String Suggestion)

            => new (Suggestion,
                    SuggestionInfo.ParameterCompleted);

        #endregion

        #region (static) CommandCompleted   (Suggestion)
        public static SuggestionResponse CommandCompleted(String Suggestion)

            => new (Suggestion,
                    SuggestionInfo.CommandCompleted);

        #endregion

        #region (static) CommandHelp        (Suggestion)
        public static SuggestionResponse CommandHelp(String Suggestion)

            => new (Suggestion,
                    SuggestionInfo.CommandHelp);

        #endregion


        #region Operator overloading

        #region Operator == (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SuggestionResponse SuggestionResponse1,
                                           SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.Equals(SuggestionResponse2);

        #endregion

        #region Operator != (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SuggestionResponse SuggestionResponse1,
                                           SuggestionResponse SuggestionResponse2)

            => !SuggestionResponse1.Equals(SuggestionResponse2);

        #endregion

        #region Operator <  (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SuggestionResponse SuggestionResponse1,
                                          SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.CompareTo(SuggestionResponse2) < 0;

        #endregion

        #region Operator <= (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SuggestionResponse SuggestionResponse1,
                                           SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.CompareTo(SuggestionResponse2) <= 0;

        #endregion

        #region Operator >  (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SuggestionResponse SuggestionResponse1,
                                          SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.CompareTo(SuggestionResponse2) > 0;

        #endregion

        #region Operator >= (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SuggestionResponse SuggestionResponse1,
                                           SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.CompareTo(SuggestionResponse2) >= 0;

        #endregion

        #endregion

        #region IComparable<SuggestionResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two SuggestionResponses.
        /// </summary>
        /// <param name="Object">A SuggestionResponse to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SuggestionResponse ipSocket
                   ? CompareTo(ipSocket)
                   : throw new ArgumentException("The given object is not a SuggestionResponse!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SuggestionResponse)

        /// <summary>
        /// Compares two SuggestionResponses.
        /// </summary>
        /// <param name="SuggestionResponse">A SuggestionResponse to compare with.</param>
        public Int32 CompareTo(SuggestionResponse? SuggestionResponse)
        {
            return Suggestion.CompareTo(SuggestionResponse?.Suggestion ?? "");
        }

        #endregion

        #endregion

        #region IEquatable<SuggestionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SuggestionResponses for equality.
        /// </summary>
        /// <param name="Object">A SuggestionResponse to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SuggestionResponse component &&
                   Equals(component);

        #endregion

        #region Equals(SuggestionResponse)

        /// <summary>
        /// Compares two SuggestionResponses for equality.
        /// </summary>
        /// <param name="SuggestionResponse">A SuggestionResponse to compare with.</param>
        public Boolean Equals(SuggestionResponse? SuggestionResponse)

            => SuggestionResponse is not null &&
               String.Equals(Suggestion, SuggestionResponse.Suggestion, StringComparison.OrdinalIgnoreCase) &&
               Info.  Equals(SuggestionResponse.Info);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Suggestion.GetHashCode() ^
                       Info.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Suggestion} [{Info}]";

        #endregion

    }

}
