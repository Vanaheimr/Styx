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
using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The RFC 8949, Section 8 diagnostic notation of CBOR values.
    /// All formatting is culture-invariant. Floating-point numbers use
    /// the shortest round-trippable representation with a guaranteed
    /// decimal point or exponent, a lowercase 'e' and no leading zeros
    /// within the exponent, e.g. "1.0e+300" and "5.960464477539063e-8".
    /// Bignums (tag 2/3) are rendered as their integer value.
    /// </summary>
    internal static class CBORDiagnosticNotation
    {

        #region (internal static) Format(Value)

        internal static String Format(in CBORValue Value)
        {

            var stringBuilder = new StringBuilder();

            Append(stringBuilder, Value);

            return stringBuilder.ToString();

        }

        #endregion

        #region (private static) Append(StringBuilder, Value)

        private static void Append(StringBuilder  StringBuilder,
                                   in CBORValue   Value)
        {

            switch (Value.Kind)
            {

                case CBORValueKind.Null:
                    StringBuilder.Append("null");
                    break;

                case CBORValueKind.Undefined:
                    StringBuilder.Append("undefined");
                    break;

                case CBORValueKind.Boolean:
                    StringBuilder.Append(Value.AsBoolean() ? "true" : "false");
                    break;

                case CBORValueKind.UnsignedInteger:
                    StringBuilder.Append(Value.AsUInt64().ToString(CultureInfo.InvariantCulture));
                    break;

                case CBORValueKind.NegativeInteger:
                    StringBuilder.Append(Value.AsInt128().ToString(null, CultureInfo.InvariantCulture));
                    break;

                case CBORValueKind.ByteString:
                    StringBuilder.Append("h'").
                                  Append(Convert.ToHexStringLower(Value.AsBytes())).
                                  Append('\'');
                    break;

                case CBORValueKind.TextString:
                    AppendQuoted(StringBuilder, Value.AsText());
                    break;

                case CBORValueKind.Array:
                {

                    StringBuilder.Append('[');

                    var first = true;

                    foreach (var item in Value.AsArray())
                    {

                        if (!first)
                            StringBuilder.Append(", ");

                        Append(StringBuilder, item);
                        first = false;

                    }

                    StringBuilder.Append(']');
                    break;

                }

                case CBORValueKind.Map:
                {

                    StringBuilder.Append('{');

                    var first = true;

                    foreach (var entry in Value.AsMap())
                    {

                        if (!first)
                            StringBuilder.Append(", ");

                        Append(StringBuilder, entry.Key);
                        StringBuilder.Append(": ");
                        Append(StringBuilder, entry.Value);
                        first = false;

                    }

                    StringBuilder.Append('}');
                    break;

                }

                case CBORValueKind.Tagged:

                    // Bignums are rendered as their integer value...
                    if (Value.HasTag(CBORTag.UnsignedBignum) ||
                        Value.HasTag(CBORTag.NegativeBignum))
                    {

                        try
                        {
                            StringBuilder.Append(Value.AsBigInteger().ToString(CultureInfo.InvariantCulture));
                            break;
                        }
                        catch (CBORException)
                        { }

                    }

                    StringBuilder.Append(Value.Tag.Value.ToString(CultureInfo.InvariantCulture)).
                                  Append('(');

                    Append(StringBuilder, Value.UntaggedValue);

                    StringBuilder.Append(')');
                    break;

                case CBORValueKind.SimpleValue:
                    StringBuilder.Append(Value.AsSimpleValue().ToString());
                    break;

                default:  // HalfFloat, SingleFloat, DoubleFloat
                    AppendFloatingPoint(StringBuilder, Value.AsDouble());
                    break;

            }

        }

        #endregion

        #region (private static) AppendQuoted(StringBuilder, Text)

        private static void AppendQuoted(StringBuilder  StringBuilder,
                                         String         Text)
        {

            StringBuilder.Append('"');

            foreach (var character in Text)
            {

                switch (character)
                {

                    case '"':
                        StringBuilder.Append("\\\"");
                        break;

                    case '\\':
                        StringBuilder.Append("\\\\");
                        break;

                    case '\b':
                        StringBuilder.Append("\\b");
                        break;

                    case '\f':
                        StringBuilder.Append("\\f");
                        break;

                    case '\n':
                        StringBuilder.Append("\\n");
                        break;

                    case '\r':
                        StringBuilder.Append("\\r");
                        break;

                    case '\t':
                        StringBuilder.Append("\\t");
                        break;

                    default:

                        if (character < 0x20)
                            StringBuilder.Append("\\u").
                                          Append(((UInt16) character).ToString("x4", CultureInfo.InvariantCulture));
                        else
                            StringBuilder.Append(character);

                        break;

                }

            }

            StringBuilder.Append('"');

        }

        #endregion

        #region (private static) AppendFloatingPoint(StringBuilder, Value)

        private static void AppendFloatingPoint(StringBuilder  StringBuilder,
                                                Double         Value)
        {

            if (Double.IsNaN(Value))
            {
                StringBuilder.Append("NaN");
                return;
            }

            if (Double.IsPositiveInfinity(Value))
            {
                StringBuilder.Append("Infinity");
                return;
            }

            if (Double.IsNegativeInfinity(Value))
            {
                StringBuilder.Append("-Infinity");
                return;
            }

            var text           = Value.ToString("R", CultureInfo.InvariantCulture);
            var exponentIndex  = text.IndexOf('E');

            if (exponentIndex < 0)
            {

                StringBuilder.Append(text);

                if (!text.Contains('.'))
                    StringBuilder.Append(".0");

                return;

            }

            var mantissa  = text[..exponentIndex];
            var exponent  = text[(exponentIndex + 1)..];
            var negative  = exponent.StartsWith('-');
            var digits    = exponent.TrimStart('+', '-').TrimStart('0');

            if (digits.Length == 0)
                digits = "0";

            StringBuilder.Append(mantissa);

            if (!mantissa.Contains('.'))
                StringBuilder.Append(".0");

            StringBuilder.Append('e').
                          Append(negative ? '-' : '+').
                          Append(digits);

        }

        #endregion

    }

}
