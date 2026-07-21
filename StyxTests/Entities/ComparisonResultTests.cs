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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Entities
{

    /// <summary>
    /// Unit tests for ComparisonResult.ToJSON(...).
    /// </summary>
    [TestFixture]
    public class ComparisonResultTests
    {

        #region ToJSON_UnmaskedProperties_RenderInTheirOwnSections()

        [Test]
        public void ToJSON_UnmaskedProperties_RenderInTheirOwnSections()
        {

            var result = new ComparisonResult(
                             [ new ComparisonResult.PropertyWithValue ("colour", "blue") ],
                             [ new ComparisonResult.PropertyWithValues("size", "S", "L") ],
                             [ new ComparisonResult.PropertyWithValue ("legacy", "old") ]
                         );

            var json = result.ToJSON();

            Assert.That(json["added"]?  ["colour"]?.Value<String>(), Is.EqualTo("blue"));
            Assert.That(json["updated"]?["size"]?.Type,              Is.EqualTo(JTokenType.Array));
            Assert.That(json["removed"]?["legacy"]?.Value<String>(), Is.EqualTo("old"));

        }

        #endregion

        #region ToJSON_MaskedAddedProperty_StaysInAddedSection()

        // Regression test: a masked *added* property used to be written to a
        // "removed" object that is never created in the added-loop, throwing a
        // NullReferenceException. It must instead land in "added" as "n/a".
        [Test]
        public void ToJSON_MaskedAddedProperty_StaysInAddedSection()
        {

            var result = new ComparisonResult(
                             [ new ComparisonResult.PropertyWithValue("secret", "top-secret") ],
                             [],
                             []
                         );

            JObject? json = null;
            Assert.That(() => json = result.ToJSON(MaskProperty: _ => true),
                        Throws.Nothing);

            Assert.That(json!["added"]?["secret"]?.Value<String>(), Is.EqualTo("n/a"));
            Assert.That(json!.ContainsKey("removed"),               Is.False);   // no phantom section

        }

        #endregion

        #region ToJSON_MaskedUpdatedProperty_StaysInUpdatedSection()

        // Same latent NullReferenceException as the added case.
        [Test]
        public void ToJSON_MaskedUpdatedProperty_StaysInUpdatedSection()
        {

            var result = new ComparisonResult(
                             [],
                             [ new ComparisonResult.PropertyWithValues("password", "old", "new") ],
                             []
                         );

            JObject? json = null;
            Assert.That(() => json = result.ToJSON(MaskProperty: _ => true),
                        Throws.Nothing);

            Assert.That(json!["updated"]?["password"]?.Value<String>(), Is.EqualTo("n/a"));
            Assert.That(json!.ContainsKey("removed"),                   Is.False);

        }

        #endregion

        #region ToJSON_IncludePropertyFilter_ExcludesUnwanted()

        [Test]
        public void ToJSON_IncludePropertyFilter_ExcludesUnwanted()
        {

            var result = new ComparisonResult(
                             [ new ComparisonResult.PropertyWithValue("keep", "yes"),
                               new ComparisonResult.PropertyWithValue("drop", "no") ],
                             [],
                             []
                         );

            var json = result.ToJSON(IncludeProperty: name => name == "keep");

            Assert.That(json["added"]?["keep"]?.Value<String>(), Is.EqualTo("yes"));
            Assert.That(json["added"]?["drop"],                  Is.Null);

        }

        #endregion

    }

}
