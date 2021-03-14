/*
 * Copyright (c) 2010-2021, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    #region (class) GeoCalculations

    /// <summary>
    /// A bunch of extention methods to transform geo coordinates
    /// into their corresponding tile identification.
    /// </summary>
    public static partial class GeoCalculations
    {

        #region GeoCoordinate2TilesXY(this GeoCoordinates)

        /// <summary>
        /// Get the corresponding tile for the given geo coordinate.
        /// </summary>
        /// <param name="GeoCoordinates">An enumeration of geo coordinates.</param>
        public static GeoBoundingBox GeoCoordinate2BoundingBox(this IEnumerable<GeoCoordinate> GeoCoordinates)
        {

            var MinLat = Double.MaxValue;
            var MaxLat = Double.MinValue;

            var MinLng = Double.MaxValue;
            var MaxLng = Double.MinValue;

            var MinAlt = Double.MaxValue;
            var MaxAlt = Double.MinValue;

            foreach (var GeoCoordinate in GeoCoordinates)
            {

                if (GeoCoordinate.Latitude.Value < MinLat)
                    MinLat = GeoCoordinate.Latitude.Value;

                if (GeoCoordinate.Longitude.Value < MinLng)
                    MinLng = GeoCoordinate.Longitude.Value;

                if (GeoCoordinate.Altitude.HasValue &&
                    GeoCoordinate.Altitude.Value.Value < MinAlt)
                    MinAlt = GeoCoordinate.Altitude.Value.Value;


                if (GeoCoordinate.Latitude.Value > MaxLat)
                    MaxLat = GeoCoordinate.Latitude.Value;

                if (GeoCoordinate.Longitude.Value > MaxLng)
                    MaxLng = GeoCoordinate.Longitude.Value;

                if (GeoCoordinate.Altitude.HasValue &&
                    GeoCoordinate.Altitude.Value.Value > MaxAlt)
                    MaxAlt = GeoCoordinate.Altitude.Value.Value;

            }

            return new GeoBoundingBox(

                           Latitude. Parse(MinLat),
                           Longitude.Parse(MinLng),
                           Altitude. Parse(MinAlt),

                           Latitude. Parse(MaxLat),
                           Longitude.Parse(MaxLng),
                           Altitude. Parse(MaxAlt)

                       );

        }

        #endregion


        //#region ToTilesXY(this GeoCoordinates, ZoomLevel)

        ///// <summary>
        ///// Transform the given enumeration of geo coordinates
        ///// into their corresponding tile identification.
        ///// </summary>
        ///// <param name="GeoCoordinates">An enumeration of geo coordinates.</param>
        ///// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        //public static GeoCoordinate2TilesXYPipe ToTilesXY(this IEnumerable<GeoCoordinate>  GeoCoordinates,
        //                                                  UInt32                           ZoomLevel)
        //{
        //    return new GeoCoordinate2TilesXYPipe(ZoomLevel, GeoCoordinates);
        //}

        //#endregion

        //#region ToTilesXY(ArrowSender, ZoomLevel, OnError)

        ///// <summary>
        ///// Transform the given enumeration of geo coordinates
        ///// into their corresponding tile identification.
        ///// </summary>
        ///// <param name="ArrowSender">The sender of the messages/objects.</param>
        ///// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        ///// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        //public static GeoCoordinate2TilesXYArrow ToTilesXY(this IArrowSender<GeoCoordinate>  ArrowSender,
        //                                                   UInt32                            ZoomLevel,
        //                                                   Func<Exception, Exception>        OnError     = null)
        //{
        //    return new GeoCoordinate2TilesXYArrow(ZoomLevel, OnError, ArrowSender);
        //}

        //#endregion

    }

    #endregion


    //#region GeoCoordinate2TilesXYPipe(ZoomLevel, IEnumerable = null, IEnumerator = null)

    ///// <summary>
    ///// A pipe transforming an enumeration of geo coordinates
    ///// into their corresponding tile identification.
    ///// </summary>
    //public class GeoCoordinate2TilesXYPipe : FuncPipe<GeoCoordinate, TilesXY>
    //{

    //    /// <summary>
    //    /// Create a new pipe to transform an enumeration of geo coordinates
    //    /// into their corresponding tile identification.
    //    /// </summary>
    //    /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
    //    /// <param name="IEnumerable">An optional IEnumerable&lt;S&gt; as element source.</param>
    //    /// <param name="IEnumerator">An optional IEnumerator&lt;S&gt; as element source.</param>
    //    public GeoCoordinate2TilesXYPipe(UInt32                      ZoomLevel,
    //                                     IEnumerable<GeoCoordinate>  IEnumerable = null,
    //                                     IEnumerator<GeoCoordinate>  IEnumerator = null)

    //        : base(Item => GeoCalculations.GeoCoordinate2TilesXY(Item, ZoomLevel),
    //               IEnumerable,
    //               IEnumerator)

    //    { }

    //}

    //#endregion

    //#region GeoCoordinate2TilesXYArrow(ZoomLevel, OnError = null, ArrowSender = null)

    ///// <summary>
    ///// An arrow transforming an enumeration of geo coordinates
    ///// into their corresponding tile identification.
    ///// </summary>
    //public class GeoCoordinate2TilesXYArrow : MapArrow<GeoCoordinate, TilesXY>
    //{

    //    /// <summary>
    //    /// Create a new arrow to transform an enumeration of geo coordinates
    //    /// into their corresponding tile identification.
    //    /// </summary>
    //    /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
    //    /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
    //    /// <param name="ArrowSender">The sender of the messages/objects.</param>
    //    public GeoCoordinate2TilesXYArrow(UInt32                       ZoomLevel,
    //                                      Func<Exception, Exception>   OnError     = null,
    //                                      IArrowSender<GeoCoordinate>  ArrowSender = null)

    //        : base(Item => GeoCalculations.GeoCoordinate2TilesXY(Item, ZoomLevel),
    //               OnError,
    //               ArrowSender)

    //    { }

    //}

    //#endregion

}
