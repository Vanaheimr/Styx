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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An ISO 3166-1 country.
    /// https://laendercode.net
    /// </summary>
    public class Country : IEquatable <Country>,
                           IComparable<Country>,
                           IComparable
    {

        #region Data

        private static readonly Dictionary<I18NString, Country> CountryNames  = new Dictionary<I18NString, Country>();
        private static readonly Dictionary<String,     Country> Alpha2Codes   = new Dictionary<String,    Country>();
        private static readonly Dictionary<String,     Country> Alpha3Codes   = new Dictionary<String,    Country>();
        private static readonly Dictionary<UInt16,     Country> NumericCodes  = new Dictionary<UInt16,    Country>();
        private static readonly Dictionary<UInt16,     Country> TelefonCodes  = new Dictionary<UInt16,    Country>();

        #endregion

        #region Properties

        /// <summary>
        /// The name of the country.
        /// </summary>
        public I18NString  CountryName    { get; }

        /// <summary>
        /// The ISO 3166-1 Alpha-2 code of the country.
        /// </summary>
        public String      Alpha2Code     { get; }

        /// <summary>
        /// The ISO 3166-1 Alpha-3 code of the country.
        /// </summary>
        public String      Alpha3Code     { get; }

        /// <summary>
        /// The ISO 3166-1 numeric code UN M49 numerical code of the country.
        /// </summary>
        public UInt16      NumericCode    { get; }

        /// <summary>
        /// Country calling code or dial in code defined by ITU-T recommendations
        /// E.123 and E.164, also called IDD (International Direct Dialling) or
        /// ISD (International Subscriber Dialling) code.
        /// </summary>
        public UInt16      TelefonCode    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new country based on a country name and its codings.
        /// </summary>
        /// <param name="CountryName">The name of the country.</param>
        /// <param name="Alpha2Code">The ISO Alpha-2 Code of the country.</param>
        /// <param name="Alpha3Code">The ISO Alpha-3 Code of the country.</param>
        /// <param name="NumericCode">The ISO numeric code UN M49 Numerical Code of the country.</param>
        /// <param name="TelefonCode">Country calling code or dial in code defined by ITU-T recommendations E.123 and E.164, also called IDD (International Direct Dialling) or ISD (International Subscriber Dialling) code.</param>
        public Country(I18NString  CountryName,
                       String      Alpha2Code,
                       String      Alpha3Code,
                       UInt16      NumericCode,
                       UInt16      TelefonCode)
        {

            this.CountryName  = CountryName;
            this.Alpha2Code   = Alpha2Code;
            this.Alpha3Code   = Alpha3Code;
            this.NumericCode  = NumericCode;
            this.TelefonCode  = TelefonCode;

        }

        #endregion


        /// <summary>
        /// An unknown country.
        /// </summary>
        public static readonly Country unknown = new (new I18NString(Languages.en, "unknown"), "", "", 000, 00);

        #region List of countries

        public static readonly Country Afghanistan                              = new (new I18NString(Languages.en, "Afghanistan"),                                    "AF", "AFG", 004, 93);
        public static readonly Country AlandIslands                             = new (new I18NString(Languages.en, "Åland Islands"),                                  "AX", "ALA", 248, 35818);
        public static readonly Country Albania                                  = new (new I18NString(Languages.en, "Albania"),                                        "AL", "ALB", 008, 355);
        public static readonly Country Algeria                                  = new (new I18NString(Languages.en, "Algeria"),                                        "DZ", "DZA", 012, 213);
        public static readonly Country AmericanSamoa                            = new (new I18NString(Languages.en, "American Samoa"),                                 "AS", "ASM", 016, 1684);
        public static readonly Country Andorra                                  = new (new I18NString(Languages.en, "Andorra"),                                        "AD", "AND", 020, 376);
        public static readonly Country Angola                                   = new (new I18NString(Languages.en, "Angola"),                                         "AO", "AGO", 024, 244);
        public static readonly Country Anguilla                                 = new (new I18NString(Languages.en, "Anguilla"),                                       "AI", "AIA", 660, 1264);
        public static readonly Country Antarctica                               = new (new I18NString(Languages.en, "Antarctica"),                                     "AQ", "ATA", 010, 0);
        public static readonly Country AntiguaAndBarbuda                        = new (new I18NString(Languages.en, "Antigua and Barbuda"),                            "AG", "ATG", 028, 1268);
        public static readonly Country Argentina                                = new (new I18NString(Languages.en, "Argentina"),                                      "AR", "ARG", 032, 54);
        public static readonly Country Armenia                                  = new (new I18NString(Languages.en, "Armenia"),                                        "AM", "ARM", 051, 374);
        public static readonly Country Aruba                                    = new (new I18NString(Languages.en, "Aruba"),                                          "AW", "ABW", 533, 297);
        //Ascension    +247
        public static readonly Country Australia                                = new (new I18NString(Languages.en, "Australia"),                                      "AU", "AUS", 036, 61);
        //Australian External Territories    +672
        public static readonly Country Austria                                  = new (new I18NString(Languages.en, "Austria"),                                        "AT", "AUT", 040, 43);
        public static readonly Country Azerbaijan                               = new (new I18NString(Languages.en, "Azerbaijan"),                                     "AZ", "AZE", 031, 994);
        public static readonly Country Bahamas                                  = new (new I18NString(Languages.en, "Bahamas"),                                        "BS", "BHS", 044, 1242);
        public static readonly Country Bahrain                                  = new (new I18NString(Languages.en, "Bahrain"),                                        "BH", "BHR", 048, 973);
        public static readonly Country Bangladesh                               = new (new I18NString(Languages.en, "Bangladesh"),                                     "BD", "BGD", 050, 880);
        public static readonly Country Barbados                                 = new (new I18NString(Languages.en, "Barbados"),                                       "BB", "BRB", 052, 1246);
        //Barbuda    +1 268
        public static readonly Country Belarus                                  = new (new I18NString(Languages.en, "Belarus"),                                        "BY", "BLR", 112, 375);
        public static readonly Country Belgium                                  = new (new I18NString(Languages.en, "Belgium"),                                        "BE", "BEL", 056, 32);
        public static readonly Country Belize                                   = new (new I18NString(Languages.en, "Belize"),                                         "BZ", "BLZ", 084, 501);
        public static readonly Country Benin                                    = new (new I18NString(Languages.en, "Benin"),                                          "BJ", "BEN", 204, 229);
        public static readonly Country Bermuda                                  = new (new I18NString(Languages.en, "Bermuda"),                                        "BM", "BMU", 060, 1441);
        public static readonly Country Bhutan                                   = new (new I18NString(Languages.en, "Bhutan"),                                         "BT", "BTN", 064, 975);
        public static readonly Country Bolivia                                  = new (new I18NString(Languages.en, "Bolivia"),                                        "BO", "BOL", 068, 591);
        //Bonaire    +599 7
        public static readonly Country BosniaAndHerzegovina                     = new (new I18NString(Languages.en, "Bosnia and Herzegovina"),                         "BA", "BIH", 070, 387);
        public static readonly Country Botswana                                 = new (new I18NString(Languages.en, "Botswana"),                                       "BW", "BWA", 072, 267);
        public static readonly Country BouvetIsland                             = new (new I18NString(Languages.en, "Bouvet Island"),                                  "BV", "BVT", 074, 0);
        public static readonly Country Brazil                                   = new (new I18NString(Languages.en, "Brazil"),                                         "BR", "BRA", 076, 55);
        public static readonly Country BritishIndianOceanTerritory              = new (new I18NString(Languages.en, "British Indian Ocean Territory"),                 "IO", "IOT", 086, 246);
        public static readonly Country BritishVirginIslands                     = new (new I18NString(Languages.en, "British Virgin Islands"),                         "VG", "VGB", 092, 1284);
        public static readonly Country BruneiDarussalam                         = new (new I18NString(Languages.en, "Brunei Darussalam"),                              "BN", "BRN", 096, 673);
        public static readonly Country Bulgaria                                 = new (new I18NString(Languages.en, "Bulgaria"),                                       "BG", "BGR", 100, 359);
        public static readonly Country BurkinaFaso                              = new (new I18NString(Languages.en, "Burkina Faso"),                                   "BF", "BFA", 854, 226);
        //Burma    +95
        public static readonly Country Burundi                                  = new (new I18NString(Languages.en, "Burundi"),                                        "BI", "BDI", 108,  257);
        public static readonly Country Cambodia                                 = new (new I18NString(Languages.en, "Cambodia"),                                       "KH", "KHM", 116,  855);
        public static readonly Country Cameroon                                 = new (new I18NString(Languages.en, "Cameroon"),                                       "CM", "CMR", 120,  237);
        public static readonly Country Canada                                   = new (new I18NString(Languages.en, "Canada"),                                         "CA", "CAN", 124,    1);
        public static readonly Country CapeVerde                                = new (new I18NString(Languages.en, "Cape Verde"),                                     "CV", "CPV", 132,  238);
        public static readonly Country CaymanIslands                            = new (new I18NString(Languages.en, "Cayman Islands"),                                 "KY", "CYM", 136, 1345);
        public static readonly Country CentralAfricanRepublic                   = new (new I18NString(Languages.en, "Central African Republic"),                       "CF", "CAF", 140,  236);
        public static readonly Country Chad                                     = new (new I18NString(Languages.en, "Chad"),                                           "TD", "TCD", 148,  235);
        //Chatham Island, New Zealand    +64
        public static readonly Country Chile                                    = new (new I18NString(Languages.en, "Chile"),                                          "CL", "CHL", 152,   56);
        public static readonly Country China                                    = new (new I18NString(Languages.en, "China"),                                          "CN", "CHN", 156,   86);
        public static readonly Country ChristmasIsland                          = new (new I18NString(Languages.en, "Christmas Island"),                               "CX", "CXR", 162,   61);
        public static readonly Country Cocos_Keeling_Islands                    = new (new I18NString(Languages.en, "Cocos (Keeling) Islands"),                        "CC", "CCK", 166,   61);
        public static readonly Country Colombia                                 = new (new I18NString(Languages.en, "Colombia"),                                       "CO", "COL", 170,   57);
        public static readonly Country Comoros                                  = new (new I18NString(Languages.en, "Comoros"),                                        "KM", "COM", 174,  269);
        public static readonly Country Congo_Brazzaville                        = new (new I18NString(Languages.en, "Congo (Brazzaville)"),                            "CG", "COG", 178,  242);
        public static readonly Country DemocraticRepublicOfTheCongo             = new (new I18NString(Languages.en, "Democratic Republic of the Congo"),               "CD", "COD", 180,  243);
        public static readonly Country CookIslands                              = new (new I18NString(Languages.en, "Cook Islands"),                                   "CK", "COK", 184,  682);
        public static readonly Country CostaRica                                = new (new I18NString(Languages.en, "Costa Rica"),                                     "CR", "CRI", 188,  506);
        public static readonly Country CoteDIvoire                              = new (new I18NString(Languages.en, "Côte d'Ivoire"),                                  "CI", "CIV", 384,  225);
        public static readonly Country Croatia                                  = new (new I18NString(Languages.en, "Croatia"),                                        "HR", "HRV", 191,  385);
        public static readonly Country Cuba                                     = new (new I18NString(Languages.en, "Cuba"),                                           "CU", "CUB", 192,   53);
        public static readonly Country Cyprus                                   = new (new I18NString(Languages.en, "Cyprus"),                                         "CY", "CYP", 196,  357);
        public static readonly Country CzechRepublic                            = new (new I18NString(Languages.en, "Czech Republic"),                                 "CZ", "CZE", 203,  420);
        public static readonly Country ClippertonIsland                         = new (new I18NString(Languages.en, "Clipperton Island"),                              "CP", "CPT",   0,    0);
        public static readonly Country Denmark                                  = new (new I18NString(Languages.en, "Denmark"),                                        "DK", "DNK", 208,   45);
        //Diego Garcia    +246
        public static readonly Country Djibouti                                 = new (new I18NString(Languages.en, "Djibouti"),                                       "DJ", "DJI", 262,  253);
        public static readonly Country Dominica                                 = new (new I18NString(Languages.en, "Dominica"),                                       "DM", "DMA", 212, 1767);
        public static readonly Country DominicanRepublic                        = new (new I18NString(Languages.en, "Dominican Republic"),                             "DO", "DOM", 214, 1809);
        //East Timor    +670
        //Easter Island    +56
        public static readonly Country Ecuador                                  = new (new I18NString(Languages.en, "Ecuador"),                                        "EC", "ECU", 218, 593);
        public static readonly Country Egypt                                    = new (new I18NString(Languages.en, "Egypt"),                                          "EG", "EGY", 818, 20);
        public static readonly Country ElSalvador                               = new (new I18NString(Languages.en, "El Salvador"),                                    "SV", "SLV", 222, 503);
        public static readonly Country EquatorialGuinea                         = new (new I18NString(Languages.en, "Equatorial Guinea"),                              "GQ", "GNQ", 226, 240);
        public static readonly Country Eritrea                                  = new (new I18NString(Languages.en, "Eritrea"),                                        "ER", "ERI", 232, 291);
        public static readonly Country Estonia                                  = new (new I18NString(Languages.en, "Estonia"),                                        "EE", "EST", 233, 372);
        public static readonly Country Ethiopia                                 = new (new I18NString(Languages.en, "Ethiopia"),                                       "ET", "ETH", 231, 251);
        public static readonly Country FalklandIslands_Malvinas                 = new (new I18NString(Languages.en, "Falkland Islands (Malvinas)"),                    "FK", "FLK", 238, 500);
        public static readonly Country FaroeIslands                             = new (new I18NString(Languages.en, "Faroe Islands"),                                  "FO", "FRO", 234, 298);
        public static readonly Country Fiji                                     = new (new I18NString(Languages.en, "Fiji"),                                           "FJ", "FJI", 242, 679);
        public static readonly Country Finland                                  = new (new I18NString(Languages.en, "Finland"),                                        "FI", "FIN", 246, 358);
        public static readonly Country France                                   = new (new I18NString(Languages.en, "France"),                                         "FR", "FRA", 250, 33);
        public static readonly Country FrenchGuiana                             = new (new I18NString(Languages.en, "French Guiana"),                                  "GF", "GUF", 254, 594);
        public static readonly Country FrenchPolynesia                          = new (new I18NString(Languages.en, "French Polynesia"),                               "PF", "PYF", 258, 689);
        public static readonly Country FrenchSouthernTerritories                = new (new I18NString(Languages.en, "French Southern Territories"),                    "TF", "ATF", 260, 596);
        public static readonly Country Gabon                                    = new (new I18NString(Languages.en, "Gabon"),                                          "GA", "GAB", 266, 241);
        public static readonly Country Gambia                                   = new (new I18NString(Languages.en, "Gambia"),                                         "GM", "GMB", 270, 220);
        public static readonly Country Georgia                                  = new (new I18NString(Languages.en, "Georgia"),                                        "GE", "GEO", 268, 995);
        public static readonly Country Germany                                  = new (new I18NString(Languages.en, "Germany").Set(Languages.de, "Deutschland"),       "DE", "DEU", 276, 49);
        public static readonly Country Ghana                                    = new (new I18NString(Languages.en, "Ghana"),                                          "GH", "GHA", 288, 233);
        public static readonly Country Gibraltar                                = new (new I18NString(Languages.en, "Gibraltar"),                                      "GI", "GIB", 292, 350);
        public static readonly Country Greece                                   = new (new I18NString(Languages.en, "Greece"),                                         "GR", "GRC", 300, 30);
        public static readonly Country Greenland                                = new (new I18NString(Languages.en, "Greenland"),                                      "GL", "GRL", 304, 229);
        public static readonly Country Grenada                                  = new (new I18NString(Languages.en, "Grenada"),                                        "GD", "GRD", 308, 1473);
        public static readonly Country Guadeloupe                               = new (new I18NString(Languages.en, "Guadeloupe"),                                     "GP", "GLP", 312, 590);
        public static readonly Country Guam                                     = new (new I18NString(Languages.en, "Guam"),                                           "GU", "GUM", 316, 1671);
        public static readonly Country Guatemala                                = new (new I18NString(Languages.en, "Guatemala"),                                      "GT", "GTM", 320, 502);
        public static readonly Country Guernsey                                 = new (new I18NString(Languages.en, "Guernsey"),                                       "GG", "GGY", 831, 44);
        public static readonly Country Guinea                                   = new (new I18NString(Languages.en, "Guinea"),                                         "GN", "GIN", 324, 224);
        public static readonly Country GuineaBissau                             = new (new I18NString(Languages.en, "Guinea-Bissau"),                                  "GW", "GNB", 624, 245);
        public static readonly Country Guyana                                   = new (new I18NString(Languages.en, "Guyana"),                                         "GY", "GUY", 328, 592);
        public static readonly Country Haiti                                    = new (new I18NString(Languages.en, "Haiti"),                                          "HT", "HTI", 332, 509);
        public static readonly Country HeardIslandAndMcdonaldIslands            = new (new I18NString(Languages.en, "Heard Island and Mcdonald Islands"),              "HM", "HMD", 334, 0);
        public static readonly Country Honduras                                 = new (new I18NString(Languages.en, "Honduras"),                                       "HN", "HND", 340, 504);
        public static readonly Country HongKong                                 = new (new I18NString(Languages.en, "Hong Kong"),                                      "HK", "HKG", 344, 852);
        public static readonly Country Hungary                                  = new (new I18NString(Languages.en, "Hungary"),                                        "HU", "HUN", 348, 36);
        public static readonly Country Iceland                                  = new (new I18NString(Languages.en, "Iceland"),                                        "IS", "ISL", 352, 354);
        public static readonly Country India                                    = new (new I18NString(Languages.en, "India"),                                          "IN", "IND", 356, 91);
        public static readonly Country Indonesia                                = new (new I18NString(Languages.en, "Indonesia"),                                      "ID", "IDN", 360, 62);
        public static readonly Country IslamicRepublicOfIran                    = new (new I18NString(Languages.en, "Islamic Republic of Iran"),                       "IR", "IRN", 364, 98);
        public static readonly Country Iraq                                     = new (new I18NString(Languages.en, "Iraq"),                                           "IQ", "IRQ", 368, 964);
        public static readonly Country Ireland                                  = new (new I18NString(Languages.en, "Ireland"),                                        "IE", "IRL", 372, 353);
        public static readonly Country IsleOfMan                                = new (new I18NString(Languages.en, "Isle of Man"),                                    "IM", "IMN", 833, 44);
        public static readonly Country Israel                                   = new (new I18NString(Languages.en, "Israel"),                                         "IL", "ISR", 376, 972);
        public static readonly Country Italy                                    = new (new I18NString(Languages.en, "Italy"),                                          "IT", "ITA", 380, 39);
        public static readonly Country Jamaica                                  = new (new I18NString(Languages.en, "Jamaica"),                                        "JM", "JAM", 388, 1876);
        //Jan Mayen+47 79
        public static readonly Country Japan                                    = new (new I18NString(Languages.en, "Japan"),                                          "JP", "JPN", 392, 81);
        public static readonly Country Jersey                                   = new (new I18NString(Languages.en, "Jersey"),                                         "JE", "JEY", 832, 44);
        public static readonly Country Jordan                                   = new (new I18NString(Languages.en, "Jordan"),                                         "JO", "JOR", 400, 962);
        public static readonly Country Kazakhstan                               = new (new I18NString(Languages.en, "Kazakhstan"),                                     "KZ", "KAZ", 398, 76);
        public static readonly Country Kenya                                    = new (new I18NString(Languages.en, "Kenya"),                                          "KE", "KEN", 404, 254);
        public static readonly Country Kiribati                                 = new (new I18NString(Languages.en, "Kiribati"),                                       "KI", "KIR", 296, 686);
        public static readonly Country DemocraticPeoplesRepublicOfKorea         = new (new I18NString(Languages.en, "Democratic People's Republic of Korea"),          "KP", "PRK", 408, 850);
        public static readonly Country RepublicOfKorea                          = new (new I18NString(Languages.en, "Republic of Korea"),                              "KR", "KOR", 410, 82);
        public static readonly Country Kuwait                                   = new (new I18NString(Languages.en, "Kuwait"),                                         "KW", "KWT", 414, 965);
        public static readonly Country Kyrgyzstan                               = new (new I18NString(Languages.en, "Kyrgyzstan"),                                     "KG", "KGZ", 417, 996);
        public static readonly Country LaoPDR                                   = new (new I18NString(Languages.en, "Lao PDR"),                                        "LA", "LAO", 418, 856);
        public static readonly Country Latvia                                   = new (new I18NString(Languages.en, "Latvia"),                                         "LV", "LVA", 428, 371);
        public static readonly Country Lebanon                                  = new (new I18NString(Languages.en, "Lebanon"),                                        "LB", "LBN", 422, 961);
        public static readonly Country Lesotho                                  = new (new I18NString(Languages.en, "Lesotho"),                                        "LS", "LSO", 426, 266);
        public static readonly Country Liberia                                  = new (new I18NString(Languages.en, "Liberia"),                                        "LR", "LBR", 430, 231);
        public static readonly Country Libya                                    = new (new I18NString(Languages.en, "Libya"),                                          "LY", "LBY", 434, 218);
        public static readonly Country Liechtenstein                            = new (new I18NString(Languages.en, "Liechtenstein"),                                  "LI", "LIE", 438, 423);
        public static readonly Country Lithuania                                = new (new I18NString(Languages.en, "Lithuania"),                                      "LT", "LTU", 440, 370);
        public static readonly Country Luxembourg                               = new (new I18NString(Languages.en, "Luxembourg"),                                     "LU", "LUX", 442, 352);
        public static readonly Country RepublicOfMacedonia                      = new (new I18NString(Languages.en, "Republic of Macedonia"),                          "MK", "MKD", 807, 389);
        public static readonly Country Macau                                    = new (new I18NString(Languages.en, "Macau"),                                          "MO", "MAC", 446, 835);
        public static readonly Country Madagascar                               = new (new I18NString(Languages.en, "Madagascar"),                                     "MG", "MDG", 450, 261);
        public static readonly Country Malawi                                   = new (new I18NString(Languages.en, "Malawi"),                                         "MW", "MWI", 454, 265);
        public static readonly Country Malaysia                                 = new (new I18NString(Languages.en, "Malaysia"),                                       "MY", "MYS", 458, 60);
        public static readonly Country Maldives                                 = new (new I18NString(Languages.en, "Maldives"),                                       "MV", "MDV", 462, 960);
        public static readonly Country Mali                                     = new (new I18NString(Languages.en, "Mali"),                                           "ML", "MLI", 466, 223);
        public static readonly Country Malta                                    = new (new I18NString(Languages.en, "Malta"),                                          "MT", "MLT", 470, 356);
        public static readonly Country MarshallIslands                          = new (new I18NString(Languages.en, "Marshall Islands"),                               "MH", "MHL", 584, 692);
        public static readonly Country Martinique                               = new (new I18NString(Languages.en, "Martinique"),                                     "MQ", "MTQ", 474, 596);
        public static readonly Country Mauritania                               = new (new I18NString(Languages.en, "Mauritania"),                                     "MR", "MRT", 478, 222);
        public static readonly Country Mauritius                                = new (new I18NString(Languages.en, "Mauritius"),                                      "MU", "MUS", 480, 230);
        public static readonly Country Mayotte                                  = new (new I18NString(Languages.en, "Mayotte"),                                        "YT", "MYT", 175, 262);
        public static readonly Country Mexico                                   = new (new I18NString(Languages.en, "Mexico"),                                         "MX", "MEX", 484, 52);
        public static readonly Country FederatedStatesOfMicronesia              = new (new I18NString(Languages.en, "Federated States of Micronesia"),                 "FM", "FSM", 583, 691);
        //Midway Island, USA    +1 808
        public static readonly Country Moldova                                  = new (new I18NString(Languages.en, "Moldova"),                                        "MD", "MDA", 498, 373);
        public static readonly Country Monaco                                   = new (new I18NString(Languages.en, "Monaco"),                                         "MC", "MCO", 492, 377);
        public static readonly Country Mongolia                                 = new (new I18NString(Languages.en, "Mongolia"),                                       "MN", "MNG", 496, 976);
        public static readonly Country Montenegro                               = new (new I18NString(Languages.en, "Montenegro"),                                     "ME", "MNE", 499, 382);
        public static readonly Country Montserrat                               = new (new I18NString(Languages.en, "Montserrat"),                                     "MS", "MSR", 500, 1664);
        public static readonly Country Morocco                                  = new (new I18NString(Languages.en, "Morocco"),                                        "MA", "MAR", 504, 212);
        public static readonly Country Mozambique                               = new (new I18NString(Languages.en, "Mozambique"),                                     "MZ", "MOZ", 508, 258);
        public static readonly Country Myanmar                                  = new (new I18NString(Languages.en, "Myanmar"),                                        "MM", "MMR", 104, 264);
        public static readonly Country Namibia                                  = new (new I18NString(Languages.en, "Namibia"),                                        "NA", "NAM", 516, 264);
        public static readonly Country Nauru                                    = new (new I18NString(Languages.en, "Nauru"),                                          "NR", "NRU", 520, 674);
        public static readonly Country Nepal                                    = new (new I18NString(Languages.en, "Nepal"),                                          "NP", "NPL", 524, 977);
        public static readonly Country Netherlands                              = new (new I18NString(Languages.en, "Netherlands"),                                    "NL", "NLD", 528, 31);
        public static readonly Country NetherlandsAntilles                      = new (new I18NString(Languages.en, "Netherlands Antilles"),                           "AN", "ANT", 530, 1869);
        public static readonly Country Curaçao                                  = new (new I18NString(Languages.en, "Curaçao"),                                        "CW", "CUW", 531, 599);
        public static readonly Country NewCaledonia                             = new (new I18NString(Languages.en, "New Caledonia"),                                  "NC", "NCL", 540, 687);
        public static readonly Country NewZealand                               = new (new I18NString(Languages.en, "New Zealand"),                                    "NZ", "NZL", 554, 64);
        public static readonly Country Nicaragua                                = new (new I18NString(Languages.en, "Nicaragua"),                                      "NI", "NIC", 558, 505);
        public static readonly Country Niger                                    = new (new I18NString(Languages.en, "Niger"),                                          "NE", "NER", 562, 227);
        public static readonly Country Nigeria                                  = new (new I18NString(Languages.en, "Nigeria"),                                        "NG", "NGA", 566, 234);
        public static readonly Country Niue                                     = new (new I18NString(Languages.en, "Niue"),                                           "NU", "NIU", 570, 683);
        public static readonly Country NorfolkIsland                            = new (new I18NString(Languages.en, "Norfolk Island"),                                 "NF", "NFK", 574, 672);
        public static readonly Country NorthernMarianaIslands                   = new (new I18NString(Languages.en, "Northern Mariana Islands"),                       "MP", "MNP", 580, 1670);
        public static readonly Country Norway                                   = new (new I18NString(Languages.en, "Norway"),                                         "NO", "NOR", 578, 47);
        public static readonly Country Oman                                     = new (new I18NString(Languages.en, "Oman"),                                           "OM", "OMN", 512, 968);
        public static readonly Country Pakistan                                 = new (new I18NString(Languages.en, "Pakistan"),                                       "PK", "PAK", 586, 92);
        public static readonly Country Palau                                    = new (new I18NString(Languages.en, "Palau"),                                          "PW", "PLW", 585, 680);
        public static readonly Country PalestinianTerritory                     = new (new I18NString(Languages.en, "Palestinian Territory"),                          "PS", "PSE", 275, 970);
        public static readonly Country Panama                                   = new (new I18NString(Languages.en, "Panama"),                                         "PA", "PAN", 591, 507);
        public static readonly Country PapuaNewGuinea                           = new (new I18NString(Languages.en, "Papua New Guinea"),                               "PG", "PNG", 598, 675);
        public static readonly Country Paraguay                                 = new (new I18NString(Languages.en, "Paraguay"),                                       "PY", "PRY", 600, 595);
        public static readonly Country Peru                                     = new (new I18NString(Languages.en, "Peru"),                                           "PE", "PER", 604, 51);
        public static readonly Country Philippines                              = new (new I18NString(Languages.en, "Philippines"),                                    "PH", "PHL", 608, 63);
        public static readonly Country Pitcairn                                 = new (new I18NString(Languages.en, "Pitcairn"),                                       "PN", "PCN", 612, 64);
        public static readonly Country Poland                                   = new (new I18NString(Languages.en, "Poland"),                                         "PL", "POL", 616, 48);
        public static readonly Country Portugal                                 = new (new I18NString(Languages.en, "Portugal"),                                       "PT", "PRT", 620, 351);
        public static readonly Country PuertoRico                               = new (new I18NString(Languages.en, "Puerto Rico"),                                    "PR", "PRI", 630, 1787);
        public static readonly Country Qatar                                    = new (new I18NString(Languages.en, "Qatar"),                                          "QA", "QAT", 634, 974);
        public static readonly Country Réunion                                  = new (new I18NString(Languages.en, "Réunion"),                                        "RE", "REU", 638, 262);
        public static readonly Country Romania                                  = new (new I18NString(Languages.en, "Romania"),                                        "RO", "ROU", 642, 40);
        public static readonly Country RussianFederation                        = new (new I18NString(Languages.en, "Russian Federation"),                             "RU", "RUS", 643, 7);
        public static readonly Country Rwanda                                   = new (new I18NString(Languages.en, "Rwanda"),                                         "RW", "RWA", 646, 250);
        //Saba    +599 4
        public static readonly Country SaintBarthelemy                          = new (new I18NString(Languages.en, "Saint-Barthélemy"),                               "BL", "BLM", 652, 590);
        public static readonly Country SaintHelena                              = new (new I18NString(Languages.en, "Saint Helena"),                                   "SH", "SHN", 654, 290);
        public static readonly Country SaintKittsAndNevis                       = new (new I18NString(Languages.en, "Saint Kitts and Nevis"),                          "KN", "KNA", 659, 1869);
        public static readonly Country SaintLucia                               = new (new I18NString(Languages.en, "Saint Lucia"),                                    "LC", "LCA", 662, 1758);
        public static readonly Country SaintMartin                              = new (new I18NString(Languages.en, "Saint-Martin (French part)"),                     "MF", "MAF", 663, 590);
        public static readonly Country SaintPierreAndMiquelon                   = new (new I18NString(Languages.en, "Saint Pierre and Miquelon"),                      "PM", "SPM", 666, 508);
        public static readonly Country SaintVincentAndGrenadines                = new (new I18NString(Languages.en, "Saint Vincent and Grenadines"),                   "VC", "VCT", 670, 1784);
        public static readonly Country Samoa                                    = new (new I18NString(Languages.en, "Samoa"),                                          "WS", "WSM", 882, 685);
        public static readonly Country SanMarino                                = new (new I18NString(Languages.en, "San Marino"),                                     "SM", "SMR", 674, 378);
        public static readonly Country SaoTomeAndPrincipe                       = new (new I18NString(Languages.en, "São Tomé and Príncipe"),                          "ST", "STP", 678, 239);
        public static readonly Country SaudiArabia                              = new (new I18NString(Languages.en, "Saudi Arabia"),                                   "SA", "SAU", 682, 966);
        public static readonly Country Senegal                                  = new (new I18NString(Languages.en, "Senegal"),                                        "SN", "SEN", 686, 221);
        public static readonly Country Serbia                                   = new (new I18NString(Languages.en, "Serbia"),                                         "RS", "SRB", 688, 381);
        public static readonly Country Seychelles                               = new (new I18NString(Languages.en, "Seychelles"),                                     "SC", "SYC", 690, 248);
        public static readonly Country SierraLeone                              = new (new I18NString(Languages.en, "Sierra Leone"),                                   "SL", "SLE", 694, 232);
        public static readonly Country Singapore                                = new (new I18NString(Languages.en, "Singapore"),                                      "SG", "SGP", 702, 65);
        //Sint Eustatius    +599 3
        //Sint Maarten (Netherlands)    +1 721
        public static readonly Country Slovakia                                 = new (new I18NString(Languages.en, "Slovakia"),                                       "SK", "SVK", 703, 421);
        public static readonly Country Slovenia                                 = new (new I18NString(Languages.en, "Slovenia"),                                       "SI", "SVN", 705, 386);
        public static readonly Country SolomonIslands                           = new (new I18NString(Languages.en, "Solomon Islands"),                                "SB", "SLB", 090, 677);
        public static readonly Country Somalia                                  = new (new I18NString(Languages.en, "Somalia"),                                        "SO", "SOM", 706, 252);
        public static readonly Country SouthAfrica                              = new (new I18NString(Languages.en, "South Africa"),                                   "ZA", "ZAF", 710, 27);
        public static readonly Country SouthGeorgiaAndTheSouthSandwichIslands   = new (new I18NString(Languages.en, "South Georgia and the South Sandwich Islands"),   "GS", "SGS", 239, 500);
        //South Ossetia    +995 34
        public static readonly Country SouthSudan                               = new (new I18NString(Languages.en, "South Sudan"),                                    "SS", "SSD", 728, 211);
        public static readonly Country Spain                                    = new (new I18NString(Languages.en, "Spain"),                                          "ES", "ESP", 724, 34);
        public static readonly Country SriLanka                                 = new (new I18NString(Languages.en, "Sri Lanka"),                                      "LK", "LKA", 144, 94);
        public static readonly Country Sudan                                    = new (new I18NString(Languages.en, "Sudan"),                                          "SD", "SDN", 736, 249);
        public static readonly Country Suriname                                 = new (new I18NString(Languages.en, "Suriname"),                                       "SR", "SUR", 740, 597);
        public static readonly Country SvalbardAndJanMayenIslands               = new (new I18NString(Languages.en, "Svalbard and Jan Mayen Islands"),                 "SJ", "SJM", 744, 4779);
        public static readonly Country Swaziland                                = new (new I18NString(Languages.en, "Swaziland"),                                      "SZ", "SWZ", 748, 268);
        public static readonly Country Sweden                                   = new (new I18NString(Languages.en, "Sweden"),                                         "SE", "SWE", 752, 46);
        public static readonly Country Switzerland                              = new (new I18NString(Languages.en, "Switzerland"),                                    "CH", "CHE", 756, 41);
        public static readonly Country SyrianArabRepublic                       = new (new I18NString(Languages.en, "Syrian Arab Republic"),                           "SY", "SYR", 760, 963);
        public static readonly Country Taiwan                                   = new (new I18NString(Languages.en, "Taiwan"),                                         "TW", "TWN", 158, 886);
        public static readonly Country Tajikistan                               = new (new I18NString(Languages.en, "Tajikistan"),                                     "TJ", "TJK", 762, 992);
        public static readonly Country UnitedRepublicOfTanzania                 = new (new I18NString(Languages.en, "United Republic of Tanzania"),                    "TZ", "TZA", 834, 255);
        public static readonly Country Thailand                                 = new (new I18NString(Languages.en, "Thailand"),                                       "TH", "THA", 764, 66);
        public static readonly Country TimorLeste                               = new (new I18NString(Languages.en, "Timor-Leste"),                                    "TL", "TLS", 626, 0);
        public static readonly Country Togo                                     = new (new I18NString(Languages.en, "Togo"),                                           "TG", "TGO", 768, 228);
        public static readonly Country Tokelau                                  = new (new I18NString(Languages.en, "Tokelau"),                                        "TK", "TKL", 772, 690);
        public static readonly Country Tonga                                    = new (new I18NString(Languages.en, "Tonga"),                                          "TO", "TON", 776, 676);
        public static readonly Country TrinidadAndTobago                        = new (new I18NString(Languages.en, "Trinidad and Tobago"),                            "TT", "TTO", 780, 1868);
        public static readonly Country Tunisia                                  = new (new I18NString(Languages.en, "Tunisia"),                                        "TN", "TUN", 788, 216);
        public static readonly Country Turkey                                   = new (new I18NString(Languages.en, "Turkey"),                                         "TR", "TUR", 792, 90);
        public static readonly Country Turkmenistan                             = new (new I18NString(Languages.en, "Turkmenistan"),                                   "TM", "TKM", 795, 993);
        public static readonly Country TurksAndCaicosIslands                    = new (new I18NString(Languages.en, "Turks and Caicos Islands"),                       "TC", "TCA", 796, 1649);
        public static readonly Country Tuvalu                                   = new (new I18NString(Languages.en, "Tuvalu"),                                         "TV", "TUV", 798, 688);
        public static readonly Country Uganda                                   = new (new I18NString(Languages.en, "Uganda"),                                         "UG", "UGA", 800, 256);
        public static readonly Country Ukraine                                  = new (new I18NString(Languages.en, "Ukraine"),                                        "UA", "UKR", 804, 380);
        public static readonly Country UnitedArabEmirates                       = new (new I18NString(Languages.en, "United Arab Emirates"),                           "AE", "ARE", 784, 971);
        public static readonly Country UnitedKingdom                            = new (new I18NString(Languages.en, "United Kingdom"),                                 "GB", "GBR", 826, 44);
        public static readonly Country UnitedKingdom_Invalid                    = new (new I18NString(Languages.en, "United Kingdom (invalid)"),                       "UK", "UKI", 11826, 11144);
        public static readonly Country UnitedStatesOfAmerica                    = new (new I18NString(Languages.en, "United States of America"),                       "US", "USA", 840, 1);
        public static readonly Country UnitedStatesMinorOutlyingIslands         = new (new I18NString(Languages.en, "United States Minor Outlying Islands"),           "UM", "UMI", 581, 1);
        public static readonly Country Uruguay                                  = new (new I18NString(Languages.en, "Uruguay"),                                        "UY", "URY", 858, 598);
        public static readonly Country Uzbekistan                               = new (new I18NString(Languages.en, "Uzbekistan"),                                     "UZ", "UZB", 860, 998);
        public static readonly Country Vanuatu                                  = new (new I18NString(Languages.en, "Vanuatu"),                                        "VU", "VUT", 548, 678);
        public static readonly Country VaticanState                             = new (new I18NString(Languages.en, "Vatican City State (Holy See)"),                  "VA", "VAT", 336, 379);
        public static readonly Country Venezuela                                = new (new I18NString(Languages.en, "Bolivarian Republic of Venezuela"),               "VE", "VEN", 862, 58);
        public static readonly Country Vietnam                                  = new (new I18NString(Languages.en, "Vietnam"),                                        "VN", "VNM", 704, 84);
        public static readonly Country VirginIslands                            = new (new I18NString(Languages.en, "Virgin Islands"),                                 "VI", "VIR", 850, 1340);
        //Wake Island, USA    +1 808
        public static readonly Country WallisAndFutunaIslands                   = new (new I18NString(Languages.en, "Wallis and Futuna Islands"),                      "WF", "WLF", 876, 681);
        public static readonly Country WesternSahara                            = new (new I18NString(Languages.en, "Western Sahara"),                                 "EH", "ESH", 732, 0);
        public static readonly Country Yemen                                    = new (new I18NString(Languages.en, "Yemen"),                                          "YE", "YEM", 887, 967);
        public static readonly Country Zambia                                   = new (new I18NString(Languages.en, "Zambia"),                                         "ZM", "ZMB", 894, 260);
        //Zanzibar    +255
        public static readonly Country Zimbabwe                                 = new (new I18NString(Languages.en, "Zimbabwe"),                                       "ZW", "ZWE", 716, 263);

        public static readonly Country Testland                                 = new (new I18NString(Languages.en, "Testland"),                                       "YY", "YYY", 999, 999);

        #endregion


        #region Tools

        #region (private) ReflectData()

        private static void ReflectData()
        {

            lock (CountryNames)
            {

                if (CountryNames.Count == 0)
                {

                    Country _Country;

                    foreach (var _FieldInfo in typeof(Country).GetFields())
                    {

                        _Country = _FieldInfo.GetValue(null) as Country;

                        if (_Country.CountryName == unknown.CountryName)
                            continue;

                        CountryNames.Add(_Country.CountryName,            _Country);
                        Alpha2Codes. Add(_Country.Alpha2Code?.ToLower(),  _Country);
                        Alpha3Codes. Add(_Country.Alpha3Code?.ToLower(),  _Country);
                        NumericCodes.Add(_Country.NumericCode,            _Country);

                        if (!TelefonCodes.ContainsKey(_Country.TelefonCode))
                            TelefonCodes.Add(_Country.TelefonCode, _Country);

                    }

                }

            }

        }

        #endregion


        #region Parse(AnyString)

        /// <summary>
        /// Tries to find the appropriate country for the given string.
        /// </summary>
        /// <param name="AnyString">Any string.</param>
        public static Country Parse(String AnyString)
        {

            if (AnyString.IsNullOrEmpty())
                return default;


            if (AnyString.Length == 2)
                return ParseAlpha2Code(AnyString);

            if (AnyString.Length == 3)
                return ParseAlpha3Code(AnyString);

            if (TryParseNumericCode(AnyString, out Country _Country))
                return _Country;

            if (TryParseTelefonCode(AnyString, out _Country))
                return _Country;

            return ParseCountryName(AnyString);

        }

        #endregion

        #region TryParse(AnyString, out Country)

        /// <summary>
        /// Tries to find the appropriate country for the given string.
        /// </summary>
        /// <param name="AnyString">Any string.</param>
        /// <param name="Country">The country.</param>
        public static Boolean TryParse(String AnyString, out Country Country)
        {

            if (AnyString.IsNullOrEmpty())
            {
                Country = default;
                return false;
            }

            if (TryParseAlpha2Code (AnyString, out Country))
                return true;

            if (TryParseAlpha3Code (AnyString, out Country))
                return true;

            if (TryParseNumericCode(AnyString, out Country))
                return true;

            if (TryParseTelefonCode(AnyString, out Country))
                return true;

            if (TryParseCountryName(AnyString, out Country))
                return true;

            return false;

        }

        #endregion


        #region ParseCountryName(CountryName)

        /// <summary>
        /// Tries to find the appropriate country for the given country name.
        /// </summary>
        /// <param name="CountryName">A country name.</param>
        public static Country ParseCountryName(String CountryName)
        {

            if (CountryName.IsNullOrEmpty())
                return default;

            ReflectData();

            foreach (var countryname in CountryNames)
                foreach (var I8Name in countryname.Key)
                    if (I8Name.Text == CountryName)
                        return countryname.Value;

            return default;

        }

        #endregion

        #region TryParseCountryName(CountryName, out Country)

        /// <summary>
        /// Tries to find the appropriate country for the given country name.
        /// </summary>
        /// <param name="CountryName">A country name.</param>
        /// <param name="Country">The corresponding country.</param>
        /// <returns>true, if successful; false otherwise.</returns>
        public static Boolean TryParseCountryName(String CountryName, out Country Country)
        {

            if (CountryName.IsNullOrEmpty())
            {
                Country = default;
                return false;
            }

            ReflectData();

            foreach (var _CountryName in CountryNames)
                foreach (var I8Name in _CountryName.Key)
                    if (I8Name.Text == CountryName)
                    {
                        Country = _CountryName.Value;
                        return true;
                    }

            Country = null;
            return false;

        }

        #endregion


        #region ParseAlpha2Code(Alpha2Code)

        /// <summary>
        /// Tries to find the appropriate country for the given alpha2code.
        /// </summary>
        /// <param name="Alpha2Code">An alpha2code for a country, e.g. "DE" for Germany.</param>
        public static Country ParseAlpha2Code(String Alpha2Code)
        {

            if (Alpha2Code.IsNullOrEmpty())
                return default;

            ReflectData();

            if (Alpha2Codes.TryGetValue(Alpha2Code.ToLower(), out Country country))
                return country;

            return default;

        }

        #endregion

        #region TryParseAlpha2Code(Alpha2Code, out Country)

        /// <summary>
        /// Tries to find the appropriate country for the given alpha2code.
        /// </summary>
        /// <param name="Alpha2Code">An alpha2code for a country, e.g. "DE" for Germany.</param>
        /// <param name="Country">The corresponding country.</param>
        /// <returns>true, if successful; false otherwise.</returns>
        public static Boolean TryParseAlpha2Code(String Alpha2Code, out Country Country)
        {

            if (Alpha2Code.IsNullOrEmpty())
            {
                Country = default;
                return false;
            }

            ReflectData();

            return Alpha2Codes.TryGetValue(Alpha2Code.ToLower(), out Country);

        }

        #endregion


        #region ParseAlpha3Code(Alpha3Code)

        /// <summary>
        /// Tries to find the appropriate country for the given alpha3code.
        /// </summary>
        /// <param name="Alpha3Code">An alpha3code for a country, e.g. "DEU" for Germany.</param>
        public static Country ParseAlpha3Code(String Alpha3Code)
        {

            if (Alpha3Code.IsNullOrEmpty())
                return default;

            ReflectData();

            if (Alpha3Codes.TryGetValue(Alpha3Code.ToLower(), out Country country))
                return country;

            return default;

        }

        #endregion

        #region TryParseAlpha3Code(Alpha3Code, out Country)

        /// <summary>
        /// Tries to find the appropriate country for the given alpha3code.
        /// </summary>
        /// <param name="Alpha3Code">An alpha3code for a country, e.g. "DEU" for Germany.</param>
        /// <param name="Country">The corresponding country.</param>
        /// <returns>true, if successful; false otherwise.</returns>
        public static Boolean TryParseAlpha3Code(String Alpha3Code, out Country Country)
        {

            if (Alpha3Code.IsNullOrEmpty())
            {
                Country = default;
                return false;
            }

            ReflectData();

            return Alpha3Codes.TryGetValue(Alpha3Code.ToLower(), out Country);

        }

        #endregion


        #region ParseNumericCode(NumericCode)

        /// <summary>
        /// Tries to find the appropriate country for the given numeric code.
        /// </summary>
        /// <param name="NumericCode">A numeric code for a country, e.g. "276" for Germany.</param>
        public static Country ParseNumericCode(UInt16 NumericCode)
        {

            ReflectData();

            Country _Country;

            if (NumericCodes.TryGetValue(NumericCode, out _Country))
                return _Country;

            return null;

        }

        /// <summary>
        /// Tries to find the appropriate country for the given numeric code.
        /// </summary>
        /// <param name="NumericCode">A numeric code for a country, e.g. "276" for Germany.</param>
        public static Country ParseNumericCode(String NumericCode)
        {
            ReflectData();
            return NumericCodes[UInt16.Parse(NumericCode)];
        }

        #endregion

        #region TryParseNumericCode(NumericCode, out Country)

        /// <summary>
        /// Tries to find the appropriate country for the given numeric code.
        /// </summary>
        /// <param name="NumericCode">A numeric code for a country, e.g. "276" for Germany.</param>
        /// <param name="Country">The corresponding country.</param>
        /// <returns>true, if successful; false otherwise.</returns>
        public static Boolean TryParseNumericCode(UInt16 NumericCode, out Country Country)
        {
            ReflectData();
            return NumericCodes.TryGetValue(NumericCode, out Country);
        }

        /// <summary>
        /// Tries to find the appropriate country for the given numeric code.
        /// </summary>
        /// <param name="NumericCode">A numeric code for a country, e.g. "276" for Germany.</param>
        /// <param name="Country">The corresponding country.</param>
        /// <returns>true, if successful; false otherwise.</returns>
        public static Boolean TryParseNumericCode(String NumericCode, out Country Country)
        {

            ReflectData();

            UInt16 _NumericCode;

            if (UInt16.TryParse(NumericCode, out _NumericCode))
                return NumericCodes.TryGetValue(_NumericCode, out Country);

            Country = unknown;
            return false;

        }

        #endregion


        #region ParseTelefonCode(TelefonCode)

        /// <summary>
        /// Tries to find the appropriate country for the given telefon code.
        /// </summary>
        /// <param name="TelefonCode">A telefon code for a country, e.g. "49" for Germany.</param>
        public static Country ParseTelefonCode(UInt16 TelefonCode)
        {

            ReflectData();

            Country _Country;

            if (TelefonCodes.TryGetValue(TelefonCode, out _Country))
                return _Country;

            return null;

        }

        /// <summary>
        /// Tries to find the appropriate country for the given telefon code.
        /// </summary>
        /// <param name="TelefonCode">A telefon code for a country, e.g. "49" for Germany.</param>
        public static Country ParseTelefonCode(String TelefonCode)
        {
            ReflectData();
            return TelefonCodes[UInt16.Parse(TelefonCode)];
        }

        #endregion

        #region TryParseTelefonCode(TelefonCode, out Country)

        /// <summary>
        /// Tries to find the appropriate country for the given telefon code.
        /// </summary>
        /// <param name="TelefonCode">A telefon code for a country, e.g. "49" for Germany.</param>
        /// <param name="Country">The corresponding country.</param>
        /// <returns>true, if successful; false otherwise.</returns>
        public static Boolean TryParseTelefonCode(UInt16 TelefonCode, out Country Country)
        {
            ReflectData();
            return TelefonCodes.TryGetValue(TelefonCode, out Country);
        }

        /// <summary>
        /// Tries to find the appropriate country for the given telefon code.
        /// </summary>
        /// <param name="TelefonCode">A telefon code for a country, e.g. "49" for Germany.</param>
        /// <param name="Country">The corresponding country.</param>
        /// <returns>true, if successful; false otherwise.</returns>
        public static Boolean TryParseTelefonCode(String TelefonCode, out Country Country)
        {

            ReflectData();

            UInt16 _TelefonCode;

            if (UInt16.TryParse(TelefonCode, out _TelefonCode))
                return TelefonCodes.TryGetValue(_TelefonCode, out Country);

            Country = unknown;
            return false;

        }

        #endregion

        #endregion

        #region Clone

        /// <summary>
        /// Clone this energy source.
        /// </summary>
        public Country Clone

            => new Country(CountryName.Clone,
                           Alpha2Code != null ? new String(Alpha2Code.ToCharArray()) : null,
                           Alpha3Code != null ? new String(Alpha3Code.ToCharArray()) : null,
                           NumericCode,
                           TelefonCode);

        #endregion


        #region Operator overloading

        #region Operator == (Country1, Country2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Country1">A country.</param>
        /// <param name="Country2">Another country.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Country Country1, Country Country2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Country1, Country2))
                return true;

            if (Country1 is null || Country2 is null)
                return false;

            if (Country1 is null)
                throw new ArgumentNullException(nameof(Country1),  "The given country must not be null!");

            return Country1.Equals(Country2);

        }

        #endregion

        #region Operator != (Country1, Country2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Country1">A country.</param>
        /// <param name="Country2">Another country.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Country Country1, Country Country2)
            => !(Country1 == Country2);

        #endregion

        #region Operator <  (Country1, Country2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Country1">A country.</param>
        /// <param name="Country2">Another country.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Country Country1, Country Country2)

            => Country1 is null
                   ? throw new ArgumentNullException(nameof(Country1), "The given country must not be null!")
                   : Country1.CompareTo(Country2) < 0;

        #endregion

        #region Operator <= (Country1, Country2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Country1">A country.</param>
        /// <param name="Country2">Another country.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Country Country1, Country Country2)
            => !(Country1 > Country2);

        #endregion

        #region Operator >  (Country1, Country2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Country1">A country.</param>
        /// <param name="Country2">Another country.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Country Country1, Country Country2)

            => Country1 is null
                   ? throw new ArgumentNullException(nameof(Country1), "The given country must not be null!")
                   : Country1.CompareTo(Country2) > 0;

        #endregion

        #region Operator >= (Country1, Country2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Country1">A country.</param>
        /// <param name="Country2">Another country.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Country Country1, Country Country2)
            => !(Country1 < Country2);

        #endregion

        #endregion

        #region IComparable<Country> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Country country
                   ? CompareTo(country)
                   : throw new ArgumentException("The given object is not a country!");

        #endregion

        #region CompareTo(Country)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Country">An object to compare with.</param>
        public Int32 CompareTo(Country Country)

            => Country is null
                   ? throw new ArgumentNullException(nameof(Country), "The given country must not be null!")
                   : String.Compare(Alpha2Code, Country.Alpha2Code, StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Country> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Country country &&
                   Equals(country);

        #endregion

        #region Equals(Country)

        /// <summary>
        /// Compares two Countrys for equality.
        /// </summary>
        /// <param name="Country">A Country to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Country Country)

            => (!(Country is null)) &&

               Alpha2Code.ToLower() == Country.Alpha2Code.ToLower();

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => CountryName.         GetHashCode() ^
               Alpha2Code.ToLower().GetHashCode() ^
               Alpha3Code.ToLower().GetHashCode() ^
               NumericCode.         GetHashCode() ^
               TelefonCode.         GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(CountryName, ", ",
                             Alpha2Code,  ", ",
                             Alpha3Code,  ", ",
                             NumericCode, ", +",
                             TelefonCode);

        #endregion

    }

}
