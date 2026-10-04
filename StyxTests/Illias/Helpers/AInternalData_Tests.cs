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

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// AInternalData tests.
    /// </summary>
    /// <remarks>
    /// Made without a time, an object was made and changed at one moment. The
    /// constructor - and the builder's - read the clock once for when it was
    /// made and once more for when it was last changed, and the two differed
    /// by the ticks between them: a new object had been changed after it was
    /// made, and two new objects alike compared as different.
    /// </remarks>
    [TestFixture]
    public class AInternalData_Tests
    {

        #region Data

        /// <summary>
        /// How many objects are made: enough for two readings of the clock to
        /// fall on different ticks more than once.
        /// </summary>
        private const Int32 times = 10_000;

        private sealed class Thing() : AInternalData(null, null);

        private sealed class ThingBuilder() : AInternalData.Builder(null, null);

        #endregion


        #region AThingMadeWithoutATimeWasChangedWhenItWasMade()

        [Test]
        public void AThingMadeWithoutATimeWasChangedWhenItWasMade()
        {

            for (var i = 0; i < times; i++)
            {

                var thing = new Thing();

                Assert.That(thing.LastChangeDate, Is.EqualTo(thing.Created), $"Made at {thing.Created:O}, changed at {thing.LastChangeDate:O}.");

            }

        }

        #endregion

        #region ABuilderMadeWithoutATimeWasChangedWhenItWasMade()

        [Test]
        public void ABuilderMadeWithoutATimeWasChangedWhenItWasMade()
        {

            for (var i = 0; i < times; i++)
            {

                var builder = new ThingBuilder();

                Assert.That(builder.LastChangeDate, Is.EqualTo(builder.Created), $"Made at {builder.Created:O}, changed at {builder.LastChangeDate:O}.");

            }

        }

        #endregion

        #region AThingMadeWithOneTimeHasItTwice()

        [Test]
        public void AThingMadeWithOneTimeHasItTwice()
        {

            var created = new DateTimeOffset(2026, 10, 4, 12, 0, 0, TimeSpan.Zero);

            var made    = new Thing2(created, null);
            var changed = new Thing2(null,    created);

            Assert.Multiple(() => {
                Assert.That(made.   Created,        Is.EqualTo(created));
                Assert.That(made.   LastChangeDate, Is.EqualTo(created));
                Assert.That(changed.Created,        Is.EqualTo(created));
                Assert.That(changed.LastChangeDate, Is.EqualTo(created));
            });

        }

        private sealed class Thing2(DateTimeOffset? Created, DateTimeOffset? LastChange) : AInternalData(null, null, Created, LastChange);

        #endregion

    }

}
