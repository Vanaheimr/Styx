/*
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

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.SideeffectPipes
{

    [TestFixture]
    public class CountPipeTests
    {

        #region testCountPipeNormal()

        [Test]
        public void testCountPipeNormal()
        {

            var _List = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };
            var _Pipe = new CountPipe<String>(_List);

            var _Counter = 0UL;
            while (_Pipe.MoveNext())
            {
                var s = _Pipe.Current;
                ClassicAssert.IsTrue(s.Equals("marko") || s.Equals("antonio") || s.Equals("rodriguez") || s.Equals("was") || s.Equals("here") || s.Equals("."));
                _Counter++;
                ClassicAssert.AreEqual(_Counter, _Pipe.SideEffect);
            }

            ClassicAssert.AreEqual(6UL, _Pipe.SideEffect);

        }

        #endregion

        #region testCountPipeZero()

        [Test]
        public void testCountPipeZero()
        {

            var _List = new List<String>();
            var _Pipe = new CountPipe<String>(_List);

            while (_Pipe.MoveNext())
            { }

            ClassicAssert.AreEqual(0UL, _Pipe.SideEffect);

        }

        #endregion

    }

}
