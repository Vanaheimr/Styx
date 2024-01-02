/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the StringBuilder class.
    /// </summary>
    public static class StringBuilderExtensions
    {

        #region AppendCSV  (this StringBuilder, IEnumerable)

        public static StringBuilder AppendCSV(this StringBuilder   StringBuilder,
                                              IEnumerable<String>  IEnumerable)

            => StringBuilder.Append(IEnumerable.CSVAggregate());

        #endregion

        #region AppendCSV  (this StringBuilder, Prefix, IEnumerable, Suffix)

        public static StringBuilder AppendCSV(this StringBuilder   StringBuilder,
                                              String               Prefix,
                                              IEnumerable<String>  IEnumerable,
                                              String               Suffix)

            => StringBuilder.Append(IEnumerable.CSVAggregate(Prefix, Suffix));

        #endregion

        #region ToUTF8Bytes(this StringBuilder)

        public static Byte[] ToUTF8Bytes(this StringBuilder StringBuilder)

            => Encoding.UTF8.GetBytes(StringBuilder.ToString());

        #endregion

    }


    public static class NewLine
    {

        public static String Concat(params String[] Lines)
        {

            var sb = new StringBuilder();

            foreach (var line in Lines)
                sb.AppendLine(line);

            return sb.ToString();

        }

    }


}
