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
    /// ConcurrentList tests: a list copied on write, read without a lock.
    /// </summary>
    [TestFixture]
    public class ConcurrentList_Tests
    {

        #region AddedElementsAreReadInTheirOrder()

        [Test]
        public void AddedElementsAreReadInTheirOrder()
        {

            var list = new ConcurrentList<String>("a");

            list.TryAdd("b");
            list.TryAddRange("c", "d");
            list.TryAddRange(new List<String> { "e" });

            Assert.Multiple(() => {
                Assert.That(list.Count,                  Is.EqualTo(5));
                Assert.That(list.ToArray(),              Is.EqualTo(new[] { "a", "b", "c", "d", "e" }));
                Assert.That(list.First(),                Is.EqualTo("a"));
                Assert.That(list.Last(),                 Is.EqualTo("e"));
                Assert.That(list.ElementAt(2),           Is.EqualTo("c"));
                Assert.That(list[3],                     Is.EqualTo("d"));
                Assert.That(list.ElementAtOrDefault(5),  Is.Null);
                Assert.That(list.Contains("b"),          Is.True);
                Assert.That(list.Contains("z"),          Is.False);
                Assert.That(list.Any(),                  Is.True);
            });

        }

        #endregion

        #region AnEmptyListHasNoFirstAndNoLast()

        [Test]
        public void AnEmptyListHasNoFirstAndNoLast()
        {

            var list = new ConcurrentList<String>();

            Assert.Multiple(() => {
                Assert.That(list.Any(),                   Is.False);
                Assert.That(list.FirstOrDefault(),        Is.Null);
                Assert.That(list.LastOrDefault(),         Is.Null);
                Assert.That(() => list.First(),           Throws.InvalidOperationException);
                Assert.That(() => list.Last(),            Throws.InvalidOperationException);
                Assert.That(() => list.ElementAt(0),      Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(() => list[-1],               Throws.TypeOf<ArgumentOutOfRangeException>());
            });

        }

        #endregion

        #region SetReplacesAndClearEmpties()

        [Test]
        public void SetReplacesAndClearEmpties()
        {

            var list = new ConcurrentList<Int32>(1, 2, 3);

            list.TrySet(7);
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 7 }));

            list.TrySet(8, 9);
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 8, 9 }));

            list.TrySet(Enumerable.Range(1, 3));
            Assert.That(list.ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));

            list.Clear();
            Assert.That(list.Count,     Is.Zero);

        }

        #endregion

        #region RemoveTakesTheFirstOccurrenceOfEachElement()

        /// <summary>
        /// As List.Remove: each element given removes one occurrence, the first.
        /// </summary>
        [Test]
        public void RemoveTakesTheFirstOccurrenceOfEachElement()
        {

            var list = new ConcurrentList<String>("a", "b", "a", "c", "a");

            Assert.That(list.TryRemove("a"),  Is.True);
            Assert.That(list.TryRemove("z"),  Is.False);
            Assert.That(list.ToArray(),       Is.EqualTo(new[] { "b", "a", "c", "a" }));

            list.TryRemoveMultiple("a", "c", "z");
            Assert.That(list.ToArray(),       Is.EqualTo(new[] { "b", "a" }));

            list.TryRemoveMultiple(new List<String> { "a", "a" });
            Assert.That(list.ToArray(),       Is.EqualTo(new[] { "b" }));

        }

        #endregion

        #region ToArrayIsACopy()

        [Test]
        public void ToArrayIsACopy()
        {

            var list   = new ConcurrentList<String>("a", "b");
            var array  = list.ToArray();

            array[0] = "z";

            Assert.That(list.First(), Is.EqualTo("a"));

        }

        #endregion

        #region AWalkSeesTheElementsAsTheyWereWhenItBegan()

        [Test]
        public void AWalkSeesTheElementsAsTheyWereWhenItBegan()
        {

            var list  = new ConcurrentList<Int32>(1, 2, 3);
            var seen  = new List<Int32>();

            foreach (var element in list)
            {
                seen.Add(element);
                list.TryAdd(element * 10);
                list.TryRemove(3);
            }

            Assert.Multiple(() => {
                Assert.That(seen,           Is.EqualTo(new[] { 1, 2, 3 }));
                Assert.That(list.ToArray(), Is.EqualTo(new[] { 1, 2, 10, 20, 30 }));
            });

        }

        #endregion

        #region ElementsCanBeReadFromTheListThatTheyAreAddedTo()

        /// <summary>
        /// The elements to add are read before the lock is taken: a list may be given
        /// what it is read to be.
        /// </summary>
        [Test]
        public void ElementsCanBeReadFromTheListThatTheyAreAddedTo()
        {

            var list = new ConcurrentList<Int32>(1, 2);

            list.TryAddRange(list.Select(element => element + 10));
            list.TryRemoveMultiple(list.Where(element => element > 10));

            Assert.That(list.ToArray(), Is.EqualTo(new[] { 1, 2 }));

        }

        #endregion

        #region NoElementIsLostWhenAddedFromManyThreads()

        [Test]
        public async Task NoElementIsLostWhenAddedFromManyThreads()
        {

            const Int32 threads  = 8;
            const Int32 each     = 500;

            var list    = new ConcurrentList<Int32>();
            var start   = new TaskCompletionSource();

            var adders  = Enumerable.Range(0, threads).
                              Select(thread => Task.Run(async () => {
                                  await start.Task;
                                  for (var i = 0; i < each; i++)
                                      list.TryAdd(thread * each + i);
                              })).
                              ToArray();

            // Readers at the same time: each sees a whole list, and never one that shrinks.
            var reader  = Task.Run(async () => {
                              await start.Task;
                              var last = 0;
                              while (last < threads * each)
                              {
                                  var count = list.Count;
                                  Assert.That(count, Is.GreaterThanOrEqualTo(last));
                                  Assert.That(list.ToArray().Length, Is.GreaterThanOrEqualTo(count));
                                  last = count;
                              }
                          });

            start.SetResult();

            await Task.WhenAll(adders);
            await reader;

            Assert.That(list.ToArray().Order(), Is.EqualTo(Enumerable.Range(0, threads * each)));

        }

        #endregion

    }

}
