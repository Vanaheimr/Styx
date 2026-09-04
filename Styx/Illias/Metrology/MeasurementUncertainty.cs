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

using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The probability distribution assigned to a measurement uncertainty,
    /// as used by the GUM (JCGM 100:2008) when evaluating and propagating it.
    /// </summary>
    public enum UncertaintyDistribution : Byte
    {

        /// <summary>
        /// Not stated.
        /// </summary>
        Unspecified   = 0,

        /// <summary>
        /// Normal (Gaussian) distribution.
        /// </summary>
        Normal        = 1,

        /// <summary>
        /// Rectangular (uniform) distribution, GUM Section 4.3.7.
        /// </summary>
        Rectangular   = 2,

        /// <summary>
        /// Triangular distribution, GUM Section 4.3.9.
        /// </summary>
        Triangular    = 3,

        /// <summary>
        /// U-shaped (arcsine) distribution.
        /// </summary>
        UShaped       = 4,

        /// <summary>
        /// Student's t-distribution.
        /// </summary>
        StudentT      = 5

    }


    /// <summary>
    /// A measurement uncertainty as defined by the "Guide to the Expression of
    /// Uncertainty in Measurement" (GUM, JCGM 100:2008, BIPM).
    ///
    /// The magnitude is stored exactly as reported, together with the coverage
    /// factor k it belongs to: a calibration certificate stating U = 0.02 with
    /// k = 2 keeps both numbers instead of being normalised to k = 1 and losing
    /// what the certificate actually says. The standard uncertainty u is
    /// available as a derived value.
    /// </summary>
    public readonly struct MeasurementUncertainty : IEquatable<MeasurementUncertainty>
    {

        #region Properties

        /// <summary>
        /// The magnitude of the uncertainty as reported: the standard
        /// uncertainty u when the coverage factor is 1, otherwise the
        /// expanded uncertainty U = k · u.
        /// </summary>
        public Decimal                  Value                  { get; }

        /// <summary>
        /// The coverage factor k the magnitude belongs to. 1 for a standard
        /// uncertainty; calibration certificates commonly state 2.
        /// </summary>
        public Decimal                  CoverageFactor         { get; }

        /// <summary>
        /// The optional coverage probability (level of confidence) as a
        /// fraction between 0 and 1, e.g. 0.95 for the customary 95 %.
        /// </summary>
        public Double?                  CoverageProbability    { get; }

        /// <summary>
        /// The optional probability distribution assigned to the uncertainty.
        /// </summary>
        public UncertaintyDistribution  Distribution           { get; }

        /// <summary>
        /// The optional effective degrees of freedom, needed to derive a
        /// coverage factor from a coverage probability (GUM Annex G).
        /// </summary>
        public Double?                  DegreesOfFreedom       { get; }


        /// <summary>
        /// The standard uncertainty u = Value / k.
        /// </summary>
        public Decimal                  StandardUncertainty
            => Value / CoverageFactor;

        /// <summary>
        /// Whether this is a plain standard uncertainty without any further
        /// statement, which is what a bare number on the wire means.
        /// </summary>
        public Boolean                  IsPlainStandardUncertainty
            => CoverageFactor      == 1              &&
               CoverageProbability is null           &&
               Distribution        == UncertaintyDistribution.Unspecified &&
               DegreesOfFreedom    is null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new measurement uncertainty.
        /// </summary>
        /// <param name="Value">The magnitude of the uncertainty. Must not be negative.</param>
        /// <param name="CoverageFactor">The coverage factor k the magnitude belongs to. Must be positive.</param>
        /// <param name="CoverageProbability">An optional coverage probability between 0 and 1.</param>
        /// <param name="Distribution">An optional probability distribution.</param>
        /// <param name="DegreesOfFreedom">Optional effective degrees of freedom. Must be positive.</param>
        public MeasurementUncertainty(Decimal                   Value,
                                      Decimal?                  CoverageFactor        = null,
                                      Double?                   CoverageProbability   = null,
                                      UncertaintyDistribution?  Distribution          = null,
                                      Double?                   DegreesOfFreedom      = null)
        {

            if (Value < 0)
                throw new ArgumentException("A measurement uncertainty must not be negative!",              nameof(Value));

            if (CoverageFactor <= 0)
                throw new ArgumentException("A coverage factor must be positive!",                          nameof(CoverageFactor));

            if (CoverageProbability is not null &&
               (CoverageProbability <= 0 || CoverageProbability > 1))
                throw new ArgumentException("A coverage probability must be within ]0, 1]!",                nameof(CoverageProbability));

            if (DegreesOfFreedom is not null && DegreesOfFreedom <= 0)
                throw new ArgumentException("The effective degrees of freedom must be positive!",           nameof(DegreesOfFreedom));

            this.Value                = Value;
            this.CoverageFactor       = CoverageFactor ?? 1;
            this.CoverageProbability  = CoverageProbability;
            this.Distribution         = Distribution   ?? UncertaintyDistribution.Unspecified;
            this.DegreesOfFreedom     = DegreesOfFreedom;

        }

        #endregion


        #region (implicit) MeasurementUncertainty(StandardUncertainty)

        /// <summary>
        /// Convert the given number into a standard uncertainty (k = 1).
        /// </summary>
        /// <param name="StandardUncertainty">A standard uncertainty.</param>
        public static implicit operator MeasurementUncertainty(Decimal StandardUncertainty)

            => new (StandardUncertainty);

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeasurementUncertainty1">A measurement uncertainty.</param>
        /// <param name="MeasurementUncertainty2">Another measurement uncertainty.</param>
        public static Boolean operator == (MeasurementUncertainty MeasurementUncertainty1,
                                           MeasurementUncertainty MeasurementUncertainty2)

            => MeasurementUncertainty1.Equals(MeasurementUncertainty2);

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeasurementUncertainty1">A measurement uncertainty.</param>
        /// <param name="MeasurementUncertainty2">Another measurement uncertainty.</param>
        public static Boolean operator != (MeasurementUncertainty MeasurementUncertainty1,
                                           MeasurementUncertainty MeasurementUncertainty2)

            => !MeasurementUncertainty1.Equals(MeasurementUncertainty2);

        #endregion

        #region IEquatable<MeasurementUncertainty> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two measurement uncertainties for equality.
        /// </summary>
        /// <param name="Object">A measurement uncertainty to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeasurementUncertainty measurementUncertainty &&
                   Equals(measurementUncertainty);

        #endregion

        #region Equals(MeasurementUncertainty)

        /// <summary>
        /// Compares two measurement uncertainties for equality.
        /// This compares the representation: U = 0.02 with k = 2 is not the
        /// same statement as u = 0.01, although both describe the same spread.
        /// </summary>
        /// <param name="MeasurementUncertainty">A measurement uncertainty to compare with.</param>
        public Boolean Equals(MeasurementUncertainty MeasurementUncertainty)

            => Value               == MeasurementUncertainty.Value               &&
               CoverageFactor      == MeasurementUncertainty.CoverageFactor      &&
               CoverageProbability == MeasurementUncertainty.CoverageProbability &&
               Distribution        == MeasurementUncertainty.Distribution        &&
               DegreesOfFreedom    == MeasurementUncertainty.DegreesOfFreedom;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => HashCode.Combine(Value,
                                CoverageFactor,
                                CoverageProbability,
                                Distribution,
                                DegreesOfFreedom);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object, e.g. "0.02" or "0.02 (k=2)".
        /// </summary>
        public override String ToString()

            => CoverageFactor == 1
                   ?  Value.ToString(CultureInfo.InvariantCulture)
                   : $"{Value.ToString(CultureInfo.InvariantCulture)} (k={CoverageFactor.ToString(CultureInfo.InvariantCulture)})";

        #endregion

    }

}
