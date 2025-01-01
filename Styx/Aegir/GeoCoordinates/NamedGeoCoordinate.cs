/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Aegir <https://www.github.com/Vanaheimr/Aegir>
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
using System.Globalization;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using System.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// A geographical coordinate with additional information.
    /// </summary>
    public class NamedGeoCoordinate : IEquatable<NamedGeoCoordinate>,
                                      IComparable<NamedGeoCoordinate>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The geographical coordinate.
        /// </summary>
        public GeoCoordinate               GeoCoordinate       { get; }

        /// <summary>
        /// An optional description of the geographical coordinate.
        /// </summary>
        public I18NString                  Description         { get; }

        /// <summary>
        /// Optional images attached to this geographical coordinate.
        /// </summary>
        public IEnumerable<String>         AttachedImages      { get; }

        /// <summary>
        /// Optional files attached to this geographical coordinate.
        /// </summary>
        public IEnumerable<String>         AttachedFiles       { get; }

        /// <summary>
        /// Optional custom data at this geographical coordinate.
        /// </summary>
        public Dictionary<String, Object>  CustomData          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geographical coordinate with additional information.
        /// </summary>
        /// <param name="GeoCoordinate">The geographical coordinate.</param>
        /// <param name="Description">An optional description of the geographical coordinate.</param>
        /// <param name="AttachedImages">Optional images attached to this geographical coordinate.</param>
        /// <param name="AttachedFiles">Optional files attached to this geographical coordinate.</param>
        /// <param name="CustomData">Optional custom data at this geographical coordinate.</param>
        public NamedGeoCoordinate(GeoCoordinate               GeoCoordinate,
                                  I18NString                  Description      = null,
                                  IEnumerable<String>         AttachedImages   = null,
                                  IEnumerable<String>         AttachedFiles    = null,
                                  Dictionary<String, Object>  CustomData       = null)
        {

            this.GeoCoordinate    = GeoCoordinate;
            this.Description      = Description;
            this.AttachedImages   = AttachedImages ?? new String[0];
            this.AttachedFiles    = AttachedFiles  ?? new String[0];
            this.CustomData       = CustomData;

        }

        #endregion


        #region ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NamedGeoCoordinate> CustomNamedGeoCoordinateSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("geoCoordinate",          GeoCoordinate.ToJSON()),

                           Description.IsNotNullOrEmpty()
                               ? new JProperty("description",      Description.  ToJSON())
                               : null,

                           AttachedImages.SafeAny()
                               ? new JProperty("attachedImages",   new JArray(AttachedImages))
                               : null,

                           AttachedImages.SafeAny()
                               ? new JProperty("attachedFiles",    new JArray(AttachedImages))
                               : null,

                           CustomData.SafeAny()
                               ? new JProperty("customData",       new JObject(CustomData.SafeSelect(data => new JProperty(data.Key, data.Value))))
                               : null

            );

            return CustomNamedGeoCoordinateSerializer is not null
                       ? CustomNamedGeoCoordinateSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out NamedGeoCoordinate, out ErrorResponse)

        /// <summary>
        /// Try to parse the given communicator group JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="NamedGeoCoordinate">The parsed named geo coordinate.</param>
        /// <param name="ErrorResponse">An error message.</param>
        public static Boolean TryParseJSON(JObject                 JSONObject,
                                           out NamedGeoCoordinate  NamedGeoCoordinate,
                                           out String              ErrorResponse)
        {

            try
            {

                NamedGeoCoordinate = null;

                //if (JSONObject?.HasValues != true)
                //{
                //    ErrorResponse = "The given JSON object must not be null or empty!";
                //    return false;
                //}

                //#region Parse NamedGeoCoordinateId   [optional]

                //// Verify that a given NamedGeoCoordinate identification
                ////   is at least valid.
                //if (JSONObject.ParseOptionalStruct("@id",
                //                                   "attached file identification",
                //                                   NamedGeoCoordinate_Id.TryParse,
                //                                   out NamedGeoCoordinate_Id? NamedGeoCoordinateIdBody,
                //                                   out ErrorResponse))
                //{

                //    if (ErrorResponse is not null)
                //        return false;

                //}

                //if (!NamedGeoCoordinateIdURL.HasValue && !NamedGeoCoordinateIdBody.HasValue)
                //{
                //    ErrorResponse = "The NamedGeoCoordinate identification is missing!";
                //    return false;
                //}

                //if (NamedGeoCoordinateIdURL.HasValue && NamedGeoCoordinateIdBody.HasValue && NamedGeoCoordinateIdURL.Value != NamedGeoCoordinateIdBody.Value)
                //{
                //    ErrorResponse = "The optional NamedGeoCoordinate identification given within the JSON body does not match the one given in the URI!";
                //    return false;
                //}

                //#endregion

                //#region Parse Context          [mandatory]

                //if (!JSONObject.ParseMandatory("@context",
                //                               "JSON-LD context",
                //                               JSONLDContext.TryParse,
                //                               out JSONLDContext Context,
                //                               out ErrorResponse))
                //{
                //    ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                //    return false;
                //}

                //if (Context != DefaultJSONLDContext)
                //{
                //    ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                //    return false;
                //}

                //#endregion

                //#region Parse Description      [optional]

                //if (JSONObject.ParseOptional("description",
                //                             "description",
                //                             out I18NString Description,
                //                             out ErrorResponse))
                //{

                //    if (ErrorResponse is not null)
                //        return false;

                //}

                //#endregion

                //var Locations     = new List<HTTPPath>();

                //var ContentType   = HTTPContentType.Application.JSONLD_UTF8;

                //var Size          = 0UL;

                //var Icon          = new HTTPPath?();

                //var Created       = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

                //var LastModified  = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

 
                //#region Get   DataSource       [optional]

                //var DataSource = JSONObject.GetOptional("dataSource");

                //#endregion


                //#region Parse CryptoHash       [optional]

                //var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                //#endregion


                //NamedGeoCoordinate = new NamedGeoCoordinate(Id:               NamedGeoCoordinateIdBody ?? NamedGeoCoordinateIdURL.Value,
                //                                Description:      Description,
                //                                Locations:        Locations,
                //                                ContentType:      ContentType,
                //                                Size:             Size,
                //                                Icon:             Icon,
                //                                Created:          Created,
                //                                LastModifed:      LastModified,
                //                                DataSource:       DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                NamedGeoCoordinate  = null;
                return false;
            }

        }

        #endregion



        #region Operator overloading

        #region Operator == (NamedGeoCoordinate1, NamedGeoCoordinate2)

        /// <summary>
        /// Compares two latitudes for equality.
        /// </summary>
        /// <param name="NamedGeoCoordinate1">A geographical coordinate with additional information.</param>
        /// <param name="NamedGeoCoordinate2">Another geographical coordinate with additional information.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NamedGeoCoordinate NamedGeoCoordinate1,
                                           NamedGeoCoordinate NamedGeoCoordinate2)
        {

            if (Object.ReferenceEquals(NamedGeoCoordinate1, NamedGeoCoordinate2))
                return true;

            if ((NamedGeoCoordinate1 is null) || (NamedGeoCoordinate2 is null))
                return false;

            return NamedGeoCoordinate1.Equals(NamedGeoCoordinate2);

        }

        #endregion

        #region Operator != (NamedGeoCoordinate1, NamedGeoCoordinate2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="NamedGeoCoordinate1">A geographical coordinate with additional information.</param>
        /// <param name="NamedGeoCoordinate2">Another geographical coordinate with additional information.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NamedGeoCoordinate NamedGeoCoordinate1,
                                           NamedGeoCoordinate NamedGeoCoordinate2)

            => !(NamedGeoCoordinate1 == NamedGeoCoordinate2);

        #endregion

        #region Operator <  (NamedGeoCoordinate1, NamedGeoCoordinate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NamedGeoCoordinate1">A geographical coordinate with additional information.</param>
        /// <param name="NamedGeoCoordinate2">Another geographical coordinate with additional information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (NamedGeoCoordinate NamedGeoCoordinate1,
                                           NamedGeoCoordinate NamedGeoCoordinate2)

            => NamedGeoCoordinate1 is null
                   ? throw new ArgumentNullException(nameof(NamedGeoCoordinate1), "The given geographical coordinate with additional information must not be null!")
                   : NamedGeoCoordinate1.CompareTo(NamedGeoCoordinate2) < 0;

        #endregion

        #region Operator <= (NamedGeoCoordinate1, NamedGeoCoordinate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NamedGeoCoordinate1">A geographical coordinate with additional information.</param>
        /// <param name="NamedGeoCoordinate2">Another geographical coordinate with additional information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NamedGeoCoordinate NamedGeoCoordinate1,
                                           NamedGeoCoordinate NamedGeoCoordinate2)

            => !(NamedGeoCoordinate1 > NamedGeoCoordinate2);

        #endregion

        #region Operator >  (NamedGeoCoordinate1, NamedGeoCoordinate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NamedGeoCoordinate1">A geographical coordinate with additional information.</param>
        /// <param name="NamedGeoCoordinate2">Another geographical coordinate with additional information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (NamedGeoCoordinate NamedGeoCoordinate1, NamedGeoCoordinate NamedGeoCoordinate2)

            => NamedGeoCoordinate1 is null
                   ? throw new ArgumentNullException(nameof(NamedGeoCoordinate1), "The given geographical coordinate with additional information must not be null!")
                   : NamedGeoCoordinate1.CompareTo(NamedGeoCoordinate2) > 0;

        #endregion

        #region Operator >= (NamedGeoCoordinate1, NamedGeoCoordinate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NamedGeoCoordinate1">A geographical coordinate with additional information.</param>
        /// <param name="NamedGeoCoordinate2">Another geographical coordinate with additional information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NamedGeoCoordinate NamedGeoCoordinate1,
                                           NamedGeoCoordinate NamedGeoCoordinate2)

            => !(NamedGeoCoordinate1 < NamedGeoCoordinate2);

        #endregion

        #endregion

        #region IComparable Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is NamedGeoCoordinate namedGeoCoordinate
                   ? CompareTo(namedGeoCoordinate)
                   : throw new ArgumentException("The given object is not a geographical coordinate with additional information!", nameof(Object));

        #endregion

        #region CompareTo(NamedGeoCoordinate)

        /// <summary>
        /// Compares two latitudes.
        /// </summary>
        /// <param name="NamedGeoCoordinate">Another geographical coordinate with additional information.</param>
        public Int32 CompareTo(NamedGeoCoordinate NamedGeoCoordinate)

            => NamedGeoCoordinate is null
                   ? throw new ArgumentNullException(nameof(Object), "The given geographical coordinate with additional information must not be null!")
                   : GeoCoordinate.CompareTo(NamedGeoCoordinate.GeoCoordinate);

        #endregion

        #endregion

        #region IEquatable Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(Object Object)

            => Object is NamedGeoCoordinate namedGeoCoordinate &&
                   Equals(namedGeoCoordinate);

        #endregion

        #region Equals(NamedGeoCoordinate)

        /// <summary>
        /// Compares two latitudes for equality.
        /// </summary>
        /// <param name="NamedGeoCoordinate">Another geographical coordinate with additional information.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(NamedGeoCoordinate NamedGeoCoordinate)

            => !(NamedGeoCoordinate is null) &&

               GeoCoordinate.Equals(NamedGeoCoordinate.GeoCoordinate) &&
               Description.  Equals(NamedGeoCoordinate.Description)   &&

               AttachedImages.Count().Equals(NamedGeoCoordinate.AttachedImages.Count()) &&
               AttachedFiles. Count().Equals(NamedGeoCoordinate.AttachedFiles. Count()) &&

               AttachedImages.All(attachedImage => NamedGeoCoordinate.AttachedImages.Contains(attachedImage)) &&
               AttachedFiles. All(attachedFile  => NamedGeoCoordinate.AttachedFiles. Contains(attachedFile));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()

            => GeoCoordinate.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        public override String ToString()

            => GeoCoordinate.ToString() + Description.ToString();

        #endregion

    }

}
