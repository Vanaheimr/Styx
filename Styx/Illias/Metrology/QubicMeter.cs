/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Numerics;
using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for m³.
    /// </summary>
    public static class QubicMeterExtensions
    {

        /// <summary>
        /// The sum of the given m³ values.
        /// </summary>
        /// <param name="QubicMeters">An enumeration of m³ values.</param>
        public static QubicMeter Sum(this IEnumerable<QubicMeter> QubicMeters)
        {

            var sum = QubicMeter.Zero;

            foreach (var qubicMeter in QubicMeters)
                sum = sum + qubicMeter;

            return sum;

        }

    }


    /// <summary>
    /// A m³.
    /// </summary>
    public readonly struct QubicMeter : IEquatable<QubicMeter>,
                                        IComparable<QubicMeter>,
                                        IComparable,
                                        IAdditionOperators   <QubicMeter, QubicMeter, QubicMeter>,
                                        ISubtractionOperators<QubicMeter, QubicMeter, QubicMeter>,
                                        IMultiplyOperators   <QubicMeter, Decimal,    QubicMeter>,
                                        IDivisionOperators   <QubicMeter, Decimal,    QubicMeter>
    {

        #region Properties

        /// <summary>
        /// The zero value of a QubicMeter.
        /// </summary>
        public static readonly QubicMeter Zero = new (0m);

        /// <summary>
        /// The value of the QubicMeter.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the QubicMeter.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


        /// <summary>
        /// The value as KiloQubicMeters.
        /// </summary>
        public Decimal  KM
            => Value / 1000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new QubicMeter based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a m³.</param>
        private QubicMeter(Decimal Value)
        {

            this.Value = Value >= 0
                             ? Value
                             : 0;

        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as a m³.
        /// </summary>
        /// <param name="Text">A text representation of a m³.</param>
        public static QubicMeter Parse(String Text)
        {

            if (TryParse(Text, out var qubicMeter))
                return qubicMeter;

            throw new ArgumentException($"Invalid text representation of a m³: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseQM     (Text)

        /// <summary>
        /// Parse the given string as a m³.
        /// </summary>
        /// <param name="Text">A text representation of a m³.</param>
        public static QubicMeter ParseQM(String Text)
        {

            if (TryParseQM(Text, out var qubicMeter))
                return qubicMeter;

            throw new ArgumentException($"Invalid text representation of a m³: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseQKM    (Text)

        /// <summary>
        /// Parse the given string as a km³.
        /// </summary>
        /// <param name="Text">A text representation of a km³.</param>
        public static QubicMeter ParseQKM(String Text)
        {

            if (TryParseQKM(Text, out var qubicMeter))
                return qubicMeter;

            throw new ArgumentException($"Invalid text representation of a km³: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseQM     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter ParseQM(Decimal  Number,
                                   Int32?   Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            throw new ArgumentException($"Invalid numeric representation of a m³: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter ParseQM(Double  Number,
                                   Int32?  Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            throw new ArgumentException($"Invalid numeric representation of a m³: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter ParseQM(Byte    Number,
                                   Int32?  Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            throw new ArgumentException($"Invalid numeric representation of a m³: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseQKM    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a km³.
        /// </summary>
        /// <param name="Number">A numeric representation of a km³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter ParseQKM(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            throw new ArgumentException($"Invalid numeric representation of a km³: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km³.
        /// </summary>
        /// <param name="Number">A numeric representation of a km³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter ParseQKM(Double  Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            throw new ArgumentException($"Invalid numeric representation of a km³: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km³.
        /// </summary>
        /// <param name="Number">A numeric representation of a km³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter ParseQKM(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            throw new ArgumentException($"Invalid numeric representation of a km³: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as a m³.
        /// </summary>
        /// <param name="Text">A text representation of a m³.</param>
        public static QubicMeter? TryParse(String Text)
        {

            if (TryParse(Text, out var qubicMeter))
                return qubicMeter;

            return null;

        }

        #endregion

        #region (static) TryParseQM  (Text)

        /// <summary>
        /// Try to parse the given text as a m³.
        /// </summary>
        /// <param name="Text">A text representation of a m³.</param>
        public static QubicMeter? TryParseQM(String Text)
        {

            if (TryParseQM(Text, out var qubicMeter))
                return qubicMeter;

            return null;

        }

        #endregion

        #region (static) TryParseQKM (Text)

        /// <summary>
        /// Try to parse the given text as a km³.
        /// </summary>
        /// <param name="Text">A text representation of a km³.</param>
        public static QubicMeter? TryParseQKM(String Text)
        {

            if (TryParseQKM(Text, out var qubicMeter))
                return qubicMeter;

            return null;

        }

        #endregion


        #region (static) TryParseQM  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter? TryParseQM(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter? TryParseQM(Double  Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter? TryParseQM(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            return null;

        }

        #endregion

        #region (static) TryParseQKM (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a km³.
        /// </summary>
        /// <param name="Number">A numeric representation of a km³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter? TryParseQKM(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a km³.
        /// </summary>
        /// <param name="Number">A numeric representation of a km³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter? TryParseQKM(Double  Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseQKM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static QubicMeter? TryParse(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryParseQM(Number, out var qubicMeter, Exponent))
                return qubicMeter;

            return null;

        }

        #endregion


        #region (static) TryParse    (Text,   out QubicMeter)

        /// <summary>
        /// Parse the given string as a m³.
        /// </summary>
        /// <param name="Text">A text representation of a m³.</param>
        /// <param name="QubicMeter">The parsed m³.</param>
        public static Boolean TryParse(String Text, out QubicMeter QubicMeter)
        {

            QubicMeter = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            Text = Text.Trim();

            var factor = 1m;

            if      (Text.EndsWith("qkm", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000m;
                Text    = Text[..^3].TrimEnd();
            }

            else if (Text.EndsWith("qm",  StringComparison.OrdinalIgnoreCase))
            {
                Text    = Text[..^2].TrimEnd();
            }

            if (Decimal.TryParse(Text,
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {

                QubicMeter = new QubicMeter(value * factor);

                return true;

            }

            return false;

        }

        #endregion

        #region (static) TryParseQM  (Text,   out QubicMeter)

        /// <summary>
        /// Parse the given string as a m³.
        /// </summary>
        /// <param name="Text">A text representation of a m³.</param>
        /// <param name="QubicMeter">The parsed m³.</param>
        public static Boolean TryParseQM(String Text, out QubicMeter QubicMeter)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    QubicMeter = new QubicMeter(value);

                    return true;

                }

            }
            catch
            { }

            QubicMeter = default;
            return false;

        }

        #endregion

        #region (static) TryParseQKM (Text,   out QubicMeter)

        /// <summary>
        /// Parse the given string as a km³.
        /// </summary>
        /// <param name="Text">A text representation of a km³.</param>
        /// <param name="QubicMeter">The parsed km³.</param>
        public static Boolean TryParseQKM(String Text, out QubicMeter QubicMeter)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    QubicMeter = new QubicMeter(1000000 * value);

                    return true;

                }

            }
            catch
            { }

            QubicMeter = default;
            return false;

        }

        #endregion


        #region (static) TryParseQM  (Number, out QubicMeter, Exponent = null)

        /// <summary>
        /// Parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="QubicMeter">The parsed m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseQM(Byte       Number,
                                        out QubicMeter  QubicMeter,
                                        Int32?     Exponent = null)
        {

            QubicMeter = new QubicMeter(Number * MathHelpers.Pow10(Exponent ?? 0));

            if (Number < 0)
                return false;

            return true;

        }


        /// <summary>
        /// Parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="QubicMeter">The parsed m³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseQM(Double     Number,
                                        out QubicMeter  QubicMeter,
                                        Int32?     Exponent = null)
        {

            try
            {

                QubicMeter = new QubicMeter((Decimal) (Number * Math.Pow(10, Exponent ?? 0)));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                QubicMeter = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a m³.
        /// </summary>
        /// <param name="Number">A numeric representation of a m³.</param>
        /// <param name="QubicMeter">The parsed m³.</param>
        public static Boolean TryParseQM(Decimal    Number,
                                        out QubicMeter  QubicMeter,
                                        Int32?     Exponent = null)
        {

            try
            {

                QubicMeter = new QubicMeter(Number * MathHelpers.Pow10(Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                QubicMeter = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseQKM (Number, out QubicMeter, Exponent = null)

        /// <summary>
        /// Parse the given number as a km³.
        /// </summary>
        /// <param name="Number">A numeric representation of a km³.</param>
        /// <param name="QubicMeter">The parsed km³.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseQKM(Byte       Number,
                                         out QubicMeter  QubicMeter,
                                         Int32?     Exponent = null)
        {

            QubicMeter = new QubicMeter(1000000 * Number * MathHelpers.Pow10(Exponent ?? 0));

            if (Number < 0)
                return false;

            return true;

        }


        /// <summary>
        /// Parse the given number as a km³.
        /// </summary>
        /// <param name="Number">A numeric representation of a km³.</param>
        /// <param name="QubicMeter">The parsed km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseQKM(Double     Number,
                                         out QubicMeter  QubicMeter,
                                         Int32?     Exponent = null)
        {

            try
            {

                QubicMeter = new QubicMeter(1000 * (Decimal) (Number * Math.Pow(10, Exponent ?? 0)));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                QubicMeter = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a km³.
        /// </summary>
        /// <param name="Number">A numeric representation of a km³.</param>
        /// <param name="QubicMeter">The parsed km.</param>
        public static Boolean TryParseQKM(Decimal    Number,
                                         out QubicMeter  QubicMeter,
                                         Int32?     Exponent = null)
        {

            try
            {

                QubicMeter = new QubicMeter(1000 * Number * MathHelpers.Pow10(Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                QubicMeter = default;
                return false;
            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (QubicMeter1, QubicMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="QubicMeter1">A m³.</param>
        /// <param name="QubicMeter2">Another m³.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (QubicMeter QubicMeter1,
                                           QubicMeter QubicMeter2)

            => QubicMeter1.Equals(QubicMeter2);

        #endregion

        #region Operator != (QubicMeter1, QubicMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="QubicMeter1">A m³.</param>
        /// <param name="QubicMeter2">Another m³.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (QubicMeter QubicMeter1,
                                           QubicMeter QubicMeter2)

            => !QubicMeter1.Equals(QubicMeter2);

        #endregion

        #region Operator <  (QubicMeter1, QubicMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="QubicMeter1">A m³.</param>
        /// <param name="QubicMeter2">Another m³.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (QubicMeter QubicMeter1,
                                          QubicMeter QubicMeter2)

            => QubicMeter1.CompareTo(QubicMeter2) < 0;

        #endregion

        #region Operator <= (QubicMeter1, QubicMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="QubicMeter1">A m³.</param>
        /// <param name="QubicMeter2">Another m³.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (QubicMeter QubicMeter1,
                                           QubicMeter QubicMeter2)

            => QubicMeter1.CompareTo(QubicMeter2) <= 0;

        #endregion

        #region Operator >  (QubicMeter1, QubicMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="QubicMeter1">A m³.</param>
        /// <param name="QubicMeter2">Another m³.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (QubicMeter QubicMeter1,
                                          QubicMeter QubicMeter2)

            => QubicMeter1.CompareTo(QubicMeter2) > 0;

        #endregion

        #region Operator >= (QubicMeter1, QubicMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="QubicMeter1">A m³.</param>
        /// <param name="QubicMeter2">Another m³.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (QubicMeter QubicMeter1,
                                           QubicMeter QubicMeter2)

            => QubicMeter1.CompareTo(QubicMeter2) >= 0;

        #endregion

        #region Operator +  (QubicMeter1, QubicMeter2)

        /// <summary>
        /// Accumulates two QubicMeters.
        /// </summary>
        /// <param name="QubicMeter1">A m³.</param>
        /// <param name="QubicMeter2">Another m³.</param>
        public static QubicMeter operator + (QubicMeter QubicMeter1,
                                             QubicMeter QubicMeter2)

            => new (QubicMeter1.Value + QubicMeter2.Value);

        #endregion

        #region Operator -  (QubicMeter1, QubicMeter2)

        /// <summary>
        /// Substracts two QubicMeters.
        /// </summary>
        /// <param name="QubicMeter1">A m³.</param>
        /// <param name="QubicMeter2">Another m³.</param>
        public static QubicMeter operator - (QubicMeter QubicMeter1,
                                             QubicMeter QubicMeter2)

            => new (QubicMeter1.Value - QubicMeter2.Value);

        #endregion


        #region Operator *  (QubicMeter,  Scalar)

        /// <summary>
        /// Multiplies a QubicMeter with a scalar.
        /// </summary>
        /// <param name="QubicMeter">A QubicMeter value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static QubicMeter operator * (QubicMeter  QubicMeter,
                                             Decimal     Scalar)

            => new (QubicMeter.Value * Scalar);

        #endregion

        #region Operator /  (QubicMeter,  Scalar)

        /// <summary>
        /// Divides a QubicMeter with a scalar.
        /// </summary>
        /// <param name="QubicMeter">A QubicMeter value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static QubicMeter operator / (QubicMeter  QubicMeter,
                                             Decimal     Scalar)

            => new (QubicMeter.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<QubicMeter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two m³s.
        /// </summary>
        /// <param name="Object">A m³ to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is QubicMeter qubicMeter
                   ? CompareTo(qubicMeter)
                   : throw new ArgumentException("The given object is not a m³!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(QubicMeter)

        /// <summary>
        /// Compares two m³s.
        /// </summary>
        /// <param name="QubicMeter">A m³ to compare with.</param>
        public Int32 CompareTo(QubicMeter QubicMeter)

            => Value.CompareTo(QubicMeter.Value);

        #endregion

        #endregion

        #region IEquatable<QubicMeter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two m³s for equality.
        /// </summary>
        /// <param name="Object">A m³ to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is QubicMeter qubicMeter &&
                   Equals(qubicMeter);

        #endregion

        #region Equals(QubicMeter)

        /// <summary>
        /// Compares two m³s for equality.
        /// </summary>
        /// <param name="QubicMeter">A m³ to compare with.</param>
        public Boolean Equals(QubicMeter QubicMeter)

            => Value.Equals(QubicMeter.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Value.ToString(CultureInfo.InvariantCulture)} m";

        #endregion

    }

}
