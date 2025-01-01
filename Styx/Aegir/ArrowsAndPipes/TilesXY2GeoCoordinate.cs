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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    #region (class) GeoCalculations

    /// <summary>
    /// A bunch of extension methods to transform geo coordinates
    /// into their corresponding tile identification.
    /// </summary>
    public static partial class GeoCalculations
    {

        #region TilesXY2GeoCoordinate(Tile, ZoomLevel)

        /// <summary>
        /// Get the corresponding geo coordinate for the given tile.
        /// </summary>
        /// <param name="Tile">A mapping tile.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        public static GeoCoordinate TilesXY2GeoCoordinate(TilesXY Tile, UInt32 ZoomLevel)
        {

            var n = Math.PI - ((2.0 * Math.PI * Tile.Y) / Math.Pow(2.0, ZoomLevel));

            return new GeoCoordinate(
                Latitude. Parse((Tile.X / Math.Pow(2.0, ZoomLevel) * 360.0) - 180.0),
                Longitude.Parse(180.0 / Math.PI * Math.Atan(Math.Sinh(n)))
            );

        }

        #endregion


        #region ToTilesXY(this Tiles, ZoomLevel)

        /// <summary>
        /// Transform the given enumeration of tiles
        /// into their corresponding geo coordinates.
        /// </summary>
        /// <param name="Tiles">An enumeration of mapping tiles.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        public static TilesXY2GeoCoordinatePipe ToGeoCoordinate(this IEnumerable<TilesXY>  Tiles,
                                                                UInt32                     ZoomLevel)
        {
            return new TilesXY2GeoCoordinatePipe(ZoomLevel, Tiles);
        }

        #endregion

        #region ToGeoCoordinate(ArrowSender, ZoomLevel, OnError)

        /// <summary>
        /// Transform the given enumeration of tiles
        /// into their corresponding geo coordinates.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        public static TilesXY2GeoCoordinateArrow ToGeoCoordinate(this IArrowSender<TilesXY>  ArrowSender,
                                                                 UInt32                      ZoomLevel,
                                                                 Func<Exception, Exception>  OnError     = null)
        {
            return new TilesXY2GeoCoordinateArrow(ZoomLevel, OnError, ArrowSender);
        }

        #endregion

    }

    #endregion


    #region TilesXY2GeoCoordinatePipe(ZoomLevel, IEnumerable = null, IEnumerator = null)

    /// <summary>
    /// A pipe transforming an enumeration of tiles
    /// into their corresponding geo coordinates.
    /// </summary>
    public class TilesXY2GeoCoordinatePipe : SelectPipe<TilesXY, GeoCoordinate>
    {

        /// <summary>
        /// Create a new pipe to transform an enumeration of tiles
        /// into their corresponding geo coordinates.
        /// </summary>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="IEnumerable">An optional IEnumerable&lt;S&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;S&gt; as element source.</param>
        public TilesXY2GeoCoordinatePipe(UInt32                ZoomLevel,
                                         IEnumerable<TilesXY>  IEnumerable = null,
                                         IEnumerator<TilesXY>  IEnumerator = null)

            : base(IEnumerable,
                   Item => GeoCalculations.TilesXY2GeoCoordinate(Item, ZoomLevel))

        { }

    }

    #endregion

    #region TilesXY2GeoCoordinateArrow(ZoomLevel, OnError = null, ArrowSender = null)

    /// <summary>
    /// An arrow transforming an enumeration of tiles
    /// into their corresponding geo coordinates.
    /// </summary>
    public class TilesXY2GeoCoordinateArrow : MapArrow<TilesXY, GeoCoordinate>
    {

        /// <summary>
        /// Create a new arrow to transform an enumeration of tiles
        /// into their corresponding geo coordinates.
        /// </summary>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public TilesXY2GeoCoordinateArrow(UInt32                      ZoomLevel,
                                          Func<Exception, Exception>  OnError     = null,
                                          IArrowSender<TilesXY>       ArrowSender = null)

            : base(Item => GeoCalculations.TilesXY2GeoCoordinate(Item, ZoomLevel),
                   OnError,
                   ArrowSender)

        { }

    }

    #endregion

}
