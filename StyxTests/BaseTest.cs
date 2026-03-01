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

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests
{

    public static class BaseTest
    {

        #region GenerateUUIDs (Number)

        public static IEnumerable<String> GenerateUUIDs(UInt32 Number)
        {

            var uuids = new List<String>();

            for (var i = 0; i < Number; i++)
                uuids.Add(Guid.NewGuid().ToString());

            return uuids;

        }

        #endregion

        #region GenerateUUIDs (Prefix, Number)

        public static IEnumerable<String> GenerateUUIDs(String  Prefix,
                                                        UInt32  Number)
        {

            var uuids = new List<String>();

            for (var i = 0; i < Number; i++)
                uuids.Add(Prefix + Guid.NewGuid().ToString());

            return uuids;

        }

        #endregion


        #region PrintCollection<T>  (Collection)

        public static void PrintCollection<T>(ICollection<T> Collection)
        {
            foreach (var item in Collection)
                Console.WriteLine(item);
        }

        #endregion

        #region PrintIEnumerator<T> (IEnumerator)

        public static void PrintIEnumerator<T>(IEnumerator<T> IEnumerator)
        {
            while (IEnumerator.MoveNext())
                Console.WriteLine(IEnumerator.Current);
        }

        #endregion


        #region Count<T>(IEnumerator)

        public static UInt64 Count<T>(IEnumerator<T> IEnumerator)
        {

            UInt64 counter = 0;

            while (IEnumerator.MoveNext())
                counter++;

            return counter;

        }

        #endregion


        #region AsList<T>(Item, Times)

        public static List<T> AsList<T>(T Item, UInt64 Times)
        {

            var list = new List<T>();

            for (var i = 0UL; i < Times; i++)
                list.Add(Item);

            return list;

        }

        #endregion

        #region PrintPerformance(Name, Events, EventName, RunTime)

        public static void PrintPerformance(String           Name,
                                            Nullable<Int32>  Events,
                                            String           EventName,
                                            TimeSpan         RunTime)
        {
            
            if (Events is not null)
                Console.WriteLine("\t" + Name + ": " + Events + " " + EventName + " in " + RunTime.TotalMilliseconds + "ms");

            else
                Console.WriteLine("\t" + Name + ": " + EventName + " in " + RunTime.TotalMilliseconds + "ms");

        }

        #endregion

    }

}
