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

using System.Text;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An object comparison result.
    /// </summary>
    /// <remarks>
    /// Create a new object comparizion result.
    /// </remarks>
    /// <param name="Added">An enumeration of added properties.</param>
    /// <param name="Updated">An enumeration of updated properties.</param>
    /// <param name="Removed">An enumeration of removed properties.</param>
    public readonly struct ComparisonResult(IEnumerable<ComparisonResult.PropertyWithValue>   Added,
                                            IEnumerable<ComparisonResult.PropertyWithValues>  Updated,
                                            IEnumerable<ComparisonResult.PropertyWithValue>   Removed)
    {

        #region (struct) PropertyWithValue

        public readonly struct PropertyWithValue(String  Name,
                                                 Object  Value)
        {

            public readonly String  Name    { get; } = Name;

            public readonly Object  Value   { get; } = Value;


            public override String  ToString()

                => String.Concat(Name, ": '", Value, "'");

        }

        #endregion

        #region (struct) PropertyWithValues

        public readonly struct PropertyWithValues(String  Name,
                                                  Object  OldValue,
                                                  Object  NewValue)
        {

            public readonly String  Name        { get; } = Name;

            public readonly Object  OldValue    { get; } = OldValue;

            public readonly Object  NewValue    { get; } = NewValue;


            public override String  ToString()

                => String.Concat(Name, ": '", OldValue, "' => '", NewValue, "'");

        }

        #endregion


        #region Properties

        /// <summary>
        /// The enumeration of added properties.
        /// </summary>
        public IEnumerable<PropertyWithValue> Added { get; } = Added;

        /// <summary>
        /// The enumeration of updated properties.
        /// </summary>
        public IEnumerable<PropertyWithValues> Updated { get; } = Updated;

        /// <summary>
        /// The enumeration of removed properties.
        /// </summary>
        public IEnumerable<PropertyWithValue> Removed { get; } = Removed;

        #endregion


        #region ToJSON     (IncludeProperty = null, MaskProperty = null)

        public JObject ToJSON(Func<String, Boolean>?  IncludeProperty   = null,
                              Func<String, Boolean>?  MaskProperty      = null)
        {

            #region Init

            IncludeProperty ??= _ => true;
            MaskProperty    ??= _ => false;

            var json = new JObject();

            #endregion

            #region Added properties

            var added = Added.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).ToArray();

            if (added.Length != 0)
            {

                var addedJSON = new JObject();
                json.Add("added", addedJSON);

                foreach (var property in added)
                {

                    // A masked property still belongs to its own section ("added"),
                    // just with its value hidden - it must not be written to a
                    // (possibly non-existent) "removed" object.
                    if (MaskProperty(property.Name))
                        addedJSON.Add(property.Name, "n/a");

                    else if (property.Value is String text)
                        addedJSON.Add(property.Name, text);

                    else if (property.Value is DateTime timestamp)
                        addedJSON.Add(property.Name, timestamp.ToISO8601());

                    else
                        addedJSON.Add(property.Name, property.Value.ToString());

                }

            }

            #endregion

            #region Updated properties

            var updated = Updated.Where(propertyWithValues => IncludeProperty(propertyWithValues.Name)).ToArray();

            if (updated.Length != 0)
            {

                var updatedJSON = new JObject();
                json.Add("updated", updatedJSON);

                foreach (var property in updated)
                {

                    if (MaskProperty(property.Name))
                        updatedJSON.Add(property.Name, "n/a");

                    else if (property.NewValue is String)
                        updatedJSON.Add(property.Name, new JArray(
                                                           property.NewValue as String,
                                                           property.OldValue as String
                                                       ));

                    else if (property.NewValue is DateTime time1)
                        updatedJSON.Add(property.Name, new JArray(
                                                           time1.ToISO8601(),
                                                           ((DateTime) property.OldValue!).ToISO8601()
                                                       ));

                    else if (property.NewValue is DateTimeOffset time2)
                        updatedJSON.Add(property.Name, new JArray(
                                                           time2.ToISO8601(),
                                                           ((DateTimeOffset) property.OldValue!).ToISO8601()
                                                       ));

                    else
                        updatedJSON.Add(property.Name, new JArray(
                                                           property.NewValue?.ToString() ?? "",
                                                           property.OldValue?.ToString() ?? ""
                                                       ));

                }

            }

            #endregion

            #region Removed properties

            var removed = Removed.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).ToArray();

            if (removed.Length != 0)
            {

                var removedJSON = new JObject();
                json.Add("removed", removedJSON);

                foreach (var property in removed)
                {

                    if (MaskProperty(property.Name))
                        removedJSON.Add(property.Name, "n/a");

                    else if (property.Value is String text)
                        removedJSON.Add(property.Name, text);

                    else if (property.Value is DateTime timestamp)
                        removedJSON.Add(property.Name, timestamp.ToISO8601());

                    else
                        removedJSON.Add(property.Name, property.Value.ToString());

                }

            }

            #endregion

            return json;

        }

        #endregion

        #region ToHTML     (IncludeProperty = null, MaskProperty = null)

        public String ToHTML(Func<String, Boolean>? IncludeProperty   = null,
                             Func<String, Boolean>? MaskProperty      = null)
        {

            #region Init

            IncludeProperty ??= _ => true;
            MaskProperty    ??= _ => false;

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

            if (Added.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

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

            }

            #endregion

            #region Updated properties

            if (Updated.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

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

            }

            #endregion

            #region Removed properties

            if (Removed.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

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

            }

            #endregion

            return sb.ToString();

        }

        #endregion

        #region ToTelegram (IncludeProperty = null, MaskProperty = null)

        public String ToTelegram(Func<String, Boolean>? IncludeProperty   = null,
                                 Func<String, Boolean>? MaskProperty      = null)
        {

            #region Init

            IncludeProperty ??= _ => true;
            MaskProperty    ??= _ => false;

            #endregion

            var sb = new StringBuilder();

            #region Added   properties

            if (Added.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

                sb.AppendLine("\n<b>Added properties</b>");

                foreach (var property in Added.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)))
                {

                    sb.Append("<i>");
                    sb.Append(property.Name);
                    sb.Append("</i>: ");

                    if (!MaskProperty(property.Name))
                        sb.AppendLine(property.Value.ToString());
                    else
                        sb.AppendLine("n/a");

                }

            }

            #endregion

            #region Updated properties

            if (Updated.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

                sb.AppendLine("\n<b>Updated properties</b>");

                foreach (var property in Updated.Where(propertyWithValues => IncludeProperty(propertyWithValues.Name)))
                {

                    sb.Append("<i>");
                    sb.Append(property.Name);
                    sb.Append("</i>: ");

                    if (!MaskProperty(property.Name))
                        sb.Append(property.OldValue);
                    else
                        sb.Append("n/a");

                    sb.Append(@" → ");

                    if (!MaskProperty(property.Name))
                        sb.AppendLine(property.NewValue.ToString());
                    else
                        sb.AppendLine("n/a");

                }

            }

            #endregion

            #region Removed properties

            if (Removed.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

                sb.AppendLine("\n<b>Removed properties</b>");

                foreach (var property in Removed.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)))
                {

                    sb.Append("<i>");
                    sb.Append(property.Name);
                    sb.Append("</i>: ");

                    if (!MaskProperty(property.Name))
                        sb.AppendLine(property.Value.ToString());
                    else
                        sb.AppendLine("n/a");

                }

            }

            #endregion

            return sb.ToString();

        }

        #endregion

        #region ToText     (IncludeProperty = null, MaskProperty = null)

        public String ToText(Func<String, Boolean>? IncludeProperty   = null,
                             Func<String, Boolean>? MaskProperty      = null)
        {

            #region Init

            IncludeProperty ??= _ => true;
            MaskProperty    ??= _ => false;

            #endregion

            #region Calculate colums sizes

            var maxPropertyNameLength     = new Int32[]{
                                                Added.  Any() ? Added.  Max(_ => IncludeProperty(_.Name) ? _.Name.Length : 0) : 0,
                                                Updated.Any() ? Updated.Max(_ => IncludeProperty(_.Name) ? _.Name.Length : 0) : 0,
                                                Removed.Any() ? Removed.Max(_ => IncludeProperty(_.Name) ? _.Name.Length : 0) : 0
                                            }.Max();

            var maxUpdatedOldValueLength  = Updated.Any() ? Updated.Max(_ => IncludeProperty(_.Name) ? _.OldValue.ToString()?.Length ?? 0 : 0) : 0;

            var sb = new StringBuilder();

            #endregion

            #region Added properties

            if (Added.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

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

            }

            #endregion

            #region Updated properties

            if (Updated.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

                sb.AppendLine(@"Updated properties");
                sb.AppendLine(@"------------------");
                sb.AppendLine();

                foreach (var property in Updated.Where(propertyWithValues => IncludeProperty(propertyWithValues.Name)))
                {

                    sb.Append((property.Name + ": ").PadRight(maxPropertyNameLength + 2));

                    if (!MaskProperty(property.Name))
                    {
                        sb.Append((property.OldValue.ToString() ?? "").PadRight(maxUpdatedOldValueLength + 3));
                        sb.AppendLine(property.NewValue.ToString());
                    }
                    else
                    {
                        sb.Append("n/a".PadRight(maxUpdatedOldValueLength + 3));
                        sb.AppendLine("n/a");
                    }

                }

                sb.AppendLine();

            }

            #endregion

            #region Removed properties

            if (Removed.Where(propertyWithValue => IncludeProperty(propertyWithValue.Name)).SafeAny())
            {

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

            }

            #endregion

            return sb.ToString();

        }

        #endregion

    }

}
