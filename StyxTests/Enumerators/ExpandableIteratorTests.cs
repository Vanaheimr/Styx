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

using NUnit.Framework;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Enumerators
{

    [TestFixture]
    public class ExpandableIteratorTests
    {

        #region testIdentityPipeNormal()

        [Test]
        public void testIdentityPipeNormal()
        {

            var _Enumerator = new ExpandableEnumerator<Int32>((new List<Int32>() { 1, 2, 3 }).GetEnumerator());

            Assert.That(_Enumerator.MoveNext(), Is.True);
            Assert.That(_Enumerator.Current, Is.EqualTo(1));

            _Enumerator.Add(4);
            Assert.That(_Enumerator.MoveNext(), Is.True);
            Assert.That(_Enumerator.Current, Is.EqualTo(4));
            _Enumerator.Add(5);
            _Enumerator.Add(6);
            Assert.That(_Enumerator.MoveNext(), Is.True);
            Assert.That(_Enumerator.Current, Is.EqualTo(5));
            Assert.That(_Enumerator.MoveNext(), Is.True);
            Assert.That(_Enumerator.Current, Is.EqualTo(6));
            Assert.That(_Enumerator.MoveNext(), Is.True);
            Assert.That(_Enumerator.Current, Is.EqualTo(2));
            Assert.That(_Enumerator.MoveNext(), Is.True);
            Assert.That(_Enumerator.Current, Is.EqualTo(3));
            Assert.That(_Enumerator.MoveNext(), Is.False);

        }

        #endregion

    }

}
