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
using System.Text.RegularExpressions;
using System.Globalization;

using org.GraphDefined.Vanaheimr.Illias;
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// The distance metric.
    /// </summary>
    public enum DistanceMetricTypes
    {

        /// <summary>
        /// Unknown distance metric.
        /// </summary>
        unkown,

        /// <summary>
        /// Air-line distance.
        /// </summary>
        air,

        /// <summary>
        /// Walking distance.
        /// </summary>
        foot,

        /// <summary>
        /// Distance via bikes.
        /// </summary>
        bike,

        /// <summary>
        /// Distance for (self-driving) cars.
        /// </summary>
        car

    }

        /// <summary>
    /// An element with distance information.
    /// </summary>
    public class WithDistance<T>
        where T: class
    {

        #region Properties

        /// <summary>
        /// An element.
        /// </summary>
        public T                    Element          { get; }

        /// <summary>
        /// The distance to the given element.
        /// </summary>
        public Double               Distance         { get; }

        /// <summary>
        /// The distance metric towards the given element.
        /// </summary>
        public DistanceMetricTypes  DistanceMetric   { get; }

        /// <summary>
        /// The expected time to reach the given element.
        /// </summary>
        public TimeSpan?            TravelTime       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// An element with distance information.
        /// </summary>
        /// <param name="Element">An element.</param>
        /// <param name="Distance">The distance to the given News.</param>
        /// <param name="DistanceMetric">The distance metric towards the given News.</param>
        /// <param name="TravelTime">The expected time to reach the given News.</param>
        public WithDistance(T                     Element,
                                Double                Distance,
                                DistanceMetricTypes?  DistanceMetric  = null,
                                TimeSpan?             TravelTime      = null)
        {

            this.Element         = Element  ?? throw new ArgumentNullException(nameof(Element), "The given element must not be null!");
            this.Distance        = Distance;
            this.DistanceMetric  = DistanceMetric ?? DistanceMetricTypes.air;
            this.TravelTime      = TravelTime;

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Element, ", ",
                             Distance, " meters via ",
                             DistanceMetric,
                             TravelTime.HasValue ? " (" + TravelTime.Value.TotalMinutes + " minutes)" : "");

        #endregion

    }

    /// <summary>
    /// JSON I/O.
    /// </summary>
    public static class GeoCoordinateExtentions
    {

        #region ToJSON(this GeoLocation)

        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A geographical location.</param>
        public static JObject ToJSON(this GeoCoordinate GeoLocation)
        {

            if (GeoLocation == default(GeoCoordinate))
                return null;

            return JSONObject.Create(
                       GeoLocation.Projection != GravitationalModel.WGS84 ? new JProperty("projection", GeoLocation.Projection.ToString()) : null,
                       new JProperty("lat", GeoLocation.Latitude. Value),
                       new JProperty("lng", GeoLocation.Longitude.Value),
                       GeoLocation.Altitude.HasValue                      ? new JProperty("alt",        GeoLocation.Altitude.Value.Value)  : null
                   );

        }


        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A geographical location.</param>
        public static JObject ToJSON(this GeoCoordinate? GeoLocation)
            => GeoLocation?.ToJSON();

        #endregion

        #region ToJSON(this GeoLocation, JPropertyKey)

        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A geographical location.</param>
        /// <param name="JPropertyKey">The name of the JSON property key to use.</param>
        public static JProperty ToJSON(this GeoCoordinate GeoLocation, String JPropertyKey)
        {

            if (GeoLocation == default(GeoCoordinate))
                return null;

            return new JProperty(JPropertyKey,
                                 GeoLocation.ToJSON());

        }


        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A GeoLocation.</param>
        /// <param name="JPropertyKey">The name of the JSON property key to use.</param>
        public static JProperty ToJSON(this GeoCoordinate? GeoLocation, String JPropertyKey)
        {

            if (!GeoLocation.HasValue)
                return null;

            return new JProperty(JPropertyKey,
                                 GeoLocation.Value.ToJSON());

        }

        #endregion


        public static Boolean TryParseGeoCoordinate(this String Text, out GeoCoordinate GeoLocation)
            => GeoCoordinate.TryParse(JObject.Parse(Text), out GeoLocation);

    }


    /// <summary>
    /// A geographical coordinate or position on a map.
    /// </summary>
    public struct GeoCoordinate : IGeoCoordinate,
                                  IEquatable <GeoCoordinate>,
                                  IComparable<GeoCoordinate>

    {

        #region (static) Regular expressions

        /// <summary>
        /// The regular expression init string for matching decimal numbers.
        /// </summary>
        public const  String IsDecimal_RegExprString                  = "([0-9]+[\\.\\,]?[0-9]*)";

        /// <summary>
        /// The regular expression init string for matching signed decimal numbers.
        /// </summary>
        public const  String IsSignedDecimal_RegExprString            = "([-]?[0-9]+[\\.\\,]?[0-9]*)";

        /// <summary>
        /// The regular expression init string for matching comma seperators.
        /// </summary>
        public const  String MayBeSeperator_RegExprString             = "[\\s,;]+";

        /// <summary>
        /// The regular expression init string for matching decimal geo positions/coordinates.
        /// </summary>
        public const  String IsDecimalGeoPosition_RegExprString       = IsDecimal_RegExprString + "[°]?[\\s]+([SN]?)" +
                                                                        MayBeSeperator_RegExprString +
                                                                        IsDecimal_RegExprString + "[°]?[\\s]+([EWO]?)";

        /// <summary>
        /// The regular expression init string for matching signed decimal geo positions/coordinates.
        /// </summary>
        public const  String IsSignedDecimalGeoPosition_RegExprString = IsSignedDecimal_RegExprString + "[°]?" +
                                                                        MayBeSeperator_RegExprString +
                                                                        IsSignedDecimal_RegExprString + "[°]?";

        /// <summary>
        /// The regular expression init string for matching sexagesimal geo positions/coordinates.
        /// </summary>
        public const  String IsSexagesimalGeoPosition_RegExprString   = "([-]?[0-9])+°[\\s]+([0-9])+'[\\s]+([0-9]+[\\.\\,]?[0-9]*)''[\\s]+([SN]?)" +
                                                                        MayBeSeperator_RegExprString +
                                                                        "([-]?[0-9])+°[\\s]+([0-9])+'[\\s]+([0-9]+[\\.\\,]?[0-9]*)''[\\s]+([EWO]?)";

        /// <summary>
        /// A regular expression for matching decimal geo positions/coordinates.
        /// </summary>
        public static readonly Regex  IsDecimalRegExpr                    = new Regex(IsDecimal_RegExprString);

        /// <summary>
        /// A regular expression for matching decimal geo positions/coordinates.
        /// </summary>
        public static readonly Regex  IsDecimalGeoPositionRegExpr         = new Regex(IsDecimalGeoPosition_RegExprString);

        /// <summary>
        /// A regular expression for matching signed decimal geo positions/coordinates.
        /// </summary>
        public static readonly Regex  IsSignedDecimalGeoPositionRegExpr   = new Regex(IsSignedDecimalGeoPosition_RegExprString);

        /// <summary>
        /// A regular expression for matching sexagesimal geo positions/coordinates.
        /// </summary>
        public static readonly Regex  IsSexagesimalGeoPositionRegExpr     = new Regex(IsSexagesimalGeoPosition_RegExprString);

        #endregion

        #region Properties

        /// <summary>
        /// The planet.
        /// </summary>
        public Planets             Planet       { get; }

        /// <summary>
        /// The Latitude (south to nord).
        /// </summary>
        public Latitude            Latitude     { get; }

        /// <summary>
        /// The Longitude (parallel to equator).
        /// </summary>
        public Longitude           Longitude    { get; }

        /// <summary>
        /// The Altitude.
        /// </summary>
        public Altitude?           Altitude     { get; }

        /// <summary>
        /// The gravitational model.
        /// </summary>
        public GravitationalModel  Projection   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The zero coordinate.
        /// </summary>
        public static GeoCoordinate Zero

            => new GeoCoordinate(Latitude. Parse(0),
                                 Longitude.Parse(0));

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Latitude">The Latitude (south to nord).</param>
        /// <param name="Longitude">The Longitude (parallel to equator).</param>
        /// <param name="Altitude">The (optional) Altitude.</param>
        /// <param name="Projection">The gravitational model or projection of the geo coordinates.</param>
        /// <param name="Planet">The planet.</param>
        public GeoCoordinate(Latitude            Latitude,
                             Longitude           Longitude,
                             Altitude?           Altitude    = null,
                             GravitationalModel  Projection  = GravitationalModel.WGS84,
                             Planets             Planet      = Planets.Earth)
        {

            this.Latitude    = Latitude;
            this.Longitude   = Longitude;
            this.Altitude    = Altitude ?? new Altitude?();
            this.Projection  = Projection;
            this.Planet      = Planet;

        }

        #endregion


        #region Create(Latitude, Longitude, Altitude = null)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Latitude">The Latitude (south to nord).</param>
        /// <param name="Longitude">The Longitude (parallel to equator).</param>
        /// <param name="Altitude">The (optional) Altitude.</param>
        public static GeoCoordinate Create(Latitude   Latitude,
                                           Longitude  Longitude,
                                           Altitude?  Altitude = null)

            => new GeoCoordinate(Latitude, Longitude, Altitude);


        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Latitude">The Latitude (south to nord).</param>
        /// <param name="Longitude">The Longitude (parallel to equator).</param>
        /// <param name="Altitude">The (optional) Altitude.</param>
        public static GeoCoordinate? Create(Latitude?   Latitude,
                                            Longitude?  Longitude,
                                            Altitude?   Altitude = null)

            => Latitude.HasValue && Longitude.HasValue
                   ? new GeoCoordinate(Latitude.Value, Longitude.Value, Altitude)
                   : new GeoCoordinate?();

        #endregion

        #region fromLatLng(Latitude, Longitude, Altitude = null)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Latitude">The Latitude (south to nord).</param>
        /// <param name="Longitude">The Longitude (parallel to equator).</param>
        /// <param name="Altitude">The (optional) Altitude.</param>
        public static GeoCoordinate fromLatLng(Double   Latitude,
                                               Double   Longitude,
                                               Double?  Altitude = null)

            => new GeoCoordinate(
                   Aegir.Latitude. Parse(Latitude),
                   Aegir.Longitude.Parse(Longitude),
                   Altitude.HasValue
                       ? new Aegir.Altitude?(Aegir.Altitude.Parse(Altitude.Value))
                       : null
               );

        #endregion

        #region fromLngLat(Longitude, Latitude, Altitude = null)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Longitude">The Longitude (parallel to equator).</param>
        /// <param name="Latitude">The Latitude (south to nord).</param>
        /// <param name="Altitude">The (optional) Altitude.</param>
        public static GeoCoordinate fromLngLat(Double   Longitude,
                                               Double   Latitude,
                                               Double?  Altitude = null)

            => new GeoCoordinate(
                   Aegir.Latitude. Parse(Latitude),
                   Aegir.Longitude.Parse(Longitude),
                   Altitude.HasValue
                       ? new Aegir.Altitude?(Aegir.Altitude.Parse(Altitude.Value))
                       : null
               );

        #endregion


        #region Parse   (LatitudeString, LongitudeString, AltitudeString = null)

        /// <summary>
        /// Parse the given latitude and longitude string representations.
        /// </summary>
        /// <param name="LatitudeString">The Latitude (south to nord).</param>
        /// <param name="LongitudeString">The Longitude (parallel to equator).</param>
        /// <param name="AltitudeString">The Altitude.</param>
        public static GeoCoordinate Parse(String  LatitudeString,
                                          String  LongitudeString,
                                          String  AltitudeString = null)
        {

            if (!Latitude. TryParse(LatitudeString,  out Latitude  LatitudeValue))
                throw new Exception("Invalid 'latitude' value!");

            if (!Longitude.TryParse(LongitudeString, out Longitude LongitudeValue))
                throw new Exception("Invalid 'longitude' value!");

            if (AltitudeString.IsNeitherNullNorEmpty())
            {

                if (!Aegir.Altitude.TryParse(AltitudeString, out Altitude AltitudeValue))
                    throw new Exception("Invalid 'altitude' value!");

                return new GeoCoordinate(LatitudeValue,
                                         LongitudeValue,
                                         AltitudeValue);

            }

            return new GeoCoordinate(LatitudeValue,
                                     LongitudeValue);

        }

        #endregion

        #region Parse   (LatitudeDouble, LongitudeDouble, AltitudeDouble = null)

        /// <summary>
        /// Parse the given latitude and longitude string representations.
        /// </summary>
        /// <param name="LatitudeDouble">The Latitude (south to nord).</param>
        /// <param name="LongitudeDouble">The Longitude (parallel to equator).</param>
        /// <param name="AltitudeDouble">The Altitude.</param>
        public static GeoCoordinate Parse(Double  LatitudeDouble,
                                          Double  LongitudeDouble,
                                          Double? AltitudeDouble = null)
        {

            if (!Latitude. TryParse(LatitudeDouble,  out Latitude  LatitudeValue))
                throw new Exception("Invalid 'latitude' value!");

            if (!Longitude.TryParse(LongitudeDouble, out Longitude LongitudeValue))
                throw new Exception("Invalid 'longitude' value!");

            if (AltitudeDouble.HasValue)
            {

                if (!Aegir.Altitude.TryParse(AltitudeDouble.Value, out Altitude AltitudeValue))
                    throw new Exception("Invalid 'altitude' value!");

                return new GeoCoordinate(LatitudeValue,
                                         LongitudeValue,
                                         AltitudeValue);
            }

            return new GeoCoordinate(LatitudeValue,
                                     LongitudeValue);

        }

        #endregion

        #region TryParse(LatitudeString, LongitudeString, out GeoCoordinate)

        /// <summary>
        /// Parse the given latitude and longitude string representations.
        /// </summary>
        /// <param name="LatitudeString">The Latitude (south to nord).</param>
        /// <param name="LongitudeString">The Longitude (parallel to equator).</param>
        /// <param name="GeoCoordinate">The resulting geo coordinate.</param>
        public static Boolean TryParse(String             LatitudeString,
                                       String             LongitudeString,
                                       out GeoCoordinate  GeoCoordinate)
        {

            GeoCoordinate = default(GeoCoordinate);

            if (!Latitude. TryParse(LatitudeString,  out Latitude  LatitudeValue))
                return false;

            if (!Longitude.TryParse(LongitudeString, out Longitude LongitudeValue))
                return false;

            GeoCoordinate = new GeoCoordinate(LatitudeValue, LongitudeValue);

            return true;

        }

        #endregion

        #region TryParse(LatitudeString, LongitudeString, AltitudeString, out GeoCoordinate)

        /// <summary>
        /// Parse the given latitude and longitude string representations.
        /// </summary>
        /// <param name="LatitudeString">The Latitude (south to nord).</param>
        /// <param name="LongitudeString">The Longitude (parallel to equator).</param>
        /// <param name="AltitudeString">The Altitude.</param>
        /// <param name="GeoCoordinate">The resulting geo coordinate.</param>
        public static Boolean TryParse(String             LatitudeString,
                                       String             LongitudeString,
                                       String             AltitudeString,
                                       out GeoCoordinate  GeoCoordinate)
        {

            GeoCoordinate = default(GeoCoordinate);

            if (!Latitude.      TryParse(LatitudeString,  out Latitude  LatitudeValue))
                return false;

            if (!Longitude.     TryParse(LongitudeString, out Longitude LongitudeValue))
                return false;

            if (!Aegir.Altitude.TryParse(AltitudeString,  out Altitude  AltitudeValue))
                return false;

            GeoCoordinate = new GeoCoordinate(LatitudeValue,
                                              LongitudeValue,
                                              AltitudeValue);

            return true;

        }

        #endregion


        #region ParseString(GeoString)

        /// <summary>
        /// Parses the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <returns>A new geo position or null.</returns>
        public static GeoCoordinate ParseString(String GeoString)
        {

            if (TryParseString(GeoString, out GeoCoordinate GeoCoordinate))
                return GeoCoordinate;

            return default(GeoCoordinate);

        }

        #endregion

        #region ParseString(GeoString, Processor)

        /// <summary>
        /// Parses the given string as a geo position/coordinate.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="GeoString">A string to parse.</param>
        /// <returns>A new geo position or null.</returns>
        public static T ParseString<T>(String GeoString, Func<Latitude, Longitude, T> Processor)
        {

            if (TryParseString(GeoString, Processor, out T _T))
                return _T;

            return default(T);

        }

        #endregion

        #region TryParseString(GeoString, out GeoCoordinate)

        /// <summary>
        /// Attempts to parse the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <param name="GeoCoordinate">The parsed geo coordinate.</param>
        /// <returns>True if success, false otherwise</returns>
        public static Boolean TryParseString(String GeoString, out GeoCoordinate GeoCoordinate)

            => TryParseString(GeoString, (lat, lng) => new GeoCoordinate(lat, lng), out GeoCoordinate);

        #endregion

        #region TryParseString(GeoString, Processor)

        /// <summary>
        /// Attempts to parse the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <param name="Processor">A delegate to process the parsed latitude and longitude.</param>
        /// <returns>True if success, false otherwise</returns>
        public static Boolean TryParseString(String GeoString, Action<Latitude, Longitude> Processor)

            => TryParseString(GeoString,
                              (lat, lng) => {
                                  Processor(lat, lng);
                                  return true;
                              },
                              out Boolean _Boolean);

        #endregion

        #region TryParseString(GeoString, Processor, out Value)

        /// <summary>
        /// Attempts to parse the given string as a geo position/coordinate.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="GeoString">A string to parse.</param>
        /// <param name="Processor">A delegate to process the parsed latitude and longitude.</param>
        /// <param name="Value">The processed value.</param>
        /// <returns>True if success, false otherwise</returns>
        public static Boolean TryParseString<T>(String GeoString, Func<Latitude, Longitude, T> Processor, out T Value)
        {

            var Match = IsDecimalGeoPositionRegExpr.Match(GeoString);

            if (Match.Success)
            {

                var latitude = Double.Parse(Match.Groups[1].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

                if (Match.Groups[2].Value == "S")
                    latitude = -1 * latitude;

                var Longitude = Double.Parse(Match.Groups[3].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

                if (Match.Groups[4].Value == "W")
                    Longitude = -1 * Longitude;

                Value = Processor(Latitude.Parse(latitude), new Longitude(Longitude));
                return true;

            }

            Match = IsSignedDecimalGeoPositionRegExpr.Match(GeoString);

            if (Match.Success)
            {

                var latitude  = Double.Parse(Match.Groups[1].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);
                var Longitude = Double.Parse(Match.Groups[2].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

                Value = Processor(Latitude.Parse(latitude), new Longitude(Longitude));
                return true;

            }

            Value = default(T);
            return false;

        }

        #endregion


        public static GeoCoordinate? TryParseJSON(String Text)
            => TryParse(JObject.Parse(Text));

        public static Boolean TryParseJSON(String Text, out GeoCoordinate GeoCoordinate)
            => TryParse(JObject.Parse(Text), out GeoCoordinate);

        public static GeoCoordinate? TryParse(JObject JSON)
        {

            if (TryParse(JSON, out GeoCoordinate _GeoCoordinate))
                return _GeoCoordinate;

            return default(GeoCoordinate);

        }

        public static Boolean TryParse(JObject JSON, out GeoCoordinate GeoLocation)
        {

            var lat         = JSON["lat"];
            var lng         = JSON["lng"];
            var alt         = JSON["alt"];
            var projection  = JSON["projection"];

            if (lat != null && lng != null)
            {

                try
                {

                    GeoLocation = Parse(lat. Value<Double>(),
                                        lng. Value<Double>(),
                                        alt?.Value<Double>());

                    return true;

                }
                catch (Exception e)
                {
                }

            }

            GeoLocation = default(GeoCoordinate);
            return false;

        }


        #region DistanceTo(Target)

        /// <summary>
        /// Calculate the distance between two geo coordinates.
        /// </summary>
        /// <param name="Target">Another geo coordinate</param>
        public Double DistanceTo(GeoCoordinate Target)
        {

            var d_lng = Longitude.DistanceTo(Target.Longitude);
            var d_lat = Latitude. DistanceTo(Target.Latitude);

            return Math.Sqrt(d_lng * d_lng + d_lat * d_lat);

        }

        #endregion

        #region DistanceTo(Target, EarthRadiusInKM = 6371)

        /// <summary>
        /// Calculate the distance between two geo coordinates in kilometers.
        /// </summary>
        /// <remarks>See also: http://www.movable-type.co.uk/scripts/latlong.html and http://en.wikipedia.org/wiki/Haversine_formula </remarks>
        /// <param name="Target">Another geo coordinate</param>
        /// <param name="EarthRadiusInKM">The currently accepted (WGS84) earth radius at the equator is 6378.137 km and 6356.752 km at the polar caps. For aviation purposes the FAI uses a radius of 6371.0 km.</param>
        public Double DistanceKM(GeoCoordinate Target, UInt32 EarthRadiusInKM = 6371)
        {

            var dLat = (Target.Latitude.Value  - Latitude. Value).ToRadians();
            var dLon = (Target.Longitude.Value - Longitude.Value).ToRadians();

            var a = Math.Sin(dLat / 2)                   * Math.Sin(dLat / 2) +
                    Math.Cos(Latitude.Value.ToRadians()) * Math.Cos(Target.Latitude.Value.ToRadians()) *
                    Math.Sin(dLon / 2)                   * Math.Sin(dLon / 2);

            // A (surprisingly marginal) performance improvement can be obtained,
            // of course, by factoring out the terms which get squared.
            //return EarthRadiusInKM * 2 * Math.Asin(Math.Sqrt(a));

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusInKM * c;

        }

        #endregion

        #region MidPoint(Target)

        /// <summary>
        /// Returns the midpoint between this point and the supplied point.
        /// </summary>
        /// <remarks>
        /// http://www.movable-type.co.uk/scripts/latlong.html
        /// http://mathforum.org/library/drmath/view/51822.html
        /// </remarks>
        /// <param name="Target">Anothre geo coordinate.</param>
        public GeoCoordinate MidPoint(GeoCoordinate Target)
        {

            var dLat = (Target.Latitude.Value  - Latitude.Value).ToRadians();
            var dLon = (Target.Longitude.Value - Longitude.Value).ToRadians();

            var Bx = Math.Cos(Target.Latitude.Value.ToRadians()) * Math.Cos(dLon);
            var By = Math.Cos(Target.Latitude.Value.ToRadians()) * Math.Sin(dLon);

            var lat3 = Math.Atan2(Math.Sin(Latitude.Value.ToRadians()) +
                                  Math.Sin(Target.Latitude.Value.ToRadians()),
                                  Math.Sqrt((Math.Cos(Latitude.Value.ToRadians()) + Bx) *
                                            (Math.Cos(Latitude.Value.ToRadians()) + Bx) + By * By));

            var lon3 = Longitude.Value.ToRadians() +
                       Math.Atan2(By, Math.Cos(Latitude.Value.ToRadians()) + Bx);

                // Normalise to -180 ... +180º
                lon3 = (lon3 + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

            return new GeoCoordinate(
                           Latitude. Parse(lat3.ToDegree()),
                           Longitude.Parse(lon3.ToDegree())
                       );

        }

        #endregion


        #region (static) Swap(ref Pixel1, ref Pixel2)

        /// <summary>
        /// Swaps two pixels.
        /// </summary>
        /// <param name="Pixel1">The first pixel.</param>
        /// <param name="Pixel2">The second pixel.</param>
        public static void Swap(ref GeoCoordinate Pixel1, ref GeoCoordinate Pixel2)
        {
            var tmp = Pixel2;
            Pixel2 = Pixel1;
            Pixel1 = tmp;
        }

        #endregion

        #region ToGeoString(GeoType = GeoFormat.Decimal, Decimals = 5)

        /// <summary>
        /// Returns a user-friendly string representaion.
        /// </summary>
        public String ToGeoString(GeoFormat GeoType = GeoFormat.Decimal, UInt16 Decimals = 7)
        {

            switch (GeoType)
            {

                // 49.44903° N, 11.07488° E
                case GeoFormat.Decimal:

                    return String.Format("{0}° {1}, {2}° {3}",
                                         Math.Round(Latitude.Value,  Decimals).ToString().Replace(',', '.'),
                                         (Latitude.Value  < 0) ? "S" : "N",
                                         Math.Round(Longitude.Value, Decimals).ToString().Replace(',', '.'),
                                         (Longitude.Value < 0) ? "W" : "E");


                // 49° 26' 56.5'' N, 11° 4' 29.6'' E
                case GeoFormat.Sexagesimal:

                    var Latitude_Grad       = (Latitude.Value > 0) ? (UInt32) Math.Abs(Math.Floor(Latitude.Value)) : (UInt32) Math.Abs(Math.Floor(Latitude.Value) + 1);
                    var Latitude_Minute_dec = (Math.Abs(Latitude.Value) - Latitude_Grad) * 60;
                    var Latitude_Minute     = (UInt32) Math.Floor(Latitude_Minute_dec);
                    var Latitude_Second_dec = (Latitude_Minute_dec - Latitude_Minute) * 60;

                    var Longitude_Grad       = (Longitude.Value > 0) ? (UInt32) Math.Abs(Math.Floor(Longitude.Value)) : (UInt32) Math.Abs(Math.Floor(Longitude.Value) + 1);
                    var Longitude_Minute_dec = (Math.Abs(Longitude.Value) - Longitude_Grad) * 60;
                    var Longitude_Minute     = (UInt32) Math.Floor(Longitude_Minute_dec);
                    var Longitude_Second_dec = (Longitude_Minute_dec - Longitude_Minute) * 60;

                    return String.Format("{0}° {1}' {2}'' {3}, {4}° {5}' {6}'' {7}",
                                         Latitude_Grad,  Latitude_Minute,  Latitude_Second_dec,  (Latitude.Value  < 0) ? "S" : "N",
                                         Longitude_Grad, Longitude_Minute, Longitude_Second_dec, (Longitude.Value < 0) ? "W" : "E");

            }

            return String.Empty;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GeoCoordinate1, GeoCoordinate2)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="GeoCoordinate1">A geo coordinate.</param>
        /// <param name="GeoCoordinate2">Another geo coordinate.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GeoCoordinate GeoCoordinate1, GeoCoordinate GeoCoordinate2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GeoCoordinate1, GeoCoordinate2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GeoCoordinate1 == null) || ((Object) GeoCoordinate2 == null))
                return false;

            return GeoCoordinate1.Equals(GeoCoordinate2);

        }

        #endregion

        #region Operator != (GeoCoordinate1, GeoCoordinate2)

        /// <summary>
        /// Compares two geo coordinates for inequality.
        /// </summary>
        /// <param name="GeoCoordinate1">A geo coordinate.</param>
        /// <param name="GeoCoordinate2">Another geo coordinate.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GeoCoordinate GeoCoordinate1, GeoCoordinate GeoCoordinate2)
        {
            return !(GeoCoordinate1 == GeoCoordinate2);
        }

        #endregion

        #endregion

        #region IComparable<ChargingStation_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given Object must not be null!");

            if (!(Object is GeoCoordinate))
                throw new ArgumentException("The given object is not a GeoCoordinate!", nameof(Object));

            return CompareTo((GeoCoordinate) Object);

        }

        #endregion

        #region CompareTo(GeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates.
        /// </summary>
        /// <param name="GeoCoordinate">Another geo coordinate.</param>
        public Int32 CompareTo(GeoCoordinate GeoCoordinate)
        {

            var lat = GeoCoordinate.Latitude.Value.CompareTo(Latitude.Value);

            if (lat != 0)
                return lat;

            return GeoCoordinate.Longitude.Value.CompareTo(Longitude.Value);

        }

        #endregion

        #endregion

        #region IEquatable<GeoCoordinate> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="Object">Another geo coordinate.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            if (!(Object is GeoCoordinate))
                return false;

            return Equals((GeoCoordinate) Object);

        }

        #endregion

        #region Equals(IGeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="IGeoCoordinate">Another geo coordinate.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(IGeoCoordinate IGeoCoordinate)
        {

            if (IGeoCoordinate.Latitude. Value != Latitude. Value)
                return false;

            if (IGeoCoordinate.Longitude.Value != Longitude.Value)
                return false;

            return true;

        }

        #endregion

        #region Equals(GeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="GeoCoordinate">Another geo coordinate.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(GeoCoordinate GeoCoordinate)
        {

            if (GeoCoordinate.Latitude.Value  != Latitude.Value)
                return false;

            if (GeoCoordinate.Longitude.Value != Longitude.Value)
                return false;

            return true;

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Latitude. GetHashCode() * 5 ^
                       Longitude.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        public override String ToString()

            => String.Concat("Latitude = ",   Latitude.Value,
                             ", Longitude = ",  Longitude.Value,
                             Altitude.HasValue
                                 ? ", Altitude = " + Altitude.Value
                                 : "");

        #endregion

    }

}
