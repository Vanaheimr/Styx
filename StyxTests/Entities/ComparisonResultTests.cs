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


        // -------------------------------------------------------------------
        // Updated-section value semantics.
        //
        // The "updated" section emits, per property, a two-element JSON array.
        // Two contracts are pinned here:
        //
        //   (a) The array must be ordered [OldValue, NewValue] - the same
        //       old -> new direction as the PropertyWithValues(Name, Old, New)
        //       constructor, its ToString ("old => new"), and every text
        //       renderer (ToHTML / ToText / ToTelegram). ToJSON currently
        //       emits [New, Old] - the inverted order - so these tests are RED
        //       until the value order in ComparisonResult.ToJSON is corrected.
        //
        //   (b) OldValue and NewValue may carry *different* runtime types (a
        //       property whose type changed, or a null <-> value transition).
        //       ToJSON must not assume OldValue shares NewValue's type. It
        //       currently branches on NewValue's type and then hard-casts
        //       OldValue to the same type ((DateTime) / (DateTimeOffset)),
        //       throwing InvalidCastException on any mismatch.
        // -------------------------------------------------------------------

        #region ToJSON_UpdatedStrings_AreOrderedOldThenNew()

        [Test]
        public void ToJSON_UpdatedStrings_AreOrderedOldThenNew()
        {

            var result = new ComparisonResult(
                             [],
                             [ new ComparisonResult.PropertyWithValues("size", "S", "L") ],
                             []
                         );

            var array = result.ToJSON()["updated"]?["size"] as JArray;

            Assert.That(array,                    Is.Not.Null);
            Assert.That(array!.Count,             Is.EqualTo(2));
            Assert.That(array[0].Value<String>(), Is.EqualTo("S"));   // old first
            Assert.That(array[1].Value<String>(), Is.EqualTo("L"));   // new second

        }

        #endregion

        #region ToJSON_UpdatedInt32_AreOrderedOldThenNew()

        [Test]
        public void ToJSON_UpdatedInt32_AreOrderedOldThenNew()
        {

            // The else-branch: neither String nor DateTime(Offset).
            var result = new ComparisonResult(
                             [],
                             [ new ComparisonResult.PropertyWithValues("count", 3, 5) ],
                             []
                         );

            var array = result.ToJSON()["updated"]?["count"] as JArray;

            Assert.That(array,                    Is.Not.Null);
            Assert.That(array![0].Value<String>(), Is.EqualTo("3"));   // old first
            Assert.That(array[1].Value<String>(),  Is.EqualTo("5"));   // new second

        }

        #endregion

        #region ToJSON_UpdatedDateTimes_AreOrderedOldThenNew()

        [Test]
        public void ToJSON_UpdatedDateTimes_AreOrderedOldThenNew()
        {

            var oldTime = new DateTime(2024, 1,  2,  3, 4,  5, DateTimeKind.Utc);
            var newTime = new DateTime(2025, 6,  7,  8, 9, 10, DateTimeKind.Utc);

            var result = new ComparisonResult(
                             [],
                             [ new ComparisonResult.PropertyWithValues("when", oldTime, newTime) ],
                             []
                         );

            var array = result.ToJSON()["updated"]?["when"] as JArray;

            Assert.That(array,                    Is.Not.Null);
            Assert.That(array![0].Value<String>(), Is.EqualTo(oldTime.ToISO8601()));   // old first
            Assert.That(array[1].Value<String>(),  Is.EqualTo(newTime.ToISO8601()));   // new second

        }

        #endregion

        #region ToJSON_UpdatedDateTimeOffsets_AreOrderedOldThenNew()

        [Test]
        public void ToJSON_UpdatedDateTimeOffsets_AreOrderedOldThenNew()
        {

            var oldTime = new DateTimeOffset(2024, 1, 2, 3, 4,  5, TimeSpan.Zero);
            var newTime = new DateTimeOffset(2025, 6, 7, 8, 9, 10, TimeSpan.Zero);

            var result = new ComparisonResult(
                             [],
                             [ new ComparisonResult.PropertyWithValues("when", oldTime, newTime) ],
                             []
                         );

            var array = result.ToJSON()["updated"]?["when"] as JArray;

            Assert.That(array,                    Is.Not.Null);
            Assert.That(array![0].Value<String>(), Is.EqualTo(oldTime.ToISO8601()));   // old first
            Assert.That(array[1].Value<String>(),  Is.EqualTo(newTime.ToISO8601()));   // new second

        }

        #endregion

        #region ToJSON_UpdatedNewDateTime_OldString_DoesNotThrow()

        // NewValue is a DateTime, OldValue is a String: ToJSON branches on the
        // DateTime new value and then does ((DateTime) property.OldValue!),
        // which throws InvalidCastException on the String old value.
        [Test]
        public void ToJSON_UpdatedNewDateTime_OldString_DoesNotThrow()
        {

            var newTime = new DateTime(2025, 6, 7, 8, 9, 10, DateTimeKind.Utc);

            var result = new ComparisonResult(
                             [],
                             [ new ComparisonResult.PropertyWithValues("when", "never", newTime) ],
                             []
                         );

            Assert.That(() => result.ToJSON(), Throws.Nothing);

        }

        #endregion

        #region ToJSON_UpdatedNewDateTimeOffset_OldString_DoesNotThrow()

        // Same mismatch for the DateTimeOffset branch:
        // ((DateTimeOffset) property.OldValue!) on a String old value.
        [Test]
        public void ToJSON_UpdatedNewDateTimeOffset_OldString_DoesNotThrow()
        {

            var newTime = new DateTimeOffset(2025, 6, 7, 8, 9, 10, TimeSpan.Zero);

            var result = new ComparisonResult(
                             [],
                             [ new ComparisonResult.PropertyWithValues("when", "never", newTime) ],
                             []
                         );

            Assert.That(() => result.ToJSON(), Throws.Nothing);

        }

        #endregion

    }

}
