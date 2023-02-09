/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A WWCP address.
    /// </summary>
    public class Address : AInternalData,
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
        public String                   Street               { get; }

        /// <summary>
        /// The postal code.
        /// </summary>
        public String                   PostalCode           { get; }

        /// <summary>
        /// The city.
        /// </summary>
        public I18NString               City                 { get; }

        /// <summary>
        /// The country.
        /// </summary>
        public Country                  Country              { get; }

        /// <summary>
        /// The house number.
        /// </summary>
        public String?                  HouseNumber          { get; }

        /// <summary>
        /// The floor level.
        /// </summary>
        public String?                  FloorLevel           { get; }

        /// <summary>
        /// The region.
        /// </summary>
        public String?                  Region               { get; }

        /// <summary>
        /// The postal code sub.
        /// </summary>
        public String?                  PostalCodeSub        { get; }

        /// <summary>
        /// The timezone.
        /// </summary>
        public Time_Zone?               TimeZone             { get; }

        /// <summary>
        /// The official languages at this address.
        /// </summary>
        public IEnumerable<Languages>?  OfficialLanguages    { get; }

        /// <summary>
        /// An optional text/comment to describe the address.
        /// </summary>
        public I18NString?              Comment              { get; }

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
        /// <param name="TimeZone">The timezone.</param>
        /// <param name="Comment">An optional text/comment to describe the address.</param>
        /// 
        /// <param name="CustomData">Optional custom data, e.g. in combination with custom parsers and serializers.</param>
        /// <param name="InternalData">An optional dictionary of internal data.</param>
        public Address(String                       Street,
                       String                       PostalCode,
                       I18NString                   City,
                       Country                      Country,

                       String?                      HouseNumber         = null,
                       String?                      FloorLevel          = null,
                       String?                      Region              = null,
                       String?                      PostalCodeSub       = null,
                       Time_Zone?                   TimeZone            = null,
                       IEnumerable<Languages>?      OfficialLanguages   = null,
                       I18NString?                  Comment             = null,

                       JObject?                     CustomData          = null,
                       UserDefinedDictionary?       InternalData        = null)

            : base(CustomData,
                   InternalData)

        {

            this.Street             = Street;
            this.PostalCode         = PostalCode;
            this.City               = City;
            this.Country            = Country;

            this.HouseNumber        = HouseNumber;
            this.FloorLevel         = FloorLevel;
            this.Region             = Region;
            this.PostalCodeSub      = PostalCodeSub;
            this.TimeZone           = TimeZone;
            this.OfficialLanguages  = OfficialLanguages;
            this.Comment            = Comment;

        }

        #endregion


        #region (static) Parse   (JSON, CustomAddressParser = null)

        /// <summary>
        /// Parse the given JSON representation of an address.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAddressParser">A delegate to parse custom address JSON objects.</param>
        public static Address Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<Address>?  CustomAddressParser   = null)
        {

            if (TryParse(JSON,
                         out Address?  address,
                         out String?   errorResponse,
                         CustomAddressParser))
            {
                return address!;
            }

            throw new ArgumentException("The given JSON representation of an address is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomAddressParser = null)

        /// <summary>
        /// Parse the given text representation of an address.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomAddressParser">A delegate to parse custom address JSON objects.</param>
        public static Address Parse(String                                 Text,
                                    CustomJObjectParserDelegate<Address>?  CustomAddressParser   = null)
        {

            if (TryParse(Text,
                         out Address?  address,
                         out String?   errorResponse,
                         CustomAddressParser))
            {
                return address!;
            }

            throw new ArgumentException("The given text representation of an address is invalid: " + errorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out Address, out ErrorResponse, CustomAddressParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an address.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Address">The parsed address.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject       JSON,
                                       out Address?  Address,
                                       out String?   ErrorResponse)

            => TryParse(JSON,
                        out Address,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an address.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Address">The parsed address.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAddressParser">A delegate to parse custom addresss JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       out Address?                           Address,
                                       out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<Address>?  CustomAddressParser)
        {

            try
            {

                Address = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Street                [mandatory]

                if (!JSON.ParseMandatoryText("street",
                                             "street",
                                             out String Street,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Country               [mandatory]

                if (!JSON.ParseMandatory("country",
                                         "country",
                                         Illias.Country.TryParse,
                                         out Country Country,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PostalCode            [mandatory]

                if (!JSON.ParseMandatoryText("postalCode",
                                             "postal code",
                                             out String PostalCode,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse City                  [mandatory]

                if (!JSON.ParseMandatory("city",
                                         "city",
                                         out I18NString City,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse HouseNumber           [optional]

                if (JSON.ParseOptional("houseNumber",
                                       "house number",
                                       out String houseNumber,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse FloorLevel            [optional]

                if (JSON.ParseOptional("floorLevel",
                                       "floor level",
                                       out String floorLevel,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Region                [optional]

                if (JSON.ParseOptional("region",
                                       "region",
                                       out String Region,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse PostalCodeSub         [optional]

                if (JSON.ParseOptional("postalCodeSub",
                                             "postal code sub",
                                             out String PostalCodeSub,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse TimeZone              [optional]

                if (JSON.ParseOptional("TimeZone",
                                       "time zone",
                                       Time_Zone.TryParse,
                                       out Time_Zone? TimeZone,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Comment               [optional]

                if (JSON.ParseOptional("comment",
                                       "comment",
                                       out I18NString comment,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse OfficialLanguages     [optional]

                if (JSON.ParseOptionalEnums("officialLanguages",
                                            "official languages",
                                            out HashSet<Languages> OfficialLanguages,
                                            out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                #region Parse CustomData            [optional]

                var customData = JSON[nameof(CustomData)] as JObject;

                #endregion


                Address = new Address(Street,
                                      PostalCode,
                                      City,
                                      Country,

                                      houseNumber,
                                      floorLevel,
                                      Region,
                                      PostalCodeSub,
                                      TimeZone,
                                      OfficialLanguages,
                                      comment,

                                      customData);


                if (CustomAddressParser is not null)
                    Address = CustomAddressParser(JSON,
                                                  Address);

                return true;

            }
            catch (Exception e)
            {
                Address        = default;
                ErrorResponse  = "The given JSON representation of an address is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out Address, out ErrorResponse, CustomAddressParser = null)

        /// <summary>
        /// Try to parse the given text representation of a address.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Address">The parsed address.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAddressParser">A delegate to parse custom addresss JSON objects.</param>
        public static Boolean TryParse(String                                 Text,
                                       out Address?                           Address,
                                       out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<Address>?  CustomAddressParser)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Address,
                                out ErrorResponse,
                                CustomAddressParser);

            }
            catch (Exception e)
            {
                Address        = default;
                ErrorResponse  = "The given text representation of an address is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(Embedded = false, CustomAddressSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        /// <param name="CustomAddressSerializer">A delegate to serialize custom address JSON objects.</param>
        public JObject ToJSON(Boolean                                    Embedded                  = false,
                              CustomJObjectSerializerDelegate<Address>?  CustomAddressSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           !Embedded
                               ? new JProperty("@context", JSONLDContext.ToString())
                               : null,

                           new JProperty("street",                    Street),
                           new JProperty("postalCode",                PostalCode),
                           new JProperty("city",                      City.ToJSON()),
                           new JProperty("country",                   Country.Alpha3Code),

                           HouseNumber.IsNotNullOrEmpty()
                               ? new JProperty("houseNumber",         HouseNumber)
                               : null,

                           FloorLevel.IsNotNullOrEmpty()
                               ? new JProperty("floorLevel",          FloorLevel)
                               : null,

                           Region.IsNotNullOrEmpty()
                               ? new JProperty("region",              Region)
                               : null,

                           PostalCodeSub.IsNotNullOrEmpty()
                               ? new JProperty("postalCodeSub",       PostalCodeSub)
                               : null,

                           TimeZone.HasValue
                               ? new JProperty("timeZone",            TimeZone.Value.ToString())
                               : null,

                           OfficialLanguages is not null && OfficialLanguages.Any()
                               ? new JProperty("officialLanguages",   new JArray(OfficialLanguages.Select(language => language.ToString())))
                               : null,

                           Comment is not null && Comment.IsNeitherNullNorEmpty()
                               ? new JProperty("comment",             Comment.ToJSON())
                               : null,

                           CustomData is not null && CustomData.HasValues
                               ? new JProperty("customData",          CustomData)
                               : null

                       );

            return CustomAddressSerializer is not null
                       ? CustomAddressSerializer(this, JSON)
                       : JSON;

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
        public static Boolean operator == (Address? Address1,
                                           Address? Address2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Address1, Address2))
                return true;

            if (Address1 is null || Address2 is null)
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
        public static Boolean operator != (Address? Address1,
                                           Address? Address2)

            => !(Address1 == Address2);

        #endregion

        #region Operator <  (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Address? Address1,
                                          Address? Address2)
        {

            if (Address1 is null)
                throw new ArgumentNullException(nameof(Address1), "The given address must not be null!");

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
        public static Boolean operator <= (Address? Address1,
                                           Address? Address2)

            => !(Address1 > Address2);

        #endregion

        #region Operator >  (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Address? Address1,
                                          Address? Address2)
        {

            if (Address1 is null)
                throw new ArgumentNullException(nameof(Address1), "The given address must not be null!");

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
        public static Boolean operator >= (Address? Address1,
                                           Address? Address2)

            => !(Address1 < Address2);

        #endregion

        #endregion

        #region IComparable<Address> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Address address
                   ? CompareTo(address)
                   : throw new ArgumentException("The given object is not an address!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Address)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address">An object to compare with.</param>
        public Int32 CompareTo(Address? Address)
        {

            if (Address is null)
                throw new ArgumentNullException(nameof(Address), "The given address must not be null!");

            var c = Street.      CompareTo(Address.Street);
            if (c != 0)
                return c;

            c = PostalCode.      CompareTo(Address.PostalCode);
            if (c != 0)
                return c;

            c = City.FirstText().CompareTo(Address.City.FirstText());
            if (c != 0)
                return c;

            c = Country.         CompareTo(Address.Country);
            if (c != 0)
                return c;


            if (HouseNumber   is not null && Address.HouseNumber   is not null)
            {
                c = HouseNumber.CompareTo(Address.HouseNumber);
                if (c != 0)
                    return c;
            }

            if (FloorLevel    is not null && Address.FloorLevel    is not null)
            {
                c = FloorLevel.CompareTo(Address.FloorLevel);
                if (c != 0)
                    return c;
            }

            if (Region        is not null && Address.Region        is not null)
            {
                c = Region.CompareTo(Address.Region);
                if (c != 0)
                    return c;
            }

            if (PostalCodeSub is not null && Address.PostalCodeSub is not null)
            {
                c = PostalCodeSub.CompareTo(Address.PostalCodeSub);
                if (c != 0)
                    return c;
            }

            if (TimeZone      is not null && Address.TimeZone      is not null)
            {
                c = TimeZone.Value.CompareTo(Address.TimeZone.Value);
                if (c != 0)
                    return c;
            }


            // OfficialLanguages


            if (Comment is not null && Address.Comment is not null)
            {
                c = Comment.FirstText().CompareTo(Address.Comment.FirstText());
                if (c != 0)
                    return c;
            }

            return c;

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
        public override Boolean Equals(Object? Object)

            => Object is Address address &&
                   Equals(address);

        #endregion

        #region Equals(Address)

        /// <summary>
        /// Compares two addresses for equality.
        /// </summary>
        /// <param name="Address">An address to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Address? Address)

            => Address is not null &&

               String. Equals(Street,
                              Address.Street,
                              StringComparison.OrdinalIgnoreCase) &&

               String. Equals(PostalCode,
                              Address.PostalCode,
                              StringComparison.OrdinalIgnoreCase) &&

               City.   Equals(Address.City)                       &&
               Country.Equals(Address.Country)                    &&

               String. Equals(HouseNumber,
                              Address.HouseNumber,
                              StringComparison.OrdinalIgnoreCase) &&

               String. Equals(FloorLevel,
                              Address.FloorLevel,
                              StringComparison.OrdinalIgnoreCase) &&

               String. Equals(Region,
                              Address.Region,
                              StringComparison.OrdinalIgnoreCase) &&

               String. Equals(PostalCodeSub,
                              Address.PostalCodeSub,
                              StringComparison.OrdinalIgnoreCase) &&

// TimeZone
// OfficialLanguages

               ((Comment is     null && Address.Comment is     null) ||
                (Comment is not null && Address.Comment is not null && Comment.Equals(Address.Comment)));

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

                return Street.         GetHashCode()       * 31 ^
                       PostalCode.     GetHashCode()       * 29 ^
                       City.           GetHashCode()       * 23 ^
                       Country.        GetHashCode()       * 17 ^

                       (HouseNumber?.  GetHashCode() ?? 0) * 13 ^
                       (FloorLevel?.   GetHashCode() ?? 0) * 11 ^
                       (Region?.       GetHashCode() ?? 0) *  7 ^
                       (PostalCodeSub?.GetHashCode() ?? 0) *  5 ^
                       (TimeZone?.     GetHashCode() ?? 0) *  3 ^
                       // OfficialLanguages
                       (Comment?.      GetHashCode() ?? 0);

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
