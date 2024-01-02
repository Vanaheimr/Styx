/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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
    public class StdDevTests
    {

        #region testStdDevPipeSingle()

        [Test]
        public void testStdDevPipeSingle()
        {

            var _Pipe = new StdDevPipe(new List<Double>() { 505.0 });
            while (_Pipe.MoveNext())
            {
                var s = _Pipe.Current;
            }

            ClassicAssert.AreEqual(  0.0, _Pipe.SideEffect1);
            ClassicAssert.AreEqual(505.0, _Pipe.SideEffect2);

        }

        #endregion

        #region testStdDevPipe()

        [Test]
        public void testStdDevPipe()
        {

            var _Pipe = new StdDevPipe(new List<Double>() { 505.0, 500.0, 495.0, 505.0 });
            while (_Pipe.MoveNext())
            {
                var s = _Pipe.Current;
            }

            ClassicAssert.AreEqual( 22.92, Math.Round(_Pipe.SideEffect1, 2));
            ClassicAssert.AreEqual(501.25, _Pipe.SideEffect2);

        }

        #endregion

    }

}
