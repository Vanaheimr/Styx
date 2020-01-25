/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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
using System.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Tools for internationalized (I18N) text/string.
    /// </summary>
    public static class I18NTools
    {

        #region SubstringMax(this I18NText, Length)

        /// <summary>
        /// Return a substring of the given maximum length.
        /// </summary>
        /// <param name="I18NText">A text.</param>
        /// <param name="Length">The maximum length of the substring.</param>
        public static I18NString SubstringMax(this I18NString I18NText, Int32 Length)
        {

            if (I18NText == null)
                return null;

            return new I18NString(I18NText.Select(text => new I18NPair(
                                                              text.Language,
                                                              text.Text.Substring(0, Math.Min(text.Text.Length, Length))
                                                          )));

        }

        #endregion

        #region TrimAll(this I18NText)

        /// <summary>
        /// Trim all texts.
        /// </summary>
        /// <param name="I18NText">A text.</param>
        public static I18NString TrimAll(this I18NString I18NText)
        {

            if (I18NText == null)
                return null;

            return new I18NString(I18NText.Select(text => new I18NPair(
                                                              text.Language,
                                                              text.Text.Trim()
                                                          )));

        }

        #endregion

        #region ToHTML(this I18NString)

        /// <summary>
        /// Convert the given internationalized (I18N) text/string to HTML.
        /// </summary>
        /// <param name="I18NString">An internationalized (I18N) text/string.</param>
        public static String ToHTML(this I18NString I18NString)
        {

            return I18NString.
                       Select(v => @"<span class=""I18N_" + v.Language + @""">" + v.Text + "</span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

        #region ToHTML(this I18NString, Prefix, Postfix)

        /// <summary>
        /// Convert the given internationalized (I18N) text/string to HTML.
        /// </summary>
        /// <param name="I18NString">An internationalized (I18N) text/string.</param>
        /// <param name="Prefix">A prefix.</param>
        /// <param name="Postfix">A postfix.</param>
        public static String ToHTML(this I18NString I18NString, String Prefix, String Postfix)
        {

            return I18NString.
                       Select(v => @"<span class=""I18N_" + v.Language + @""">" + Prefix + v.Text + Postfix + "</span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

        #region ToHTMLLink(this I18NString, String URI)

        /// <summary>
        /// Convert the given internationalized (I18N) text/string to a HTML link.
        /// </summary>
        /// <param name="I18NString">An internationalized (I18N) text/string.</param>
        /// <param name="URI">An URI.</param>
        public static String ToHTMLLink(this I18NString I18NString, String URI)
        {

            return I18NString.
                       Select(v => @"<span class=""I18N_" + v.Language + @"""><a href=""" + URI + @"?language=en"">" + v.Text + "</a></span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

        //#region ToJSON(this I18NString)

        ///// <summary>
        ///// Convert the given internationalized (I18N) text/string to JSON.
        ///// </summary>
        ///// <param name="I18NString">An internationalized (I18N) text/string.</param>
        //public static String ToJSON(this I18NString I18NString)
        //{

        //    if (!I18NString.Any())
        //        return "{ }";

        //    return "{" + Environment.NewLine +
        //           I18NString.
        //               Select(v => @"""" + v.Language + @""": """ + v.Text + @"""").
        //               AggregateWith("," + Environment.NewLine) + Environment.NewLine +
        //           "}";

        //}

        //#endregion

    }

}
