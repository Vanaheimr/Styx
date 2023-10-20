/*
 * Copyright (c) 2010-2023, Achim Friedland <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    #region (class) GeoCalculations

    /// <summary>
    /// A bunch of extension methods to transform the current
    /// mouse position into a geo coordinate.
    /// </summary>
    public static partial class GeoCalculations
    {

        #region Mouse2GeoCoordinate(GeoCoordinate, ZoomLevel)

        /// <summary>
        /// Get the geo coordinate for the given mouse position on the map.
        /// </summary>
        /// <param name="MouseX">The X position of the mouse on the map.</param>
        /// <param name="MouseY">The Y position of the mouse on the map.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        public static GeoCoordinate Mouse2GeoCoordinate(Double MouseX, Double MouseY, UInt32 ZoomLevel)
        {

            var MapSize = Math.Pow(2.0, ZoomLevel) * 256;

            var n = Math.PI - ((2.0 * Math.PI * MouseY) / MapSize);

            return new GeoCoordinate(
                Latitude. Parse((180.0 / Math.PI * Math.Atan(Math.Sinh(n))) % 90),
                Longitude.Parse(((MouseX / MapSize * 360.0) - 180.0) % 90)
            );

        }

        #endregion


        #region ToTilesXY(this GeoCoordinates, ZoomLevel)

        /// <summary>
        /// Transform the given enumeration of mouse positions
        /// into their corresponding geo coordinates.
        /// </summary>
        /// <param name="MousePositions">An enumeration of geo coordinates.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        public static Mouse2GeoCoordinatePipe ToGeoCoordinates(this IEnumerable<Tuple<Double, Double>>  MousePositions,
                                                               UInt32                                   ZoomLevel)
        {
            return new Mouse2GeoCoordinatePipe(ZoomLevel, MousePositions);
        }

        #endregion

        #region ToTilesXY(ArrowSender, ZoomLevel, OnError)

        /// <summary>
        /// Transform the given enumeration of mouse positions
        /// into their corresponding geo coordinates.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        public static Mouse2GeoCoordinateArrow ToTilesXY(this IArrowSender<Tuple<Double, Double>>  ArrowSender,
                                                         UInt32                                    ZoomLevel,
                                                         Func<Exception, Exception>                OnError     = null)
        {
            return new Mouse2GeoCoordinateArrow(ZoomLevel, OnError, ArrowSender);
        }

        #endregion

    }

    #endregion


    #region Mouse2GeoCoordinatePipe(ZoomLevel, IEnumerable = null, IEnumerator = null)

    /// <summary>
    /// A pipe transforming an enumeration of mouse positions
    /// on the map into their corresponding geo coordinates.
    /// </summary>
    public class Mouse2GeoCoordinatePipe : SelectPipe<Tuple<Double, Double>, GeoCoordinate>
    {

        /// <summary>
        /// Create a new pipe to transform an enumeration of mouse positions
        /// on the map into their corresponding geo coordinates.
        /// </summary>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="IEnumerable">An optional IEnumerable&lt;S&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;S&gt; as element source.</param>
        public Mouse2GeoCoordinatePipe(UInt32                              ZoomLevel,
                                       IEnumerable<Tuple<Double, Double>>  IEnumerable = null,
                                       IEnumerator<Tuple<Double, Double>>  IEnumerator = null)

            : base(IEnumerable,
                   Item => GeoCalculations.Mouse2GeoCoordinate(Item.Item1, Item.Item2, ZoomLevel))

        { }

    }

    #endregion

    #region Mouse2GeoCoordinateArrow(ZoomLevel, OnError = null, ArrowSender = null)

    /// <summary>
    /// An arrow transforming an enumeration of mouse positions
    /// on the map into their corresponding geo coordinates.
    /// </summary>
    public class Mouse2GeoCoordinateArrow : MapArrow<Tuple<Double, Double>, GeoCoordinate>
    {

        /// <summary>
        /// Create a new arrow to transform an enumeration of mouse positions
        /// on the map into their corresponding geo coordinates.
        /// </summary>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public Mouse2GeoCoordinateArrow(UInt32                               ZoomLevel,
                                        Func<Exception, Exception>           OnError     = null,
                                        IArrowSender<Tuple<Double, Double>>  ArrowSender = null)

            : base(Item => GeoCalculations.Mouse2GeoCoordinate(Item.Item1, Item.Item2, ZoomLevel),
                   OnError,
                   ArrowSender)

        { }

    }

    #endregion

}
