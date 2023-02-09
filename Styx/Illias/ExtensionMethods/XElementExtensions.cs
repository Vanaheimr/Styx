/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the XElement class.
    /// </summary>
    public static class XElementExtensions
    {

        // XML Elements

        #region ElementOrFail(ParentXElement, XName, Message = null)

        public static XElement ElementOrFail(this XElement  ParentXElement,
                                             XName          XName,
                                             String         Message = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new Exception(Message.IsNotNullOrEmpty() ? Message : "The parent XML element must not be null!");

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                throw new Exception(Message.IsNotNullOrEmpty() ? Message : "The XML element '" + XName.LocalName + "' was not found!");

            return _XElement;

        }

        #endregion

        #region ElementsOrFail(ParentXElement, XName, Message)

        public static IEnumerable<XElement> ElementsOrFail(this XElement  ParentXElement,
                                                           XName          XName,
                                                           String         Message = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new Exception(Message.IsNotNullOrEmpty() ? Message : "The parent XML element must not be null!");

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                throw new Exception(Message.IsNotNullOrEmpty() ? Message : "No XML element '" + XName.LocalName + "' could be found!");

            return _XElements;

        }

        #endregion

        #region IfElementIsNotNullOrEmpty(ParentXElement, XName, ElementAction)

        public static void IfElementIsNotNullOrEmpty(this XElement     ParentXElement,
                                                     XName             XName,
                                                     Action<XElement>  ElementAction)
        {

            #region Initial checks

            if (ParentXElement == null || ElementAction == null)
                return;

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return;

            if (_XElement != null)
                ElementAction(_XElement);

        }

        #endregion


        #region ElementValueOrDefault(this ParentXElement, XName, DefaultValue = null)

        /// <summary>
        /// Return the value of the first (in document order) child element with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent XML element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static String ElementValueOrDefault(this XElement  ParentXElement,
                                                   XName          XName,
                                                   String         DefaultValue = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                return DefaultValue;

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return DefaultValue;

            return _XElement.Value ?? DefaultValue;

        }

        #endregion

        #region ElementValueOrDefault(this ParentXElement, NestedXName, XName, DefaultValue = null)

        /// <summary>
        /// Return the value of the first (in document order) child element with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent XML element.</param>
        /// <param name="NestedXName">The nested System.Xml.Linq.XName.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static String ElementValueOrDefault(this XElement  ParentXElement,
                                                   XName          NestedXName,
                                                   XName          XName,
                                                   String         DefaultValue = null)
        {

            if (ParentXElement == null)
                return DefaultValue;

            var _XElement1 = ParentXElement.Element(XName);

            if (_XElement1 == null)
                return DefaultValue;

            var _XElement2 = _XElement1.Element(NestedXName);

            if (_XElement2 == null)
                return DefaultValue;

            return _XElement2.Value;

        }

        #endregion

        #region ElementValueOrFail(this ParentXElement, XName, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) child element with the
        /// specified System.Xml.Linq.XName or throw an optional exception.
        /// </summary>
        /// <param name="ParentXElement">The XML parent XML element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static String ElementValueOrFail(this XElement  ParentXElement,
                                                XName          XName,
                                                String         ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new Exception(ExceptionMessage);

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
            {

                if (ExceptionMessage.IsNotNullOrEmpty())
                    throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "Missing XML element '" + XName.LocalName + "'!");

                return null;

            }

            return _XElement.Value;

        }

        #endregion

        #region IfValueIsNotNullOrEmpty(ParentXElement, XName, ValueAction)

        public static void IfValueIsNotNullOrEmpty(this XElement   ParentXElement,
                                                   XName           XName,
                                                   Action<String>  ValueAction)
        {

            #region Initial checks

            if (ParentXElement == null || ValueAction == null)
                return;

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return;

            if (_XElement.Value.IsNotNullOrEmpty())
                ValueAction(_XElement.Value);

        }

        #endregion


        #region ElementValues(ParentXElement, XName)

        public static IEnumerable<String> ElementValues(this XElement  ParentXElement,
                                                        XName          XName)
        {

            #region Initial checks

            if (ParentXElement == null)
                return new String[0];

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                return new String[0];

            return _XElements.Select(xelement => xelement.Value);

        }

        #endregion


        // Map XML Element

        #region MapElement          (ParentXElement, XName, Mapper, Default = default(T))

        public static T MapElement<T>(this XElement      ParentXElement,
                                      XName              XName,
                                      Func<XElement, T>  Mapper,
                                      T                  Default = default(T))
        {

            #region Initial checks

            if (ParentXElement == null || Mapper == null)
                return Default;

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return Default;

            return Mapper(_XElement);

        }

        public static T MapElement<T>(this XElement                           ParentXElement,
                                      XName                                   XName,
                                      Func<XElement, OnExceptionDelegate, T>  Mapper,
                                      OnExceptionDelegate                     OnException  = null,
                                      T                                       Default      = default(T))
        {

            #region Initial checks

            if (ParentXElement == null || Mapper == null)
                return Default;

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return Default;

            return Mapper(_XElement, OnException);

        }

        #endregion

        #region MapElementOrFail    (ParentXElement, XName, Mapper, OnException = null, ExceptionMessage = null)

        public static T MapElementOrFail<T>(this XElement                           ParentXElement,
                                            XName                                   XName,
                                            Func<XElement, OnExceptionDelegate, T>  Mapper,
                                            OnExceptionDelegate                     OnException       = null,
                                            String                                  ExceptionMessage  = null)
        {

            if (ParentXElement is null)
                if (OnException != null)
                    OnException(DateTime.UtcNow, ParentXElement, new Exception("The parent XML element must not be null!"));
                else
                    throw new Exception("The parent XML element must not be null!");

            if (Mapper == null)
                if (OnException != null)
                    OnException(DateTime.UtcNow, ParentXElement, new Exception("Mapper delegate must not be null!"));
                else
                    throw new Exception("Mapper delegate must not be null!");

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                if (OnException != null)
                    OnException(DateTime.UtcNow, ParentXElement, new Exception(ExceptionMessage));
                else
                    throw new Exception(ExceptionMessage);

            return Mapper(_XElement, OnException);

        }

        public static Boolean MapElementOrFail<T>(this XElement      ParentXElement,
                                                  XName              XName,
                                                  Func<XElement, T>  Mapper,
                                                  out T?             Element,
                                                  out String?        ErrorResponse)
        {

            if (ParentXElement is null)
            {
                Element        = default;
                ErrorResponse  = "The parent XML element must not be null!";
                return false;
            }

            if (Mapper is null)
            {
                Element        = default;
                ErrorResponse  = "The mapper delegate must not be null!";
                return false;
            }

            var xElement = ParentXElement.Element(XName);

            if (xElement is null)
            {
                Element        = default;
                ErrorResponse  = "The XML element was not found!";
                return false;
            }

            Element        = Mapper(xElement);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region MapElementOrNullable(ParentXElement, XName, Mapper, Default = default(T))

        public static T? MapElementOrNullable<T>(this XElement      ParentXElement,
                                                 XName              XName,
                                                 Func<XElement, T>  Mapper)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null || Mapper == null)
                return new T?();

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return new T?();

            return Mapper(_XElement);

        }

        public static T? MapElementOrNullable<T>(this XElement                           ParentXElement,
                                                 XName                                   XName,
                                                 Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                 OnExceptionDelegate                     OnException = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null || Mapper == null)
                return new T?();

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return new T?();

            return Mapper(_XElement, OnException);

        }

        #endregion


        // Map XML Elements

        #region MapElements      (ParentXElement, XName, Mapper, OnException = null)

        public static IEnumerable<T> MapElements<T>(this XElement        ParentXElement,
                                                    XName                XName,
                                                    Func<XElement, T>    Mapper,
                                                    OnExceptionDelegate  OnException = null)
        {

            #region Initial checks

            if (ParentXElement == null || Mapper == null)
                return new T[0];

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                return new T[0];

            return _XElements.Select(XML   => Mapper(XML)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        public static IEnumerable<T> MapElements<T>(this XElement                           ParentXElement,
                                                    XName                                   XName,
                                                    Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                    OnExceptionDelegate                     OnException = null)
        {

            #region Initial checks

            if (ParentXElement == null || Mapper == null)
                return new T[0];

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                return new T[0];

            return _XElements.Select(XML   => Mapper(XML, OnException)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        #endregion

        #region MapElements      (ParentXElement, XWrapper, XName, Mapper, OnException = null)

        public static IEnumerable<T> MapElements<T>(this XElement        ParentXElement,
                                                    XName                XWrapper,
                                                    XName                XName,
                                                    Func<XElement, T>    Mapper,
                                                    OnExceptionDelegate  OnException = null)
        {

            #region Initial checks

            if (ParentXElement == null || Mapper == null)
                return new T[0];

            #endregion

            var _XElement  = ParentXElement.Element(XWrapper);
            if (_XElement == null)
                return new T[0];

            var _XElements = _XElement.Elements(XName);
            if (_XElements == null || !_XElements.Any())
                return new T[0];

            return _XElements.Select(XML   => Mapper(XML)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        public static IEnumerable<T> MapElements<T>(this XElement                           ParentXElement,
                                                    XName                                   XWrapper,
                                                    XName                                   XName,
                                                    Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                    OnExceptionDelegate                     OnException = null)
        {

            #region Initial checks

            if (ParentXElement == null || Mapper == null)
                return new T[0];

            #endregion

            var _XElement  = ParentXElement.Element(XWrapper);
            if (_XElement == null)
                return new T[0];

            var _XElements = _XElement.Elements(XName);
            if (_XElements == null || !_XElements.Any())
                return new T[0];

            return _XElements.Select(XML   => Mapper(XML, OnException)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        #endregion

        #region MapElementsOrFail(ParentXElement, XName, Mapper, OnException = null, ExceptionMessage = null)

        public static IEnumerable<T> MapElementsOrFail<T>(this XElement                           ParentXElement,
                                                          XName                                   XName,
                                                          Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                          OnExceptionDelegate                     OnException       = null,
                                                          String                                  ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                if (OnException != null)
                    OnException(DateTime.UtcNow, ParentXElement, new Exception("The parent XML element must not be null!"));
                else
                    throw new Exception("The parent XML element must not be null!");

            if (Mapper == null)
                if (OnException != null)
                    OnException(DateTime.UtcNow, ParentXElement, new Exception("Mapper delegate must not be null!"));
                else
                    throw new Exception("Mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
            {

                if (OnException != null)
                {

                    OnException(DateTime.UtcNow,
                                ParentXElement,
                                new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                                  ? ExceptionMessage
                                                  : "The given XML element must not be null!"));

                    return new T[0];

                }

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!");

            }

            return _XElements.Select(XML   => Mapper(XML, OnException)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        #endregion

        #region MapElementsOrFail(ParentXElement, XWrapper, XName, Mapper, OnException = null, ExceptionMessage = null)

        public static IEnumerable<T> MapElementsOrFail<T>(this XElement                           ParentXElement,
                                                          XName                                   XWrapper,
                                                          XName                                   XName,
                                                          Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                          OnExceptionDelegate                     OnException       = null,
                                                          String                                  ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                if (OnException != null)
                    OnException(DateTime.UtcNow, ParentXElement, new Exception("The parent XML element must not be null!"));
                else
                    throw new Exception("The parent XML element must not be null!");

            if (Mapper == null)
                if (OnException != null)
                    OnException(DateTime.UtcNow, ParentXElement, new Exception("Mapper delegate must not be null!"));
                else
                    throw new Exception("Mapper delegate must not be null!");

            #endregion

            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement == null)
                if (OnException != null)
                    OnException(DateTime.UtcNow, ParentXElement, new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!"));
                else
                    throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!");

            var _XElements = _XElement.Elements(XName);

            if (_XElements == null)
                //if (OnException != null)
                //    OnException(DateTime.UtcNow, ParentXElement, new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!"));
                //else
                //    throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!");
                return new T[0];

            var __XElements = _XElements.ToArray();

            if (__XElements.Length == 0)
                //if (OnException != null)
                //    OnException(DateTime.UtcNow, ParentXElement, new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!"));
                //else
                //    throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given array of XML elements must not be empty!");
                return new T[0];


            return _XElements.Select(XML => Mapper(XML, OnException)).Where(v => !EqualityComparer<T>.Default.Equals(v, default(T)));

        }

        #endregion



        // Map XML Value

        #region MapValueOrNull(ParentXElement, XName)

        public static String MapValueOrNull(this XElement  ParentXElement,
                                            XName          XName)
        {

            if (ParentXElement == null || XName == null)
                return null;

            return ParentXElement?.Element(XName)?.Value;

        }

        #endregion

        #region MapValueOrNull(ParentXElement, XName, ValueMapper)

        public static T MapValueOrNull<T>(this XElement    ParentXElement,
                                          XName            XName,
                                          Func<String, T>  ValueMapper)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return default(T);

            if (_XElement.Value.IsNullOrEmpty())
                return default(T);

            return ValueMapper(_XElement.Value);

        }

        #endregion

        #region MapValueOrNullable(ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        public static T? MapValueOrNullable<T>(this XElement    ParentXElement,
                                               XName            XName,
                                               Func<String, T>  ValueMapper,
                                               String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null || _XElement.Value.IsNullOrEmpty())
                return new T?();


            try
            {

                return ValueMapper(_XElement.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region MapValueOrNullable(ParentXElement, XWrapper, XName, ValueMapper)

        public static T? MapValueOrNullable<T>(this XElement    ParentXElement,
                                               XName            XWrapper,
                                               XName            XName,
                                               Func<String, T>  ValueMapper)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XWrapper = ParentXElement.Element(XWrapper);

            if (_XWrapper == null)
                return new T?();


            var _XElement = _XWrapper.Element(XName);

            if (_XElement == null || _XElement.Value.IsNullOrEmpty())
                return new T?();

            return ValueMapper(_XElement.Value);

        }

        #endregion

        #region MapValueOrFail(ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        public static T MapValueOrFail<T>(this XElement    ParentXElement,
                                          XName            XName,
                                          Func<String, T>  ValueMapper,
                                          String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value.IsNullOrEmpty())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");


            try
            {

                return ValueMapper(_XElement.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        public static T MapValueOrFail<T>(this XElement                         ParentXElement,
                                          XName                                 XName,
                                          Func<String, OnExceptionDelegate, T>  ValueMapper,
                                          OnExceptionDelegate                   OnException       = null,
                                          String                                ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");


            try
            {

                return ValueMapper(_XElement.Value, OnException);

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow,
                                    _XElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                    ? ExceptionMessage
                                    : "The XML element '" + XName.LocalName + "' is invalid!",
                                e);

            }

        }

        public static T MapValueOrFail<T>(this XElement                          ParentXElement,
                                          XName                                  XName,
                                          Func<String, OnExceptionDelegate, T?>  ValueMapper,
                                          OnExceptionDelegate                    OnException       = null,
                                          String                                 ExceptionMessage  = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");

            T? Value;

            try
            {

                Value = ValueMapper(_XElement.Value, OnException);

                if (!Value.HasValue)
                    throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "The XML element '" + XName.LocalName + "' is invalid!");

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow,
                                    _XElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                    ? ExceptionMessage
                                    : "The XML element '" + XName.LocalName + "' is invalid!",
                                e);

            }

            return Value.Value;

        }

        #endregion

        #region MapValueOrFail(ParentXElement, XWrapper, XName, ValueMapper, ExceptionMessage = null)

        public static T MapValueOrFail<T>(this XElement                         ParentXElement,
                                          XName                                 XWrapper,
                                          XName                                 XName,
                                          Func<String, OnExceptionDelegate, T>  ValueMapper,
                                          OnExceptionDelegate                   OnException       = null,
                                          String                                ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XWrapper = ParentXElement.Element(XWrapper);

            if (_XWrapper == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML wrapper element '" + XWrapper.LocalName + "'!");


            var _XElement = _XWrapper.Element(XName);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");

            if (_XElement.Value.IsNullOrEmpty())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "' must not be null!");


            try
            {

                return ValueMapper(_XElement.Value, OnException);

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow,
                                    _XElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        public static T MapValueOrFail<T>(this XElement    ParentXElement,
                                          XName            XWrapper,
                                          XName            XName,
                                          Func<String, T>  ValueMapper,
                                          String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XWrapper = ParentXElement.Element(XWrapper);

            if (_XWrapper == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML wrapper element '" + XWrapper.LocalName + "'!");


            var _XElement = _XWrapper.Element(XName);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");

            if (_XElement.Value == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "' must not be null!");


            try
            {

                return ValueMapper(_XElement.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region MapValueOrDefault(ParentXElement, XName, ValueMapper, DefaultValue = default(T))

        public static T MapValueOrDefault<T>(this XElement    ParentXElement,
                                             XName            XName,
                                             Func<String, T>  ValueMapper,
                                             T                DefaultValue = default(T))
        {

            #region Initial checks

            if (ParentXElement == null)
                return DefaultValue;

            if (ValueMapper == null)
                return DefaultValue;

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return DefaultValue;

            if (_XElement.Value.IsNullOrEmpty())
                return DefaultValue;

            return ValueMapper(_XElement.Value);

        }

        #endregion

        #region MapValueOrDefault(ParentXElement, XWrapper, XName, ValueMapper, DefaultValue = default(T))

        public static T MapValueOrDefault<T>(this XElement    ParentXElement,
                                             XName            XWrapper,
                                             XName            XName,
                                             Func<String, T>  ValueMapper,
                                             T                DefaultValue = default(T))
        {

            #region Initial checks

            if (ParentXElement == null)
                return DefaultValue;

            if (ValueMapper == null)
                return DefaultValue;

            #endregion


            var _XWrapper = ParentXElement.Element(XWrapper);

            if (_XWrapper == null)
                return DefaultValue;


            var _XElement = _XWrapper.Element(XName);

            if (_XElement == null || _XElement.Value.IsNullOrEmpty())
                return DefaultValue;


            return ValueMapper(_XElement.Value);

        }

        #endregion

        #region MapBooleanOrFail(ParentXElement, XName, ExceptionMessage = null)

        public static Boolean MapBooleanOrFail(this XElement  ParentXElement,
                                               XName          XName,
                                               String         ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement), "The given XML element must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");


            switch (_XElement.Value)
            {

                case "0":
                case "false":
                    return false;

                case "1":
                case "true":
                    return true;

                default:
                    throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "The value '" + _XElement.Value + "' of the given XML element '" + XName.LocalName + "' must not be null!");

            }

        }

        #endregion

        #region MapNullableBoolean(ParentXElement, XName)

        public static Boolean? MapNullableBoolean(this XElement  ParentXElement,
                                                  XName          XName)
        {

            #region Initial checks

            if (ParentXElement == null)
                return new Boolean?();

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                return new Boolean?();

            if (_XElement.Value.IsNullOrEmpty())
                return new Boolean?();

            switch (_XElement.Value)
            {

                case "0":
                case "false":
                    return new Boolean?(false);

                case "1":
                case "true":
                    return new Boolean?(true);

                default:
                    return new Boolean?();

            }

        }

        #endregion


        #region ParseTimestampOrFail(...)

        public static DateTime ParseTimestampOrFail(this XElement        ParentXElement,
                                                    XName                XName,
                                                    OnExceptionDelegate  OnException       = null,
                                                    String               ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");

            try
            {

                var Timestamp = DateTime.Parse(_XElement.Value);

                if (Timestamp.Kind != DateTimeKind.Utc)
                    Timestamp = Timestamp.ToUniversalTime();

                return Timestamp;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow,
                                    _XElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region ParseTimeSpanOrFail(...)

        public static TimeSpan ParseTimeSpanOrFail(this XElement        ParentXElement,
                                                   XName                XName,
                                                   OnExceptionDelegate  OnException       = null,
                                                   String               ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");

            try
            {

                return TimeSpan.FromSeconds(UInt32.Parse(_XElement.Value));

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow,
                                    _XElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion



        // Map XML Values

        #region MapValues(ParentXElement, XName, ValueMapper)

        public static IEnumerable<T> MapValues<T>(this XElement    ParentXElement,
                                                  XName            XName,
                                                  Func<String, T>  ValueMapper)
        {

            #region Initial checks

            if (ParentXElement == null)
                return new T[0];

            if (ValueMapper == null)
                return new T[0];

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                return new T[0];

            return _XElements.SafeSelect(xelement => ValueMapper(xelement.Value));

        }

        #endregion

        #region MapValues(ParentXElement, XWrapperName, XElementsName, ValueMapper)

        public static IEnumerable<T> MapValues<T>(this XElement    ParentXElement,
                                                  XName            XWrapperName,
                                                  XName            XElementsName,
                                                  Func<String, T>  ValueMapper)
        {

            #region Initial checks

            if (ParentXElement == null)
                return new T[0];

            if (ValueMapper == null)
                return new T[0];

            #endregion

            var _XElement = ParentXElement.Element(XWrapperName);

            if (_XElement == null)
                return new T[0];

            var _XElements = _XElement.Elements(XElementsName);

            if (_XElements == null || !_XElements.Any())
                return new T[0];

            return _XElements.SafeSelect(xelement => ValueMapper(xelement.Value));

        }

        #endregion

        #region MapValuesOrDefault(ParentXElement, XWrapperName, XElementsName, ValueMapper, DefaultValue = default(T))

        public static IEnumerable<T> MapValuesOrDefault<T>(this XElement    ParentXElement,
                                                           XName            XWrapperName,
                                                           XName            XElementsName,
                                                           Func<String, T>  ValueMapper,
                                                           T                DefaultValue = default)
        {

            #region Initial checks

            if (ParentXElement == null)
                return new T[1] { DefaultValue };

            if (ValueMapper == null)
                return new T[1] { DefaultValue };

            #endregion

            var _XElement = ParentXElement.Element(XWrapperName);

            if (_XElement == null)
                return new T[1] { DefaultValue };

            var _XElements = _XElement.Elements(XElementsName);

            if (_XElements == null)
                return new T[1] { DefaultValue };

            var __XElements = _XElements.ToArray();

            if (__XElements.Length == 0)
                return new T[1] { DefaultValue };

            return _XElements.SafeSelect(__XElement => ValueMapper(__XElement.Value));

        }

        #endregion

        #region MapValuesOrFail(ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        public static IEnumerable<T> MapValuesOrFail<T>(this XElement    ParentXElement,
                                                        XName            XName,
                                                        Func<String, T>  ValueMapper,
                                                        String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XName.LocalName + "'!");


            return _XElements.SafeSelect(__XElement => ValueMapper(__XElement.Value));

        }

        public static IEnumerable<T> MapValuesOrFail<T>(this XElement    ParentXElement,
                                                        XName            XWrapper,
                                                        XName            XName,
                                                        Func<String, T>  ValueMapper,
                                                        String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "Missing XML element '" + XWrapper.LocalName + "'!");


            var _XElements = _XElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");


            return _XElements.SafeSelect(__XElement => ValueMapper(__XElement.Value));

        }

        #endregion

        #region MapEnumValues(ParentXElement, XWrapper, XName, ValueMapper, ExceptionMessage = null)

        public static T MapEnumValues<T>(this XElement    ParentXElement,
                                         XName            XName,
                                         Func<String, T>  ValueMapper,
                                         String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                return default(T);


            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return (T) (Object) Value;

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }


        public static T MapEnumValues<T>(this XElement    ParentXElement,
                                         XName            XWrapper,
                                         XName            XName,
                                         Func<String, T>  ValueMapper,
                                         String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement == null)
                return default(T);


            var _XElements = _XElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                return default(T);


            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return (T) (Object) Value;

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }

        #endregion

        #region MapEnumValuesOrNull(ParentXElement, XWrapper, XName, ValueMapper, ExceptionMessage = null)

        public static T? MapEnumValuesOrNull<T>(this XElement    ParentXElement,
                                                XName            XName,
                                                Func<String, T>  ValueMapper,
                                                String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                return new T?();


            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return new T?((T) (Object) Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }


        public static T? MapEnumValuesOrNull<T>(this XElement    ParentXElement,
                                                XName            XWrapper,
                                                XName            XName,
                                                Func<String, T>  ValueMapper,
                                                String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement == null)
                return default(T);


            var _XElements = _XElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                return new T?();


            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return new T?((T) (Object) Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }

        #endregion

        #region MapEnumValuesOrFail(ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        public static T MapEnumValuesOrFail<T>(this XElement    ParentXElement,
                                               XName            XName,
                                               Func<String, T>  ValueMapper,
                                               String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XName.LocalName + "'!");

            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return (T) (Object) Value;

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }

        public static T MapEnumValuesOrFail<T>(this XElement    ParentXElement,
                                               XName            XWrapper,
                                               XName            XName,
                                               Func<String, T>  ValueMapper,
                                               String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "Missing XML element '" + XWrapper.LocalName + "'!");


            var _XElements = _XElement.Elements(XName);

            if (_XElements == null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");

            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return (T) (Object) Value;

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }

        public static T MapEnumValuesOrFail2<T>(this XElement    ParentXElement,
                                                XName            XWrapper,
                                                XName            XName,
                                                Func<String, T>  ValueMapper,
                                                String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _WrapperXElements = ParentXElement.Elements(XWrapper);

            if (_WrapperXElements == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "Missing XML element(s) '" + XWrapper.LocalName + "'!");


            var _XElements = _WrapperXElements.Select(xml => xml.Element(XName));

            if (_XElements == null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");

            try
            {

                Int32 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int32) (Object) _T;

                return (T) Enum.ToObject(typeof(T), Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }


        #endregion


        // Attributes

        #region AttributeValueOrDefault(this ParentXElement, XName, DefaultValue)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static String AttributeValueOrDefault(this XElement  ParentXElement,
                                                     XName          XName,
                                                     String         DefaultValue)
        {

            #region Initial checks

            if (ParentXElement == null)
                return DefaultValue;

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute == null)
                return DefaultValue;

            return _XAttribute.Value;

        }

        #endregion

        #region AttributeValueOrFail(this ParentXElement, XName, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static String AttributeValueOrFail(this XElement  ParentXElement,
                                                  XName          XName,
                                                  String         ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            #endregion


            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The given XML attribute '" + XName.LocalName + "' was not found!");

            return _XAttribute.Value;

        }

        #endregion


        #region MapAttributeValueOrFail(this ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static T MapAttributeValueOrFail<T>(this XElement    ParentXElement,
                                                   XName            XName,
                                                   Func<String, T>  ValueMapper,
                                                   String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML attribute mapper delegate must not be null!");

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute == null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The given XML attribute '" + XName.LocalName + "' was not found!");


            try
            {

                return ValueMapper(_XAttribute.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML attribute '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region MapAttributeValueOrDefault(this ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static T MapAttributeValueOrDefault<T>(this XElement    ParentXElement,
                                                      XName            XName,
                                                      Func<String, T>  ValueMapper,
                                                      String           ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML attribute mapper delegate must not be null!");

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute == null)
                return default(T);


            try
            {

                return ValueMapper(_XAttribute.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML attribute '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region MapAttributeValueOrDefault(this ParentXElement, XName, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static String MapAttributeValueOrDefault(this XElement  ParentXElement,
                                                        XName          XName,
                                                        String         ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute == null)
                return null;

            return _XAttribute.Value;

        }

        #endregion

        #region MapAttributeValueOrNullable(this ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static T? MapAttributeValueOrNullable<T>(this XElement    ParentXElement,
                                                        XName            XName,
                                                        Func<String, T>  ValueMapper,
                                                        String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement == null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML attribute mapper delegate must not be null!");

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute == null)
                return new T?();


            try
            {

                return new T?(ValueMapper(_XAttribute.Value));

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML attribute '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion



        // Stuff...

        #region ToUTF8Bytes(this XML)

        public static Byte[] ToUTF8Bytes(this XElement XML)
        {

            if (XML == null)
                return new Byte[0];

            return Encoding.UTF8.GetBytes(XML.ToString());

        }

        #endregion


    }

}
