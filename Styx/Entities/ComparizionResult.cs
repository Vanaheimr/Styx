/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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
using System.Text;
using System.Linq;

using Newtonsoft.Json.Linq;

using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An object comparizion result.
    /// </summary>
    public readonly struct ComparizionResult
    {

        #region (struct) PropertyWithValue

        public readonly struct PropertyWithValue
        {

            public readonly String  Name     { get; }

            public readonly Object  Value    { get; }

            public PropertyWithValue(String  Name,
                                     Object  Value)
            {

                this.Name   = Name;
                this.Value  = Value;

            }

            public override String ToString()

                => String.Concat(Name, ": '", Value, "'");

        }

        #endregion

        #region (struct) PropertyWithValues

        public readonly struct PropertyWithValues
        {

            public readonly String  Name        { get; }

            public readonly Object  OldValue    { get; }

            public readonly Object  NewValue    { get; }


            public PropertyWithValues(String  Name,
                                      Object  OldValue,
                                      Object  NewValue)
            {

                this.Name      = Name;
                this.OldValue  = OldValue;
                this.NewValue  = NewValue;

            }

            public override String ToString()

                => String.Concat(Name, ": '", OldValue, "' => '", NewValue, "'");

        }

        #endregion


        #region Properties

        /// <summary>
        /// The enumeration of added properties.
        /// </summary>
        public IEnumerable<PropertyWithValue>   Added      { get; }

        /// <summary>
        /// The enumeration of updated properties.
        /// </summary>
        public IEnumerable<PropertyWithValues>  Updated    { get; }

        /// <summary>
        /// The enumeration of removed properties.
        /// </summary>
        public IEnumerable<PropertyWithValue>   Removed    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new object comparizion result.
        /// </summary>
        /// <param name="Added">An enumeration of added properties.</param>
        /// <param name="Updated">An enumeration of updated properties.</param>
        /// <param name="Removed">An enumeration of removed properties.</param>
        public ComparizionResult(IEnumerable<PropertyWithValue>   Added,
                                 IEnumerable<PropertyWithValues>  Updated,
                                 IEnumerable<PropertyWithValue>   Removed)
        {

            this.Added    = Added;
            this.Updated  = Updated;
            this.Removed  = Removed;

        }

        #endregion


        #region ToJSON(IncludeProperty = null, MaskProperty = null)

        public JObject ToJSON(Func<String, Boolean>  IncludeProperty   = null,
                              Func<String, Boolean>  MaskProperty      = null)
        {

            #region Init

            if (IncludeProperty is null)
                IncludeProperty = _ => true;

            if (MaskProperty is null)
                MaskProperty = _ => false;

            var JSON = new JObject();

            #endregion

            #region Added properties

            foreach (var property in Added.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)))
            {

                if (!JSON.ContainsKey("added"))
                    JSON.Add("added", new JObject());

                if (MaskProperty(property.Name))
                    (JSON["removed"] as JObject).Add(property.Name, "n/a");

                else
                {

                    if      (property.Value is String   text)
                        (JSON["added"] as JObject).Add(property.Name, text);

                    else if (property.Value is DateTime timestamp)
                        (JSON["added"] as JObject).Add(property.Name, timestamp.  ToIso8601());

                    else
                        (JSON["added"] as JObject).Add(property.Name, property.Value.ToString());

                }

            }

            #endregion

            #region Updated properties

            foreach (var property in Updated.Where(propertyWithValues => IncludeProperty(propertyWithValues.Name)))
            {

                if (!JSON.ContainsKey("updated"))
                    JSON.Add("updated", new JObject());

                if (MaskProperty(property.Name))
                    (JSON["removed"] as JObject).Add(property.Name, "n/a");

                else
                {

                    if      (property.NewValue is String)
                        (JSON["updated"] as JObject).Add(property.Name, new JArray(property.NewValue as String, property.OldValue as String));

                    else if (property.NewValue is DateTime)
                        (JSON["updated"] as JObject).Add(property.Name, new JArray(((DateTime) property.NewValue).ToIso8601(), ((DateTime) property.OldValue).ToIso8601()));

                    else
                        (JSON["updated"] as JObject).Add(property.Name, new JArray(property.NewValue.ToString(), property.OldValue.ToString()));

                }

            }

            #endregion

            #region Removed properties

            foreach (var property in Removed.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)))
            {

                if (!JSON.ContainsKey("removed"))
                    JSON.Add("removed", new JObject());

                if (MaskProperty(property.Name))
                    (JSON["removed"] as JObject).Add(property.Name, "n/a");

                else
                {

                    if      (property.Value is String   text)
                        (JSON["removed"] as JObject).Add(property.Name, text);

                    else if (property.Value is DateTime timestamp)
                        (JSON["removed"] as JObject).Add(property.Name, timestamp.    ToIso8601());

                    else
                        (JSON["removed"] as JObject).Add(property.Name, property.Value.ToString());

                }

            }

            #endregion

            return JSON;

        }

        #endregion

        #region ToHTML(IncludeProperty = null, MaskProperty = null)

        public String ToHTML(Func<String, Boolean>  IncludeProperty   = null,
                             Func<String, Boolean>  MaskProperty      = null)
        {

            #region Init

            if (IncludeProperty is null)
                IncludeProperty = _ => true;

            if (MaskProperty is null)
                MaskProperty = _ => false;

            #endregion

            #region Add CSS

            var sb = new StringBuilder();

            sb.AppendLine(@"<style>");
            sb.AppendLine(@" .headline {                            font-weight: bold;                padding: 2px 15px 2px 5px; }");
            sb.AppendLine(@" .key      { background-color: #DDDDDD; font-weight: bold; font-size: 80% padding: 2px 15px 2px 5px; }");
            sb.AppendLine(@" .value    { background-color: #F0F0F0;                                   padding: 2px 15px 2px 5px; }");
            sb.AppendLine(@" .space    { height: 3px; }");
            sb.AppendLine(@"</style>");

            sb.AppendLine();

            #endregion

            #region Added properties

            sb.AppendLine(@"<div class=""headline"">Added properties</div>");
            sb.AppendLine(@"<table id=""added"" class=""properties"">");

            foreach (var property in Added.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)))
            {

                sb.Append(@"<tr><td class=""key"">");
                sb.Append(property.Name);
                sb.Append(@"</td><td class=""value"">");

                if (!MaskProperty(property.Name))
                    sb.Append(property.Value);
                else
                    sb.Append("n/a");

                sb.Append(@"</td></tr>");
                sb.Append(Environment.NewLine);

            }

            sb.AppendLine("</table>");

            #endregion

            #region Updated properties

            sb.AppendLine(@"<div class=""headline"">Updated properties</div>");
            sb.AppendLine(@"<table id=""updated"" class=""properties"">");

            foreach (var property in Updated.Where(propertyWithValues => IncludeProperty(propertyWithValues.Name)))
            {

                sb.Append(@"<tr><td class=""key"">");
                sb.Append(property.Name);
                sb.Append(@"</td><td class=""value"">");

                if (!MaskProperty(property.Name))
                    sb.Append(property.OldValue);
                else
                    sb.Append("n/a");


                sb.Append(@"</td><td class=""value"">");

                if (!MaskProperty(property.Name))
                    sb.Append(property.NewValue);
                else
                    sb.Append("n/a");

                sb.Append(@"</td></tr>");
                sb.Append(Environment.NewLine);

            }

            sb.AppendLine("</table>");

            #endregion

            #region Removed properties

            sb.AppendLine(@"<div class=""headline"">Removed properties</div>");
            sb.AppendLine(@"<table id=""removed"" class=""properties"">");

            foreach (var property in Removed.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)))
            {

                sb.Append(@"<tr><td class=""key"">");
                sb.Append(property.Name);
                sb.Append(@"</td><td class=""value"">");

                if (!MaskProperty(property.Name))
                    sb.Append(property.Value);
                else
                    sb.Append("n/a");

                sb.Append(@"</td></tr>");
                sb.Append(Environment.NewLine);

            }

            sb.AppendLine("</table>");

            #endregion

            return sb.ToString();

        }

        #endregion

        #region ToText(IncludeProperty = null, MaskProperty = null)

        public String ToText(Func<String, Boolean>  IncludeProperty   = null,
                             Func<String, Boolean>  MaskProperty      = null)
        {

            #region Init

            if (IncludeProperty is null)
                IncludeProperty = _ => true;

            if (MaskProperty is null)
                MaskProperty = _ => false;

            #endregion

            #region Calculate colums sizes

            var maxPropertyNameLength     = new Int32[]{
                                                Added.  Max(_ => IncludeProperty(_.Name) ? _.Name.Length : 0),
                                                Updated.Max(_ => IncludeProperty(_.Name) ? _.Name.Length : 0),
                                                Removed.Max(_ => IncludeProperty(_.Name) ? _.Name.Length : 0)
                                            }.Max();

            var maxUpdatedOldValueLength  = Updated.Max(_ => IncludeProperty(_.Name) ? _.OldValue.ToString().Length : 0);

            var sb = new StringBuilder();

            #endregion

            #region Added properties

            sb.AppendLine(@"Added properties");
            sb.AppendLine(@"----------------");
            sb.AppendLine();

            foreach (var property in Added.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)))
            {

                sb.Append((property.Name + ": ").PadRight(maxPropertyNameLength + 2));

                if (!MaskProperty(property.Name))
                    sb.AppendLine(property.Value.ToString());
                else
                    sb.AppendLine("n/a");

            }

            sb.AppendLine();

            #endregion

            #region Updated properties

            sb.AppendLine(@"Updated properties");
            sb.AppendLine(@"------------------");
            sb.AppendLine();

            foreach (var property in Updated.Where(propertyWithValues => IncludeProperty(propertyWithValues.Name)))
            {

                sb.Append((property.Name + ": ").PadRight(maxPropertyNameLength + 2));

                if (!MaskProperty(property.Name))
                {
                    sb.Append(property.OldValue.ToString().PadRight(maxUpdatedOldValueLength + 3));
                    sb.AppendLine(property.NewValue.ToString());
                }
                else
                {
                    sb.Append("n/a".PadRight(maxUpdatedOldValueLength + 3));
                    sb.AppendLine("n/a");
                }

            }

            sb.AppendLine();

            #endregion

            #region Removed properties

            sb.AppendLine(@"Removed properties");
            sb.AppendLine(@"------------------");
            sb.AppendLine();

            foreach (var property in Removed.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)))
            {

                sb.Append((property.Name + ": ").PadRight(maxPropertyNameLength + 2));

                if (!MaskProperty(property.Name))
                    sb.AppendLine(property.Value.ToString());
                else
                    sb.AppendLine("n/a");

            }

            sb.AppendLine();

            #endregion

            return sb.ToString();

        }

        #endregion


    }

}
