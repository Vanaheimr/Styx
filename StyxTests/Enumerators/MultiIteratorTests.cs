﻿/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Enumerators
{

    [TestFixture]
    public class MultiIteratorTests
    {

        #region testMultiIteratorSimple()

        [Test]
        public void testMultiIteratorSimple()
        {

            var a       = new List<Int32>() { 1, 2    }.GetEnumerator();
            var b       = new List<Int32>() { 3, 4, 5 }.GetEnumerator();
            var c       = new List<Int32>() { 6, 7, 8 }.GetEnumerator();

            var _Enumerator = new MultiEnumerator<Int32>(a, b, c);
            var _Counter    = 0;
            
            while (_Enumerator.MoveNext())
            {
                _Counter++;
                ClassicAssert.AreEqual(_Counter, _Enumerator.Current);
            }

            ClassicAssert.AreEqual(_Counter, 8);

        }

        #endregion

        #region testMultiIteratorNoParameters()

        [Test]
        public void testMultiIteratorNoParameters()
        {

            var _Enumerator = new MultiEnumerator<Int32>();
            var _Counter    = 0;

            while (_Enumerator.MoveNext())
                _Counter++;

            ClassicAssert.AreEqual(0, _Counter);

        }

        #endregion

    }

}
