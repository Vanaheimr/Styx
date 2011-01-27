﻿/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET
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

using NUnit.Framework;

using de.ahzf.Pipes;

#endregion

namespace TestApplication
{

    public class Program
    {

        public static void Main(String[] myArgs)
        {

            var _SingleEnumerator = new SingleEnumerator<UInt64>(123);
            var _List             = new List<UInt64>();
                _List.Add(123);
            var _ListEnumerator   = _List.GetEnumerator();

            var _List1 = new List<UInt64>();
            var _List2 = new List<UInt64>();

            while (_SingleEnumerator.MoveNext())
                _List1.Add(_SingleEnumerator.Current);

            while (_ListEnumerator.MoveNext())
                _List2.Add(_ListEnumerator.Current);




            var names = new List<String>() { "marko", "josh", "peter" };

            IPipe<String, String> pipe = new IdentityPipe<String>();
            pipe.SetStarts(names);

            var counter = 0UL;
            while (pipe.MoveNext())
            {
                counter++;
                String name = pipe.Current;
                Assert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
            }

            Assert.AreEqual(counter, 3UL);
            pipe.SetStarts(names);
            counter = 0UL;

            foreach (var name in pipe)
            {
                Assert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
                counter++;
            }

            Assert.AreEqual(counter, 3UL);

        }

    }

}
