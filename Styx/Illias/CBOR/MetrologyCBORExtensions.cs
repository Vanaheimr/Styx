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

using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// CBOR writer/reader conveniences for metrological values (tag 44252)
    /// and bridges between metrological values and the strongly-typed
    /// metrology structs like Watt, Ampere and WattHour.
    /// Note: The prefixable base unit of mass is the Gram, therefore
    /// a Kilogram bridges to (Gram, SIPrefix.Kilo).
    /// </summary>
    public static class MetrologyCBORExtensions
    {

        #region WriteMetrologicalValue  (this CBORWriter, MetrologicalValue, SymbolicUnit = false)

        /// <summary>
        /// Write the given metrological value (tag 44252).
        /// </summary>
        /// <param name="CBORWriter">A CBOR writer.</param>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="SymbolicUnit">Whether to write the unit as its symbol text instead of its numeric identification.</param>
        public static void WriteMetrologicalValue(this CBORWriter    CBORWriter,
                                                  MetrologicalValue  MetrologicalValue,
                                                  Boolean            SymbolicUnit = false)
        {
            MetrologicalValue.WriteTo(CBORWriter, SymbolicUnit);
        }

        #endregion

        #region TryReadMetrologicalValue(this CBORReader, out MetrologicalValue, out ErrorResponse)

        /// <summary>
        /// Try to read a metrological value (tag 44252).
        /// Note: Malformed CBOR data throws a CBORException;
        /// false is only returned for semantic errors like
        /// unknown units or non-canonical SI prefixes.
        /// </summary>
        /// <param name="CBORReader">A CBOR reader.</param>
        /// <param name="MetrologicalValue">The parsed metrological value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryReadMetrologicalValue(this ref CBORReader               CBORReader,
                                                       out MetrologicalValue             MetrologicalValue,
                                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var cbor = CBORValue.ReadFrom(ref CBORReader);

            return MetrologicalValue.TryParse(cbor,
                                              out MetrologicalValue,
                                              out ErrorResponse);

        }

        #endregion


        #region AsMetrologicalValue(this Watt/..., Prefix = null)

        /// <summary>
        /// Return this watt value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Watt">A watt value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Watt Watt, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Watt.Value, UnitOfMeasure.Watt), Prefix);


        /// <summary>
        /// Return this watt-hour value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="WattHour">A watt-hour value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this WattHour WattHour, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(WattHour.Value, UnitOfMeasure.WattHour), Prefix);


        /// <summary>
        /// Return this ampere value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Ampere">An ampere value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Ampere Ampere, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Ampere.Value, UnitOfMeasure.Ampere), Prefix);


        /// <summary>
        /// Return this volt value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Volt">A volt value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Volt Volt, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Volt.Value, UnitOfMeasure.Volt), Prefix);


        /// <summary>
        /// Return this hertz value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Hertz">A hertz value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Hertz Hertz, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Hertz.Value, UnitOfMeasure.Hertz), Prefix);


        /// <summary>
        /// Return this volt-ampere value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="VoltAmpere">A volt-ampere value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this VoltAmpere VoltAmpere, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(VoltAmpere.Value, UnitOfMeasure.VoltAmpere), Prefix);


        /// <summary>
        /// Return this volt-ampere-reactive value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="VoltAmpereReactive">A volt-ampere-reactive value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this VoltAmpereReactive VoltAmpereReactive, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(VoltAmpereReactive.Value, UnitOfMeasure.VoltAmpereReactive), Prefix);


        /// <summary>
        /// Return this meter value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Meter">A meter value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Meter Meter, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Meter.m, UnitOfMeasure.Meter), Prefix);


        /// <summary>
        /// Return this kelvin value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Kelvin">A kelvin value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Kelvin Kelvin, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Kelvin.Value, UnitOfMeasure.Kelvin), Prefix);


        /// <summary>
        /// Return this Celsius value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Celsius">A Celsius value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Celsius Celsius, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Celsius.Value, UnitOfMeasure.Celsius), Prefix);


        /// <summary>
        /// Return this tonne value as a metrological value, optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Tonne">A tonne value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Tonne Tonne, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Tonne.Value, UnitOfMeasure.Tonne), Prefix);


        /// <summary>
        /// Return this kilogram value as a metrological value (Gram, SIPrefix.Kilo),
        /// optionally converted into the given SI prefix.
        /// </summary>
        /// <param name="Kilogram">A kilogram value.</param>
        /// <param name="Prefix">An optional SI prefix to convert into.</param>
        public static MetrologicalValue AsMetrologicalValue(this Kilogram Kilogram, SIPrefix? Prefix = null)
            => Convert(new MetrologicalValue(Kilogram.Value, UnitOfMeasure.Gram, SIPrefix.Kilo), Prefix);

        #endregion

        #region TryTo...(this MetrologicalValue, out ...)

        /// <summary>
        /// Try to convert this metrological value into a watt value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Watt">The watt value.</param>
        public static Boolean TryToWatt(this MetrologicalValue MetrologicalValue, out Watt Watt)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Watt &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                Watt = Watt.FromW(baseValue.Value);
                return true;
            }

            Watt = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a watt-hour value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="WattHour">The watt-hour value.</param>
        public static Boolean TryToWattHour(this MetrologicalValue MetrologicalValue, out WattHour WattHour)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.WattHour &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                WattHour = WattHour.FromWh(baseValue.Value);
                return true;
            }

            WattHour = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into an ampere value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Ampere">The ampere value.</param>
        public static Boolean TryToAmpere(this MetrologicalValue MetrologicalValue, out Ampere Ampere)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Ampere &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                Ampere = Ampere.FromA(baseValue.Value);
                return true;
            }

            Ampere = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a volt value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Volt">The volt value.</param>
        public static Boolean TryToVolt(this MetrologicalValue MetrologicalValue, out Volt Volt)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Volt &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                Volt = Volt.FromV(baseValue.Value);
                return true;
            }

            Volt = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a hertz value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Hertz">The hertz value.</param>
        public static Boolean TryToHertz(this MetrologicalValue MetrologicalValue, out Hertz Hertz)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Hertz &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                Hertz = Hertz.FromHz(baseValue.Value);
                return true;
            }

            Hertz = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a volt-ampere value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="VoltAmpere">The volt-ampere value.</param>
        public static Boolean TryToVoltAmpere(this MetrologicalValue MetrologicalValue, out VoltAmpere VoltAmpere)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.VoltAmpere &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                VoltAmpere = VoltAmpere.FromVA(baseValue.Value);
                return true;
            }

            VoltAmpere = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a volt-ampere-reactive value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="VoltAmpereReactive">The volt-ampere-reactive value.</param>
        public static Boolean TryToVoltAmpereReactive(this MetrologicalValue MetrologicalValue, out VoltAmpereReactive VoltAmpereReactive)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.VoltAmpereReactive &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                VoltAmpereReactive = VoltAmpereReactive.FromVAr(baseValue.Value);
                return true;
            }

            VoltAmpereReactive = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a meter value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Meter">The meter value.</param>
        public static Boolean TryToMeter(this MetrologicalValue MetrologicalValue, out Meter Meter)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Meter &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                Meter = Meter.From_m(baseValue.Value);
                return true;
            }

            Meter = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a kelvin value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Kelvin">The kelvin value.</param>
        public static Boolean TryToKelvin(this MetrologicalValue MetrologicalValue, out Kelvin Kelvin)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Kelvin &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                Kelvin = Kelvin.FromK(baseValue.Value);
                return true;
            }

            Kelvin = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a Celsius value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Celsius">The Celsius value.</param>
        public static Boolean TryToCelsius(this MetrologicalValue MetrologicalValue, out Celsius Celsius)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Celsius &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                Celsius = Celsius.FromC(baseValue.Value);
                return true;
            }

            Celsius = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a tonne value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Tonne">The tonne value.</param>
        public static Boolean TryToTonne(this MetrologicalValue MetrologicalValue, out Tonne Tonne)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Tonne &&
                MetrologicalValue.TryToBaseUnit(out var baseValue))
            {
                Tonne = Tonne.FromT(baseValue.Value);
                return true;
            }

            Tonne = default;
            return false;

        }


        /// <summary>
        /// Try to convert this metrological value into a kilogram value.
        /// The unit of measure must be the Gram: A metrological value of
        /// e.g. (5000, Gram) or (5, Gram, Kilo) becomes 5 kg.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Kilogram">The kilogram value.</param>
        public static Boolean TryToKilogram(this MetrologicalValue MetrologicalValue, out Kilogram Kilogram)
        {

            if (MetrologicalValue.Unit == UnitOfMeasure.Gram)
            {

                try
                {

                    Kilogram = Kilogram.FromKG(MetrologicalValue.ConvertTo(SIPrefix.Kilo).Value);

                    return true;

                }
                catch (Exception e) when (e is OverflowException || e is ArgumentOutOfRangeException)
                { }

            }

            Kilogram = default;
            return false;

        }

        #endregion


        #region (private static) Convert(MetrologicalValue, Prefix)

        private static MetrologicalValue Convert(MetrologicalValue  MetrologicalValue,
                                                 SIPrefix?          Prefix)

            => Prefix.HasValue && Prefix.Value != MetrologicalValue.Prefix
                   ? MetrologicalValue.ConvertTo(Prefix.Value)
                   : MetrologicalValue;

        #endregion

    }

}
