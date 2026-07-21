/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Aegir;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Aegir
{

    /// <summary>
    /// Unit tests for the GeoJSON feature parsers.
    /// GeoJSON coordinates are ordered [longitude, latitude].
    /// </summary>
    [TestFixture]
    public class GeoJSONFeatureTests
    {

        #region PointFeature_Parse_ReadsCoordinatesIdAndProperties()

        [Test]
        public void PointFeature_Parse_ReadsCoordinatesIdAndProperties()
        {

            var feature = PointFeature.Parse(JObject.Parse("""
                {
                    "type":       "Feature",
                    "id":         "berlin",
                    "geometry":   { "type": "Point", "coordinates": [ 13.4, 52.5 ] },
                    "properties": { "name": "Berlin", "population": 3600000 }
                }
                """));

            // [lon, lat] => Longitude 13.4, Latitude 52.5
            Assert.That(feature.GeoCoordinate.Latitude. Value, Is.EqualTo(52.5));
            Assert.That(feature.GeoCoordinate.Longitude.Value, Is.EqualTo(13.4));

            Assert.That(feature.Id,                        Is.EqualTo("berlin"));
            Assert.That(feature.GetProperty("name"),       Is.EqualTo("Berlin"));
            Assert.That(feature.GetProperty("population"), Is.EqualTo(3600000));

        }

        #endregion

        #region PointFeature_Parse_MissingId_YieldsNullId()

        [Test]
        public void PointFeature_Parse_MissingId_YieldsNullId()
        {

            // The GeoJSON "id" member is optional.
            var feature = PointFeature.Parse(JObject.Parse("""
                { "type": "Feature", "geometry": { "type": "Point", "coordinates": [ 13.4, 52.5 ] } }
                """));

            Assert.That(feature.Id, Is.Null);

        }

        #endregion

        #region PointFeature_Parse_MissingProperties_YieldsEmptyProperties()

        [Test]
        public void PointFeature_Parse_MissingProperties_YieldsEmptyProperties()
        {

            // No "properties" object => ParseProperties(null) => empty.
            var feature = PointFeature.Parse(JObject.Parse("""
                { "type": "Feature", "geometry": { "type": "Point", "coordinates": [ 13.4, 52.5 ] } }
                """));

            Assert.That(feature.GetProperty("anything"), Is.Null);

        }

        #endregion

        #region PointFeature_Parse_InvalidGeoJSON_Throws()

        [Test]
        public void PointFeature_Parse_InvalidGeoJSON_Throws()
        {

            // Missing "coordinates".
            Assert.That(
                () => PointFeature.Parse(JObject.Parse("""
                    { "type": "Feature", "geometry": { "type": "Point" } }
                    """)),
                Throws.TypeOf<ArgumentException>()
            );

        }

        #endregion

        #region LineStringFeature_Parse_ReadsAllCoordinates()

        [Test]
        public void LineStringFeature_Parse_ReadsAllCoordinates()
        {

            var feature = LineStringFeature.Parse(JObject.Parse("""
                {
                    "type":     "Feature",
                    "geometry": { "type": "LineString",
                                  "coordinates": [ [ 13.4, 52.5 ], [ 13.5, 52.6 ], [ 13.6, 52.7 ] ] }
                }
                """));

            var coordinates = feature.GeoCoordinates.ToList();

            Assert.That(coordinates.Count,                    Is.EqualTo(3));
            Assert.That(coordinates[0].Latitude. Value,       Is.EqualTo(52.5));
            Assert.That(coordinates[0].Longitude.Value,       Is.EqualTo(13.4));
            Assert.That(coordinates[2].Latitude. Value,       Is.EqualTo(52.7));

        }

        #endregion

        #region PolygonFeature_Parse_ReadsNestedRings()

        [Test]
        public void PolygonFeature_Parse_ReadsNestedRings()
        {

            var feature = PolygonFeature.Parse(JObject.Parse("""
                {
                    "type":     "Feature",
                    "geometry": { "type": "Polygon",
                                  "coordinates": [ [ [ 13.4, 52.5 ], [ 13.5, 52.5 ],
                                                    [ 13.5, 52.6 ], [ 13.4, 52.5 ] ] ] }
                }
                """));

            var rings = feature.Rings.Select(ring => ring.ToList()).ToList();

            Assert.That(rings.Count,                     Is.EqualTo(1));
            Assert.That(rings[0].Count,                  Is.EqualTo(4));
            Assert.That(rings[0][0].Latitude. Value,     Is.EqualTo(52.5));
            Assert.That(rings[0][0].Longitude.Value,     Is.EqualTo(13.4));

        }

        #endregion

        #region GeoJSON_Parse_FeatureCollection_ParsesContainedFeatures()

        [Test]
        public void GeoJSON_Parse_FeatureCollection_ParsesContainedFeatures()
        {

            var geoJSON = GeoJSON.Parse(JObject.Parse("""
                {
                    "type": "FeatureCollection",
                    "features": [
                        { "type": "Feature",
                          "geometry": { "type": "Point", "coordinates": [ 13.4, 52.5 ] },
                          "properties": {} }
                    ]
                }
                """));

            var features = geoJSON.Features.ToList();

            Assert.That(features.Count,           Is.EqualTo(1));
            Assert.That(features[0],              Is.TypeOf<PointFeature>());

        }

        #endregion

    }

}
