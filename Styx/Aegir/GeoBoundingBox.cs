/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    public class GeoBoundingBox
    {

        public GeoCoordinate  GeoCoordinate1    { get; private set; }
        public Latitude       Latitude          { get; private set; }
        public Longitude      Longitude         { get; private set; }
        public Altitude       Altitude          { get; private set; }

        public GeoCoordinate  GeoCoordinate2    { get; private set; }
        public Latitude       Latitude2         { get; private set; }
        public Longitude      Longitude2        { get; private set; }
        public Altitude       Altitude2         { get; private set; }

        public Double         GeoWidth          { get; private set; }
        public Double         GeoHeight         { get; private set; }
        public Double         GeoAlitude        { get; private set; }


        public GeoBoundingBox(Latitude  Latitude,
                              Longitude Longitude,
                              Latitude  Latitude2,
                              Longitude Longitude2)

            : this(Latitude,  Longitude,  Altitude.Parse(0),
                   Latitude2, Longitude2, Altitude.Parse(0))

        { }

        public GeoBoundingBox(Latitude  Latitude,
                              Longitude Longitude,
                              Altitude  Altitude,
                              Latitude  Latitude2,
                              Longitude Longitude2,
                              Altitude  Altitude2)
        {

            this.GeoCoordinate1 = new GeoCoordinate(Latitude,  Longitude,  Altitude);
            this.Latitude       = Latitude;
            this.Longitude      = Longitude;
            this.Altitude       = Altitude;

            this.GeoCoordinate2 = new GeoCoordinate(Latitude2, Longitude2, Altitude2);
            this.Latitude2      = Latitude2;
            this.Longitude2     = Longitude2;
            this.Altitude2      = Altitude2;

            this.GeoWidth       = GeoCoordinate1.DistanceKM(new GeoCoordinate(GeoCoordinate1.Latitude, GeoCoordinate2.Longitude)); // Longitude West  <-> East
            this.GeoHeight      = GeoCoordinate1.DistanceKM(new GeoCoordinate(GeoCoordinate2.Latitude, GeoCoordinate1.Longitude)); // Latitude  North <-> South
            this.GeoAlitude     = Altitude2. Value - Altitude. Value;

        }

    }

}
