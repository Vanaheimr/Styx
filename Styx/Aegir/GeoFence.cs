/*
 * Copyright (c) 2010-2018, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// A geo fenche.
    /// </summary>
    public class GeoFence
    {

        #region Properties

        /// <summary>
        /// An enumeration of geo coordinates.
        /// </summary>
        public IEnumerable<GeoCoordinate>  GeoCoordinates    { get; }

        /// <summary>
        /// An optional geographical distance.
        /// </summary>
        public Meter?                      Distance          { get; }

        /// <summary>
        /// An optional description.
        /// </summary>
        public I18NString                  Description       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geo fence.
        /// </summary>
        /// <param name="GeoCoordinates">An enumeration of geo coordinates.</param>
        /// <param name="Distance">An optional geographical distance.</param>
        /// <param name="Description">An optional description.</param>
        public GeoFence(IEnumerable<GeoCoordinate>  GeoCoordinates,
                        Meter?                      Distance,
                        I18NString                  Description)
        {

            this.GeoCoordinates  = GeoCoordinates;
            this.Distance        = Distance;
            this.Description     = Description ?? new I18NString();

        }

        #endregion


        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return GeoCoordinates.GetHashCode() * 5 ^

                       (Distance.HasValue
                            ? Distance.Value.GetHashCode()
                            : 0) * 3 ^

                       (Description.IsNullOrEmpty()
                            ? Description.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (GeoCoordinates.Count() == 1 && Distance.HasValue)
                return String.Concat(GeoCoordinates.First(),
                                     " with radius ",
                                     Distance.Value,
                                     "km",
                                     Description.IsNullOrEmpty()
                                         ? "(" + Description.FirstText() + ")"
                                         : "");

            if (GeoCoordinates.Count() > 1)
                return GeoCoordinates.
                           Select(coordinate => coordinate.ToString()).
                           AggregateWith(", ");

            return Description.IsNullOrEmpty()
                       ? Description.FirstText()
                       : nameof(GeoFence);

        }

        #endregion

        //#region ToJSON(IncludeHash = true)

        ///// <summary>
        ///// Return a JSON representation of this object.
        ///// </summary>
        ///// <param name="IncludeHash">Include the hash value of this object.</param>
        //public override JObject ToJSON(Boolean IncludeHash = true)

        //    => JSONObject.Create(

        //           new JProperty("@id",          Id.         ToString()),
        //           new JProperty("name",         Name.       ToJSON()),
        //           new JProperty("description",  Description.ToJSON()),
        //           new JProperty("isPublic",     IsPublic),
        //           new JProperty("isDisabled",   IsDisabled),

        //           IncludeHash
        //               ? new JProperty("Hash",   CurrentCryptoHash)
        //               : null

        //       );

        //#endregion

    }

}
