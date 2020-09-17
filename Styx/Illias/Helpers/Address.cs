/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// JSON I/O.
    /// </summary>
    public static class AddressExtentions
    {

        #region ToJSON(this Address, JPropertyKey, Embedded  = false)

        public static JProperty ToJSON(this Address  Address,
                                       String        JPropertyKey,
                                       Boolean       Embedded  = false)

            => Address != null
                   ? new JProperty(JPropertyKey,
                                   Address.ToJSON(Embedded))
                   : null;

        #endregion

        #region ToJSON(this Addresses, JPropertyKey)

        public static JArray ToJSON(this IEnumerable<Address> Addresses)

            => Addresses?.Any() == true
                   ? new JArray(Addresses.SafeSelect(addr => addr.ToJSON()))
                   : null;

        #endregion

        #region ToJSON(this Addresses, JPropertyKey)

        public static JProperty ToJSON(this IEnumerable<Address> Addresses, String JPropertyKey)

            => Addresses != null
                   ? new JProperty(JPropertyKey,
                                   Addresses.ToJSON())
                   : null;

        #endregion

        //public static Boolean TryParseAddress(this String Text, out Address Address)
        //    => Address.TryParseJSON(JObject.Parse(Text), out Address);

    }

    /// <summary>
    /// A WWCP address.
    /// </summary>
    public class Address : ACustomData,
                           IEquatable<Address>,
                           IComparable<Address>,
                           IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/UsersAPI+json/address";

        #endregion

        #region Properties

        /// <summary>
        /// The name of the street.
        /// </summary>
        public String      Street           { get; }

        /// <summary>
        /// The house number.
        /// </summary>
        public String      HouseNumber      { get; }

        /// <summary>
        /// The floor level.
        /// </summary>
        public String      FloorLevel       { get; }

        /// <summary>
        /// The postal code.
        /// </summary>
        public String      PostalCode       { get; }

        /// <summary>
        /// The postal code sub.
        /// </summary>
        public String      PostalCodeSub    { get; }

        /// <summary>
        /// The city.
        /// </summary>
        public I18NString  City             { get; }

        /// <summary>
        /// The country.
        /// </summary>
        public Country     Country          { get; }

        /// <summary>
        /// An optional text/comment to describe the address.
        /// </summary>
        public I18NString  Comment          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new address.
        /// </summary>
        /// <param name="Street">The name of the street.</param>
        /// <param name="HouseNumber">The house number.</param>
        /// <param name="FloorLevel">The floor level.</param>
        /// <param name="PostalCode">The postal code</param>
        /// <param name="PostalCodeSub">The postal code sub</param>
        /// <param name="City">The city.</param>
        /// <param name="Country">The country.</param>
        /// <param name="Comment">An optional text/comment to describe the address.</param>
        /// 
        /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
        public Address(String                      Street,
                       String                      HouseNumber,
                       String                      FloorLevel,
                       String                      PostalCode,
                       String                      PostalCodeSub,
                       I18NString                  City,
                       Country                     Country,
                       I18NString                  Comment      = null,

                       Dictionary<String, Object>  CustomData   = null)

            : base(CustomData)

        {

            this.Street         = Street        ?? "";
            this.HouseNumber    = HouseNumber   ?? "";
            this.FloorLevel     = FloorLevel    ?? "";
            this.PostalCode     = PostalCode    ?? "";
            this.PostalCodeSub  = PostalCodeSub ?? "";
            this.City           = City          ?? I18NString.Empty;
            this.Country        = Country;
            this.Comment        = Comment       ?? I18NString.Empty;

        }

        #endregion


        #region (static) Create(Country, PostalCode, City, Street, HouseNumber, CustomData = null)

        /// <summary>
        /// Create a new minimal address.
        /// </summary>
        /// <param name="Country">The country.</param>
        /// <param name="PostalCode">The postal code</param>
        /// <param name="City">The city.</param>
        /// <param name="Street">The name of the street.</param>
        /// <param name="HouseNumber">The house number.</param>
        /// <param name="FloorLevel">The floor level.</param>
        /// <param name="Comment">A comment to this address.</param>
        /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
        public static Address Create(Country                     Country,
                                     String                      PostalCode,
                                     I18NString                  City,
                                     String                      Street,
                                     String                      HouseNumber,
                                     String                      FloorLevel   = null,
                                     I18NString                  Comment      = null,
                                     Dictionary<String, Object>  CustomData   = null)


            => new Address(Street,
                           HouseNumber,
                           FloorLevel,
                           PostalCode,
                           "",
                           City,
                           Country,
                           Comment,
                           CustomData);

        #endregion

        //public static Address Parse(String Text)
        //{

        //    if (TryParseJSON(JObject.Parse(Text), out Address _Address))
        //        return _Address;

        //    return null;

        //}

        //public static Address ParseAddressJSON(JObject JSONObject, String PropertyKey)
        //{

        //    try
        //    {

        //        if (JSONObject[PropertyKey] is JObject JSON)
        //            return Create(Country.Parse(JSON["country"    ]?.Value<String>()),
        //                                        JSON["postalCode" ]?.Value<String>(),
        //                                       (JSON["city"       ] as JObject)?.ParseI18NString(),
        //                                        JSON["street"     ]?.Value<String>(),
        //                                        JSON["houseNumber"]?.Value<String>(),
        //                                        JSON["floorLevel" ]?.Value<String>(),
        //                                       (JSON["comment"    ] as JObject)?.ParseI18NString());

        //    }
        //    catch (Exception)
        //    { }

        //    return null;

        //}


        #region ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public JObject ToJSON(Boolean Embedded  = false)

            => JSONObject.Create(

                   !Embedded
                       ? new JProperty("@context", JSONLDContext.ToString())
                       : null,

                   FloorLevel.         ToJSON("floorLevel"),
                   HouseNumber.        ToJSON("houseNumber"),
                   Street.             ToJSON("street"),
                   PostalCode.         ToJSON("postalCode"),
                   PostalCodeSub.      ToJSON("postalCodeSub"),
                   City.               ToJSON("city"),
                   Country?.Alpha3Code.ToJSON("country"),
                   Comment.            ToJSON("comment")

                );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out Address, out ErrorResponse)

        public static Boolean TryParse(JObject      JSONObject,
                                       out Address  Address,
                                       out String   ErrorResponse)
        {

            try
            {

                Address = null;

                if (JSONObject?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PrivacyLevel     [optional]

                if (JSONObject.ParseOptional("country",
                                             "country",
                                             Country.TryParse,
                                             out Country country,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse PostalCode       [optional]

                if (JSONObject.ParseOptional("postalCode",
                                             "postal code",
                                             out String postalCode,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse postalCodeSub    [optional]

                if (JSONObject.ParseOptional("postalCodeSub",
                                             "postal code sub",
                                             out String postalCodeSub,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse City             [optional]

                if (JSONObject.ParseOptional("city",
                                             "city",
                                             out I18NString city,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Street           [optional]

                if (JSONObject.ParseOptional("street",
                                             "street",
                                             out String street,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse HouseNumber      [optional]

                if (JSONObject.ParseOptional("houseNumber",
                                             "house number",
                                             out String houseNumber,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse FloorLevel       [optional]

                if (JSONObject.ParseOptional("floorLevel",
                                             "floor level",
                                             out String floorLevel,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Comment          [optional]

                if (JSONObject.ParseOptional("comment",
                                             "comment",
                                             out I18NString comment,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                Address = new Address(street,
                                      houseNumber,
                                      floorLevel,
                                      postalCode,
                                      postalCodeSub,
                                      city,
                                      country,
                                      comment);


                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = "Parsing the given address failed: " + e.Message;
                Address        = null;
                return false;
            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (Address1, Address2)

        /// <summary>
        /// Compares two addresses for equality.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Address Address1, Address Address2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Address1, Address2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Address1 == null) || ((Object) Address2 == null))
                return false;

            return Address1.Equals(Address2);

        }

        #endregion

        #region Operator != (Address1, Address2)

        /// <summary>
        /// Compares two addresses for inequality.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Address Address1, Address Address2)

            => !(Address1 == Address2);

        #endregion

        #region Operator <  (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Address Address1, Address Address2)
        {

            if ((Object) Address1 == null)
                throw new ArgumentNullException(nameof(Address1), "The given Address1 must not be null!");

            return Address1.CompareTo(Address2) < 0;

        }

        #endregion

        #region Operator <= (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Address Address1, Address Address2)
            => !(Address1 > Address2);

        #endregion

        #region Operator >  (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Address Address1, Address Address2)
        {

            if ((Object)Address1 == null)
                throw new ArgumentNullException(nameof(Address1), "The given Address1 must not be null!");

            return Address1.CompareTo(Address2) > 0;

        }

        #endregion

        #region Operator >= (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Address Address1, Address Address2)
            => !(Address1 < Address2);

        #endregion

        #endregion

        #region IComparable<Address> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var Address = Object as Address;
            if ((Object)Address == null)
                throw new ArgumentException("The given object is not an address identification!", nameof(Object));

            return CompareTo(Address);

        }

        #endregion

        #region CompareTo(Address)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address">An object to compare with.</param>
        public Int32 CompareTo(Address Address)
        {

            if ((Object) Address == null)
                throw new ArgumentNullException(nameof(Address), "The given address must not be null!");

            var c = Country.     CompareTo(Address.Country);
            if (c != 0)
                return c;

            c = PostalCode.      CompareTo(Address.PostalCode);
            if (c != 0)
                return c;

            c = City.FirstText().CompareTo(Address.City.FirstText());
            if (c != 0)
                return c;

            return Street.       CompareTo(Address.Street);

        }

        #endregion

        #endregion

        #region IEquatable<Address> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is an Address.
            var Address = Object as Address;
            if ((Object) Address == null)
                return false;

            return this.Equals(Address);

        }

        #endregion

        #region Equals(Address)

        /// <summary>
        /// Compares two addresses for equality.
        /// </summary>
        /// <param name="Address">An address to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Address Address)
        {

            if ((Object) Address == null)
                return false;

            try
            {

            return Street.        Equals(Address.Street)        &&
                   HouseNumber.   Equals(Address.HouseNumber)   &&
                   FloorLevel.    Equals(Address.FloorLevel)    &&
                   PostalCode.    Equals(Address.PostalCode)    &&
                   PostalCodeSub. Equals(Address.PostalCodeSub) &&
                   City.          Equals(Address.City)          &&
                   Country.       Equals(Address.Country);

            }
            catch (Exception e)
            {
                return false;
            }

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Street.        GetHashCode() * 17 ^
                       HouseNumber.   GetHashCode() * 13 ^
                       FloorLevel.    GetHashCode() * 11 ^
                       PostalCode.    GetHashCode() *  7 ^
                       PostalCodeSub. GetHashCode() *  5 ^
                       City.          GetHashCode() *  3 ^
                       Country.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Street                          + " " +
               HouseNumber                     + " " +
               FloorLevel                      + ", " +
               PostalCode                      + " " +
               PostalCodeSub                   + " " +
               City                            + ", " +
               Country.CountryName.FirstText() + " / " +
               Comment;

        #endregion

    }

}
