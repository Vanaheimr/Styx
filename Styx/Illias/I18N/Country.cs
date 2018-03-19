/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A country.
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
        /// The ISO Alpha-2 code of the country.
        /// </summary>
        public String      Alpha2Code     { get; }

        /// <summary>
        /// The ISO Alpha-3 code of the country.
        /// </summary>
        public String      Alpha3Code     { get; }

        /// <summary>
        /// The ISO numeric code UN M49 numerical code of the country.
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
        public static readonly Country unknown = new Country(new I18NString(Languages.eng, "unknown"), "", "", 000, 00);

        #region List of countries

        public static readonly Country Afghanistan                              = new Country(new I18NString(Languages.eng, "Afghanistan"),                                    "AF", "AFG", 004, 93);
        public static readonly Country AlandIslands                             = new Country(new I18NString(Languages.eng, "Åland Islands"),                                  "AX", "ALA", 248, 35818);
        public static readonly Country Albania                                  = new Country(new I18NString(Languages.eng, "Albania"),                                        "AL", "ALB", 008, 355);
        public static readonly Country Algeria                                  = new Country(new I18NString(Languages.eng, "Algeria"),                                        "DZ", "DZA", 012, 213);
        public static readonly Country AmericanSamoa                            = new Country(new I18NString(Languages.eng, "American Samoa"),                                 "AS", "ASM", 016, 1684);
        public static readonly Country Andorra                                  = new Country(new I18NString(Languages.eng, "Andorra"),                                        "AD", "AND", 020, 376);
        public static readonly Country Angola                                   = new Country(new I18NString(Languages.eng, "Angola"),                                         "AO", "AGO", 024, 244);
        public static readonly Country Anguilla                                 = new Country(new I18NString(Languages.eng, "Anguilla"),                                       "AI", "AIA", 660, 1264);
        public static readonly Country Antarctica                               = new Country(new I18NString(Languages.eng, "Antarctica"),                                     "AQ", "ATA", 010, 0);
        public static readonly Country AntiguaAndBarbuda                        = new Country(new I18NString(Languages.eng, "Antigua and Barbuda"),                            "AG", "ATG", 028, 1268);
        public static readonly Country Argentina                                = new Country(new I18NString(Languages.eng, "Argentina"),                                      "AR", "ARG", 032, 54);
        public static readonly Country Armenia                                  = new Country(new I18NString(Languages.eng, "Armenia"),                                        "AM", "ARM", 051, 374);
        public static readonly Country Aruba                                    = new Country(new I18NString(Languages.eng, "Aruba"),                                          "AW", "ABW", 533, 297);
        //Ascension	+247
        public static readonly Country Australia                                = new Country(new I18NString(Languages.eng, "Australia"),                                      "AU", "AUS", 036, 61);
        //Australian External Territories	+672
        public static readonly Country Austria                                  = new Country(new I18NString(Languages.eng, "Austria"),                                        "AT", "AUT", 040, 43);
        public static readonly Country Azerbaijan                               = new Country(new I18NString(Languages.eng, "Azerbaijan"),                                     "AZ", "AZE", 031, 994);
        public static readonly Country Bahamas                                  = new Country(new I18NString(Languages.eng, "Bahamas"),                                        "BS", "BHS", 044, 1242);
        public static readonly Country Bahrain                                  = new Country(new I18NString(Languages.eng, "Bahrain"),                                        "BH", "BHR", 048, 973);
        public static readonly Country Bangladesh                               = new Country(new I18NString(Languages.eng, "Bangladesh"),                                     "BD", "BGD", 050, 880);
        public static readonly Country Barbados                                 = new Country(new I18NString(Languages.eng, "Barbados"),                                       "BB", "BRB", 052, 1246);
        //Barbuda	+1 268
        public static readonly Country Belarus                                  = new Country(new I18NString(Languages.eng, "Belarus"),                                        "BY", "BLR", 112, 375);
        public static readonly Country Belgium                                  = new Country(new I18NString(Languages.eng, "Belgium"),                                        "BE", "BEL", 056, 32);
        public static readonly Country Belize                                   = new Country(new I18NString(Languages.eng, "Belize"),                                         "BZ", "BLZ", 084, 501);
        public static readonly Country Benin                                    = new Country(new I18NString(Languages.eng, "Benin"),                                          "BJ", "BEN", 204, 229);
        public static readonly Country Bermuda                                  = new Country(new I18NString(Languages.eng, "Bermuda"),                                        "BM", "BMU", 060, 1441);
        public static readonly Country Bhutan                                   = new Country(new I18NString(Languages.eng, "Bhutan"),                                         "BT", "BTN", 064, 975);
        public static readonly Country Bolivia                                  = new Country(new I18NString(Languages.eng, "Bolivia"),                                        "BO", "BOL", 068, 591);
        //Bonaire	+599 7
        public static readonly Country BosniaAndHerzegovina                     = new Country(new I18NString(Languages.eng, "Bosnia and Herzegovina"),                         "BA", "BIH", 070, 387);
        public static readonly Country Botswana                                 = new Country(new I18NString(Languages.eng, "Botswana"),                                       "BW", "BWA", 072, 267);
        public static readonly Country BouvetIsland                             = new Country(new I18NString(Languages.eng, "Bouvet Island"),                                  "BV", "BVT", 074, 0);
        public static readonly Country Brazil                                   = new Country(new I18NString(Languages.eng, "Brazil"),                                         "BR", "BRA", 076, 55);
        public static readonly Country BritishIndianOceanTerritory              = new Country(new I18NString(Languages.eng, "British Indian Ocean Territory"),                 "IO", "IOT", 086, 246);
        public static readonly Country BritishVirginIslands                     = new Country(new I18NString(Languages.eng, "British Virgin Islands"),                         "VG", "VGB", 092, 1284);
        public static readonly Country BruneiDarussalam                         = new Country(new I18NString(Languages.eng, "Brunei Darussalam"),                              "BN", "BRN", 096, 673);
        public static readonly Country Bulgaria                                 = new Country(new I18NString(Languages.eng, "Bulgaria"),                                       "BG", "BGR", 100, 359);
        public static readonly Country BurkinaFaso                              = new Country(new I18NString(Languages.eng, "Burkina Faso"),                                   "BF", "BFA", 854, 226);
        //Burma	+95
        public static readonly Country Burundi                                  = new Country(new I18NString(Languages.eng, "Burundi"),                                        "BI", "BDI", 108, 257);
        public static readonly Country Cambodia                                 = new Country(new I18NString(Languages.eng, "Cambodia"),                                       "KH", "KHM", 116, 855);
        public static readonly Country Cameroon                                 = new Country(new I18NString(Languages.eng, "Cameroon"),                                       "CM", "CMR", 120, 237);
        public static readonly Country Canada                                   = new Country(new I18NString(Languages.eng, "Canada"),                                         "CA", "CAN", 124, 1);
        public static readonly Country CapeVerde                                = new Country(new I18NString(Languages.eng, "Cape Verde"),                                     "CV", "CPV", 132, 238);
        public static readonly Country CaymanIslands                            = new Country(new I18NString(Languages.eng, "Cayman Islands"),                                 "KY", "CYM", 136, 1345);
        public static readonly Country CentralAfricanRepublic                   = new Country(new I18NString(Languages.eng, "Central African Republic"),                       "CF", "CAF", 140, 236);
        public static readonly Country Chad                                     = new Country(new I18NString(Languages.eng, "Chad"),                                           "TD", "TCD", 148, 235);
        //Chatham Island, New Zealand	+64
        public static readonly Country Chile                                    = new Country(new I18NString(Languages.eng, "Chile"),                                          "CL", "CHL", 152, 56);
        public static readonly Country China                                    = new Country(new I18NString(Languages.eng, "China"),                                          "CN", "CHN", 156, 86);
        public static readonly Country ChristmasIsland                          = new Country(new I18NString(Languages.eng, "Christmas Island"),                               "CX", "CXR", 162, 61);
        public static readonly Country Cocos_Keeling_Islands                    = new Country(new I18NString(Languages.eng, "Cocos (Keeling) Islands"),                        "CC", "CCK", 166, 61);
        public static readonly Country Colombia                                 = new Country(new I18NString(Languages.eng, "Colombia"),                                       "CO", "COL", 170, 57);
        public static readonly Country Comoros                                  = new Country(new I18NString(Languages.eng, "Comoros"),                                        "KM", "COM", 174, 269);
        public static readonly Country Congo_Brazzaville                        = new Country(new I18NString(Languages.eng, "Congo (Brazzaville)"),                            "CG", "COG", 178, 242);
        public static readonly Country DemocraticRepublicOfTheCongo             = new Country(new I18NString(Languages.eng, "Democratic Republic of the Congo"),               "CD", "COD", 180, 243);
        public static readonly Country CookIslands                              = new Country(new I18NString(Languages.eng, "Cook Islands"),                                   "CK", "COK", 184, 682);
        public static readonly Country CostaRica                                = new Country(new I18NString(Languages.eng, "Costa Rica"),                                     "CR", "CRI", 188, 506);
        public static readonly Country CoteDIvoire                              = new Country(new I18NString(Languages.eng, "Côte d'Ivoire"),                                  "CI", "CIV", 384, 225);
        public static readonly Country Croatia                                  = new Country(new I18NString(Languages.eng, "Croatia"),                                        "HR", "HRV", 191, 385);
        public static readonly Country Cuba                                     = new Country(new I18NString(Languages.eng, "Cuba"),                                           "CU", "CUB", 192, 53);
        public static readonly Country Cyprus                                   = new Country(new I18NString(Languages.eng, "Cyprus"),                                         "CY", "CYP", 196, 357);
        public static readonly Country CzechRepublic                            = new Country(new I18NString(Languages.eng, "Czech Republic"),                                 "CZ", "CZE", 203, 420);
        public static readonly Country Denmark                                  = new Country(new I18NString(Languages.eng, "Denmark"),                                        "DK", "DNK", 208, 45);
        //Diego Garcia	+246
        public static readonly Country Djibouti                                 = new Country(new I18NString(Languages.eng, "Djibouti"),                                       "DJ", "DJI", 262, 253);
        public static readonly Country Dominica                                 = new Country(new I18NString(Languages.eng, "Dominica"),                                       "DM", "DMA", 212, 1767);
        public static readonly Country DominicanRepublic                        = new Country(new I18NString(Languages.eng, "Dominican Republic"),                             "DO", "DOM", 214, 1809);
        //East Timor	+670
        //Easter Island	+56
        public static readonly Country Ecuador                                  = new Country(new I18NString(Languages.eng, "Ecuador"),                                        "EC", "ECU", 218, 593);
        public static readonly Country Egypt                                    = new Country(new I18NString(Languages.eng, "Egypt"),                                          "EG", "EGY", 818, 20);
        public static readonly Country ElSalvador                               = new Country(new I18NString(Languages.eng, "El Salvador"),                                    "SV", "SLV", 222, 503);
        public static readonly Country EquatorialGuinea                         = new Country(new I18NString(Languages.eng, "Equatorial Guinea"),                              "GQ", "GNQ", 226, 240);
        public static readonly Country Eritrea                                  = new Country(new I18NString(Languages.eng, "Eritrea"),                                        "ER", "ERI", 232, 291);
        public static readonly Country Estonia                                  = new Country(new I18NString(Languages.eng, "Estonia"),                                        "EE", "EST", 233, 372);
        public static readonly Country Ethiopia                                 = new Country(new I18NString(Languages.eng, "Ethiopia"),                                       "ET", "ETH", 231, 251);
        public static readonly Country FalklandIslands_Malvinas                 = new Country(new I18NString(Languages.eng, "Falkland Islands (Malvinas)"),                    "FK", "FLK", 238, 500);
        public static readonly Country FaroeIslands                             = new Country(new I18NString(Languages.eng, "Faroe Islands"),                                  "FO", "FRO", 234, 298);
        public static readonly Country Fiji                                     = new Country(new I18NString(Languages.eng, "Fiji"),                                           "FJ", "FJI", 242, 679);
        public static readonly Country Finland                                  = new Country(new I18NString(Languages.eng, "Finland"),                                        "FI", "FIN", 246, 358);
        public static readonly Country France                                   = new Country(new I18NString(Languages.eng, "France"),                                         "FR", "FRA", 250, 33);
        public static readonly Country FrenchGuiana                             = new Country(new I18NString(Languages.eng, "French Guiana"),                                  "GF", "GUF", 254, 594);
        public static readonly Country FrenchPolynesia                          = new Country(new I18NString(Languages.eng, "French Polynesia"),                               "PF", "PYF", 258, 689);
        public static readonly Country FrenchSouthernTerritories                = new Country(new I18NString(Languages.eng, "French Southern Territories"),                    "TF", "ATF", 260, 596);
        public static readonly Country Gabon                                    = new Country(new I18NString(Languages.eng, "Gabon"),                                          "GA", "GAB", 266, 241);
        public static readonly Country Gambia                                   = new Country(new I18NString(Languages.eng, "Gambia"),                                         "GM", "GMB", 270, 220);
        public static readonly Country Georgia                                  = new Country(new I18NString(Languages.eng, "Georgia"),                                        "GE", "GEO", 268, 995);
        public static readonly Country Germany                                  = new Country(new I18NString(Languages.eng, "Germany").Add(Languages.deu, "Deutschland"),       "DE", "DEU", 276, 49);
        public static readonly Country Ghana                                    = new Country(new I18NString(Languages.eng, "Ghana"),                                          "GH", "GHA", 288, 233);
        public static readonly Country Gibraltar                                = new Country(new I18NString(Languages.eng, "Gibraltar"),                                      "GI", "GIB", 292, 350);
        public static readonly Country Greece                                   = new Country(new I18NString(Languages.eng, "Greece"),                                         "GR", "GRC", 300, 30);
        public static readonly Country Greenland                                = new Country(new I18NString(Languages.eng, "Greenland"),                                      "GL", "GRL", 304, 229);
        public static readonly Country Grenada                                  = new Country(new I18NString(Languages.eng, "Grenada"),                                        "GD", "GRD", 308, 1473);
        public static readonly Country Guadeloupe                               = new Country(new I18NString(Languages.eng, "Guadeloupe"),                                     "GP", "GLP", 312, 590);
        public static readonly Country Guam                                     = new Country(new I18NString(Languages.eng, "Guam"),                                           "GU", "GUM", 316, 1671);
        public static readonly Country Guatemala                                = new Country(new I18NString(Languages.eng, "Guatemala"),                                      "GT", "GTM", 320, 502);
        public static readonly Country Guernsey                                 = new Country(new I18NString(Languages.eng, "Guernsey"),                                       "GG", "GGY", 831, 44);
        public static readonly Country Guinea                                   = new Country(new I18NString(Languages.eng, "Guinea"),                                         "GN", "GIN", 324, 224);
        public static readonly Country GuineaBissau                             = new Country(new I18NString(Languages.eng, "Guinea-Bissau"),                                  "GW", "GNB", 624, 245);
        public static readonly Country Guyana                                   = new Country(new I18NString(Languages.eng, "Guyana"),                                         "GY", "GUY", 328, 592);
        public static readonly Country Haiti                                    = new Country(new I18NString(Languages.eng, "Haiti"),                                          "HT", "HTI", 332, 509);
        public static readonly Country HeardIslandAndMcdonaldIslands            = new Country(new I18NString(Languages.eng, "Heard Island and Mcdonald Islands"),              "HM", "HMD", 334, 0);
        public static readonly Country Honduras                                 = new Country(new I18NString(Languages.eng, "Honduras"),                                       "HN", "HND", 340, 504);
        public static readonly Country HongKong                                 = new Country(new I18NString(Languages.eng, "Hong Kong"),                                      "HK", "HKG", 344, 852);
        public static readonly Country Hungary                                  = new Country(new I18NString(Languages.eng, "Hungary"),                                        "HU", "HUN", 348, 36);
        public static readonly Country Iceland                                  = new Country(new I18NString(Languages.eng, "Iceland"),                                        "IS", "ISL", 352, 354);
        public static readonly Country India                                    = new Country(new I18NString(Languages.eng, "India"),                                          "IN", "IND", 356, 91);
        public static readonly Country Indonesia                                = new Country(new I18NString(Languages.eng, "Indonesia"),                                      "ID", "IDN", 360, 62);
        public static readonly Country IslamicRepublicOfIran                    = new Country(new I18NString(Languages.eng, "Islamic Republic of Iran"),                       "IR", "IRN", 364, 98);
        public static readonly Country Iraq                                     = new Country(new I18NString(Languages.eng, "Iraq"),                                           "IQ", "IRQ", 368, 964);
        public static readonly Country Ireland                                  = new Country(new I18NString(Languages.eng, "Ireland"),                                        "IE", "IRL", 372, 353);
        public static readonly Country IsleOfMan                                = new Country(new I18NString(Languages.eng, "Isle of Man"),                                    "IM", "IMN", 833, 44);
        public static readonly Country Israel                                   = new Country(new I18NString(Languages.eng, "Israel"),                                         "IL", "ISR", 376, 972);
        public static readonly Country Italy                                    = new Country(new I18NString(Languages.eng, "Italy"),                                          "IT", "ITA", 380, 39);
        public static readonly Country Jamaica                                  = new Country(new I18NString(Languages.eng, "Jamaica"),                                        "JM", "JAM", 388, 1876);
        //Jan Mayen+47 79
        public static readonly Country Japan                                    = new Country(new I18NString(Languages.eng, "Japan"),                                          "JP", "JPN", 392, 81);
        public static readonly Country Jersey                                   = new Country(new I18NString(Languages.eng, "Jersey"),                                         "JE", "JEY", 832, 44);
        public static readonly Country Jordan                                   = new Country(new I18NString(Languages.eng, "Jordan"),                                         "JO", "JOR", 400, 962);
        public static readonly Country Kazakhstan                               = new Country(new I18NString(Languages.eng, "Kazakhstan"),                                     "KZ", "KAZ", 398, 76);
        public static readonly Country Kenya                                    = new Country(new I18NString(Languages.eng, "Kenya"),                                          "KE", "KEN", 404, 254);
        public static readonly Country Kiribati                                 = new Country(new I18NString(Languages.eng, "Kiribati"),                                       "KI", "KIR", 296, 686);
        public static readonly Country DemocraticPeoplesRepublicOfKorea         = new Country(new I18NString(Languages.eng, "Democratic People's Republic of Korea"),          "KP", "PRK", 408, 850);
        public static readonly Country RepublicOfKorea                          = new Country(new I18NString(Languages.eng, "Republic of Korea"),                              "KR", "KOR", 410, 82);
        public static readonly Country Kuwait                                   = new Country(new I18NString(Languages.eng, "Kuwait"),                                         "KW", "KWT", 414, 965);
        public static readonly Country Kyrgyzstan                               = new Country(new I18NString(Languages.eng, "Kyrgyzstan"),                                     "KG", "KGZ", 417, 996);
        public static readonly Country LaoPDR                                   = new Country(new I18NString(Languages.eng, "Lao PDR"),                                        "LA", "LAO", 418, 856);
        public static readonly Country Latvia                                   = new Country(new I18NString(Languages.eng, "Latvia"),                                         "LV", "LVA", 428, 371);
        public static readonly Country Lebanon                                  = new Country(new I18NString(Languages.eng, "Lebanon"),                                        "LB", "LBN", 422, 961);
        public static readonly Country Lesotho                                  = new Country(new I18NString(Languages.eng, "Lesotho"),                                        "LS", "LSO", 426, 266);
        public static readonly Country Liberia                                  = new Country(new I18NString(Languages.eng, "Liberia"),                                        "LR", "LBR", 430, 231);
        public static readonly Country Libya                                    = new Country(new I18NString(Languages.eng, "Libya"),                                          "LY", "LBY", 434, 218);
        public static readonly Country Liechtenstein                            = new Country(new I18NString(Languages.eng, "Liechtenstein"),                                  "LI", "LIE", 438, 423);
        public static readonly Country Lithuania                                = new Country(new I18NString(Languages.eng, "Lithuania"),                                      "LT", "LTU", 440, 370);
        public static readonly Country Luxembourg                               = new Country(new I18NString(Languages.eng, "Luxembourg"),                                     "LU", "LUX", 442, 352);
        public static readonly Country RepublicOfMacedonia                      = new Country(new I18NString(Languages.eng, "Republic of Macedonia"),                          "MK", "MKD", 807, 389);
        public static readonly Country Macau                                    = new Country(new I18NString(Languages.eng, "Macau"),                                          "MO", "MAC", 446, 835);
        public static readonly Country Madagascar                               = new Country(new I18NString(Languages.eng, "Madagascar"),                                     "MG", "MDG", 450, 261);
        public static readonly Country Malawi                                   = new Country(new I18NString(Languages.eng, "Malawi"),                                         "MW", "MWI", 454, 265);
        public static readonly Country Malaysia                                 = new Country(new I18NString(Languages.eng, "Malaysia"),                                       "MY", "MYS", 458, 60);
        public static readonly Country Maldives                                 = new Country(new I18NString(Languages.eng, "Maldives"),                                       "MV", "MDV", 462, 960);
        public static readonly Country Mali                                     = new Country(new I18NString(Languages.eng, "Mali"),                                           "ML", "MLI", 466, 223);
        public static readonly Country Malta                                    = new Country(new I18NString(Languages.eng, "Malta"),                                          "MT", "MLT", 470, 356);
        public static readonly Country MarshallIslands                          = new Country(new I18NString(Languages.eng, "Marshall Islands"),                               "MH", "MHL", 584, 692);
        public static readonly Country Martinique                               = new Country(new I18NString(Languages.eng, "Martinique"),                                     "MQ", "MTQ", 474, 596);
        public static readonly Country Mauritania                               = new Country(new I18NString(Languages.eng, "Mauritania"),                                     "MR", "MRT", 478, 222);
        public static readonly Country Mauritius                                = new Country(new I18NString(Languages.eng, "Mauritius"),                                      "MU", "MUS", 480, 230);
        public static readonly Country Mayotte                                  = new Country(new I18NString(Languages.eng, "Mayotte"),                                        "YT", "MYT", 175, 262);
        public static readonly Country Mexico                                   = new Country(new I18NString(Languages.eng, "Mexico"),                                         "MX", "MEX", 484, 52);
        public static readonly Country FederatedStatesOfMicronesia              = new Country(new I18NString(Languages.eng, "Federated States of Micronesia"),                 "FM", "FSM", 583, 691);
        //Midway Island, USA	+1 808
        public static readonly Country Moldova                                  = new Country(new I18NString(Languages.eng, "Moldova"),                                        "MD", "MDA", 498, 373);
        public static readonly Country Monaco                                   = new Country(new I18NString(Languages.eng, "Monaco"),                                         "MC", "MCO", 492, 377);
        public static readonly Country Mongolia                                 = new Country(new I18NString(Languages.eng, "Mongolia"),                                       "MN", "MNG", 496, 976);
        public static readonly Country Montenegro                               = new Country(new I18NString(Languages.eng, "Montenegro"),                                     "ME", "MNE", 499, 382);
        public static readonly Country Montserrat                               = new Country(new I18NString(Languages.eng, "Montserrat"),                                     "MS", "MSR", 500, 1664);
        public static readonly Country Morocco                                  = new Country(new I18NString(Languages.eng, "Morocco"),                                        "MA", "MAR", 504, 212);
        public static readonly Country Mozambique                               = new Country(new I18NString(Languages.eng, "Mozambique"),                                     "MZ", "MOZ", 508, 258);
        public static readonly Country Myanmar                                  = new Country(new I18NString(Languages.eng, "Myanmar"),                                        "MM", "MMR", 104, 264);
        public static readonly Country Namibia                                  = new Country(new I18NString(Languages.eng, "Namibia"),                                        "NA", "NAM", 516, 264);
        public static readonly Country Nauru                                    = new Country(new I18NString(Languages.eng, "Nauru"),                                          "NR", "NRU", 520, 674);
        public static readonly Country Nepal                                    = new Country(new I18NString(Languages.eng, "Nepal"),                                          "NP", "NPL", 524, 977);
        public static readonly Country Netherlands                              = new Country(new I18NString(Languages.eng, "Netherlands"),                                    "NL", "NLD", 528, 31);
        public static readonly Country NetherlandsAntilles                      = new Country(new I18NString(Languages.eng, "Netherlands Antilles"),                           "AN", "ANT", 530, 1869);
        public static readonly Country NewCaledonia                             = new Country(new I18NString(Languages.eng, "New Caledonia"),                                  "NC", "NCL", 540, 687);
        public static readonly Country NewZealand                               = new Country(new I18NString(Languages.eng, "New Zealand"),                                    "NZ", "NZL", 554, 64);
        public static readonly Country Nicaragua                                = new Country(new I18NString(Languages.eng, "Nicaragua"),                                      "NI", "NIC", 558, 505);
        public static readonly Country Niger                                    = new Country(new I18NString(Languages.eng, "Niger"),                                          "NE", "NER", 562, 227);
        public static readonly Country Nigeria                                  = new Country(new I18NString(Languages.eng, "Nigeria"),                                        "NG", "NGA", 566, 234);
        public static readonly Country Niue                                     = new Country(new I18NString(Languages.eng, "Niue"),                                           "NU", "NIU", 570, 683);
        public static readonly Country NorfolkIsland                            = new Country(new I18NString(Languages.eng, "Norfolk Island"),                                 "NF", "NFK", 574, 672);
        public static readonly Country NorthernMarianaIslands                   = new Country(new I18NString(Languages.eng, "Northern Mariana Islands"),                       "MP", "MNP", 580, 1670);
        public static readonly Country Norway                                   = new Country(new I18NString(Languages.eng, "Norway"),                                         "NO", "NOR", 578, 47);
        public static readonly Country Oman                                     = new Country(new I18NString(Languages.eng, "Oman"),                                           "OM", "OMN", 512, 968);
        public static readonly Country Pakistan                                 = new Country(new I18NString(Languages.eng, "Pakistan"),                                       "PK", "PAK", 586, 92);
        public static readonly Country Palau                                    = new Country(new I18NString(Languages.eng, "Palau"),                                          "PW", "PLW", 585, 680);
        public static readonly Country PalestinianTerritory                     = new Country(new I18NString(Languages.eng, "Palestinian Territory"),                          "PS", "PSE", 275, 970);
        public static readonly Country Panama                                   = new Country(new I18NString(Languages.eng, "Panama"),                                         "PA", "PAN", 591, 507);
        public static readonly Country PapuaNewGuinea                           = new Country(new I18NString(Languages.eng, "Papua New Guinea"),                               "PG", "PNG", 598, 675);
        public static readonly Country Paraguay                                 = new Country(new I18NString(Languages.eng, "Paraguay"),                                       "PY", "PRY", 600, 595);
        public static readonly Country Peru                                     = new Country(new I18NString(Languages.eng, "Peru"),                                           "PE", "PER", 604, 51);
        public static readonly Country Philippines                              = new Country(new I18NString(Languages.eng, "Philippines"),                                    "PH", "PHL", 608, 63);
        public static readonly Country Pitcairn                                 = new Country(new I18NString(Languages.eng, "Pitcairn"),                                       "PN", "PCN", 612, 64);
        public static readonly Country Poland                                   = new Country(new I18NString(Languages.eng, "Poland"),                                         "PL", "POL", 616, 48);
        public static readonly Country Portugal                                 = new Country(new I18NString(Languages.eng, "Portugal"),                                       "PT", "PRT", 620, 351);
        public static readonly Country PuertoRico                               = new Country(new I18NString(Languages.eng, "Puerto Rico"),                                    "PR", "PRI", 630, 1787);
        public static readonly Country Qatar                                    = new Country(new I18NString(Languages.eng, "Qatar"),                                          "QA", "QAT", 634, 974);
        public static readonly Country Réunion                                  = new Country(new I18NString(Languages.eng, "Réunion"),                                        "RE", "REU", 638, 262);
        public static readonly Country Romania                                  = new Country(new I18NString(Languages.eng, "Romania"),                                        "RO", "ROU", 642, 40);
        public static readonly Country RussianFederation                        = new Country(new I18NString(Languages.eng, "Russian Federation"),                             "RU", "RUS", 643, 7);
        public static readonly Country Rwanda                                   = new Country(new I18NString(Languages.eng, "Rwanda"),                                         "RW", "RWA", 646, 250);
        //Saba	+599 4
        public static readonly Country SaintBarthelemy                          = new Country(new I18NString(Languages.eng, "Saint-Barthélemy"),                               "BL", "BLM", 652, 590);
        public static readonly Country SaintHelena                              = new Country(new I18NString(Languages.eng, "Saint Helena"),                                   "SH", "SHN", 654, 290);
        public static readonly Country SaintKittsAndNevis                       = new Country(new I18NString(Languages.eng, "Saint Kitts and Nevis"),                          "KN", "KNA", 659, 1869);
        public static readonly Country SaintLucia                               = new Country(new I18NString(Languages.eng, "Saint Lucia"),                                    "LC", "LCA", 662, 1758);
        public static readonly Country SaintMartin                              = new Country(new I18NString(Languages.eng, "Saint-Martin (French part)"),                     "MF", "MAF", 663, 590);
        public static readonly Country SaintPierreAndMiquelon                   = new Country(new I18NString(Languages.eng, "Saint Pierre and Miquelon"),                      "PM", "SPM", 666, 508);
        public static readonly Country SaintVincentAndGrenadines                = new Country(new I18NString(Languages.eng, "Saint Vincent and Grenadines"),                   "VC", "VCT", 670, 1784);
        public static readonly Country Samoa                                    = new Country(new I18NString(Languages.eng, "Samoa"),                                          "WS", "WSM", 882, 685);
        public static readonly Country SanMarino                                = new Country(new I18NString(Languages.eng, "San Marino"),                                     "SM", "SMR", 674, 378);
        public static readonly Country SaoTomeAndPrincipe                       = new Country(new I18NString(Languages.eng, "São Tomé and Príncipe"),                          "ST", "STP", 678, 239);
        public static readonly Country SaudiArabia                              = new Country(new I18NString(Languages.eng, "Saudi Arabia"),                                   "SA", "SAU", 682, 966);
        public static readonly Country Senegal                                  = new Country(new I18NString(Languages.eng, "Senegal"),                                        "SN", "SEN", 686, 221);
        public static readonly Country Serbia                                   = new Country(new I18NString(Languages.eng, "Serbia"),                                         "RS", "SRB", 688, 381);
        public static readonly Country Seychelles                               = new Country(new I18NString(Languages.eng, "Seychelles"),                                     "SC", "SYC", 690, 248);
        public static readonly Country SierraLeone                              = new Country(new I18NString(Languages.eng, "Sierra Leone"),                                   "SL", "SLE", 694, 232);
        public static readonly Country Singapore                                = new Country(new I18NString(Languages.eng, "Singapore"),                                      "SG", "SGP", 702, 65);
        //Sint Eustatius	+599 3
        //Sint Maarten (Netherlands)	+1 721
        public static readonly Country Slovakia                                 = new Country(new I18NString(Languages.eng, "Slovakia"),                                       "SK", "SVK", 703, 421);
        public static readonly Country Slovenia                                 = new Country(new I18NString(Languages.eng, "Slovenia"),                                       "SI", "SVN", 705, 386);
        public static readonly Country SolomonIslands                           = new Country(new I18NString(Languages.eng, "Solomon Islands"),                                "SB", "SLB", 090, 677);
        public static readonly Country Somalia                                  = new Country(new I18NString(Languages.eng, "Somalia"),                                        "SO", "SOM", 706, 252);
        public static readonly Country SouthAfrica                              = new Country(new I18NString(Languages.eng, "South Africa"),                                   "ZA", "ZAF", 710, 27);
        public static readonly Country SouthGeorgiaAndTheSouthSandwichIslands   = new Country(new I18NString(Languages.eng, "South Georgia and the South Sandwich Islands"),   "GS", "SGS", 239, 500);
        //South Ossetia	+995 34
        public static readonly Country SouthSudan                               = new Country(new I18NString(Languages.eng, "South Sudan"),                                    "SS", "SSD", 728, 211);
        public static readonly Country Spain                                    = new Country(new I18NString(Languages.eng, "Spain"),                                          "ES", "ESP", 724, 34);
        public static readonly Country SriLanka                                 = new Country(new I18NString(Languages.eng, "Sri Lanka"),                                      "LK", "LKA", 144, 94);
        public static readonly Country Sudan                                    = new Country(new I18NString(Languages.eng, "Sudan"),                                          "SD", "SDN", 736, 249);
        public static readonly Country Suriname                                 = new Country(new I18NString(Languages.eng, "Suriname"),                                       "SR", "SUR", 740, 597);
        public static readonly Country SvalbardAndJanMayenIslands               = new Country(new I18NString(Languages.eng, "Svalbard and Jan Mayen Islands"),                 "SJ", "SJM", 744, 4779);
        public static readonly Country Swaziland                                = new Country(new I18NString(Languages.eng, "Swaziland"),                                      "SZ", "SWZ", 748, 268);
        public static readonly Country Sweden                                   = new Country(new I18NString(Languages.eng, "Sweden"),                                         "SE", "SWE", 752, 46);
        public static readonly Country Switzerland                              = new Country(new I18NString(Languages.eng, "Switzerland"),                                    "CH", "CHE", 756, 41);
        public static readonly Country SyrianArabRepublic                       = new Country(new I18NString(Languages.eng, "Syrian Arab Republic"),                           "SY", "SYR", 760, 963);
        public static readonly Country Taiwan                                   = new Country(new I18NString(Languages.eng, "Taiwan"),                                         "TW", "TWN", 158, 886);
        public static readonly Country Tajikistan                               = new Country(new I18NString(Languages.eng, "Tajikistan"),                                     "TJ", "TJK", 762, 992);
        public static readonly Country UnitedRepublicOfTanzania                 = new Country(new I18NString(Languages.eng, "United Republic of Tanzania"),                    "TZ", "TZA", 834, 255);
        public static readonly Country Thailand                                 = new Country(new I18NString(Languages.eng, "Thailand"),                                       "TH", "THA", 764, 66);
        public static readonly Country TimorLeste                               = new Country(new I18NString(Languages.eng, "Timor-Leste"),                                    "TL", "TLS", 626, 0);
        public static readonly Country Togo                                     = new Country(new I18NString(Languages.eng, "Togo"),                                           "TG", "TGO", 768, 228);
        public static readonly Country Tokelau                                  = new Country(new I18NString(Languages.eng, "Tokelau"),                                        "TK", "TKL", 772, 690);
        public static readonly Country Tonga                                    = new Country(new I18NString(Languages.eng, "Tonga"),                                          "TO", "TON", 776, 676);
        public static readonly Country TrinidadAndTobago                        = new Country(new I18NString(Languages.eng, "Trinidad and Tobago"),                            "TT", "TTO", 780, 1868);
        public static readonly Country Tunisia                                  = new Country(new I18NString(Languages.eng, "Tunisia"),                                        "TN", "TUN", 788, 216);
        public static readonly Country Turkey                                   = new Country(new I18NString(Languages.eng, "Turkey"),                                         "TR", "TUR", 792, 90);
        public static readonly Country Turkmenistan                             = new Country(new I18NString(Languages.eng, "Turkmenistan"),                                   "TM", "TKM", 795, 993);
        public static readonly Country TurksAndCaicosIslands                    = new Country(new I18NString(Languages.eng, "Turks and Caicos Islands"),                       "TC", "TCA", 796, 1649);
        public static readonly Country Tuvalu                                   = new Country(new I18NString(Languages.eng, "Tuvalu"),                                         "TV", "TUV", 798, 688);
        public static readonly Country Uganda                                   = new Country(new I18NString(Languages.eng, "Uganda"),                                         "UG", "UGA", 800, 256);
        public static readonly Country Ukraine                                  = new Country(new I18NString(Languages.eng, "Ukraine"),                                        "UA", "UKR", 804, 380);
        public static readonly Country UnitedArabEmirates                       = new Country(new I18NString(Languages.eng, "United Arab Emirates"),                           "AE", "ARE", 784, 971);
        public static readonly Country UnitedKingdom                            = new Country(new I18NString(Languages.eng, "United Kingdom"),                                 "GB", "GBR", 826, 44);
        public static readonly Country UnitedStatesOfAmerica                    = new Country(new I18NString(Languages.eng, "United States of America"),                       "US", "USA", 840, 1);
        public static readonly Country UnitedStatesMinorOutlyingIslands         = new Country(new I18NString(Languages.eng, "United States Minor Outlying Islands"),           "UM", "UMI", 581, 1);
        public static readonly Country Uruguay                                  = new Country(new I18NString(Languages.eng, "Uruguay"),                                        "UY", "URY", 858, 598);
        public static readonly Country Uzbekistan                               = new Country(new I18NString(Languages.eng, "Uzbekistan"),                                     "UZ", "UZB", 860, 998);
        public static readonly Country Vanuatu                                  = new Country(new I18NString(Languages.eng, "Vanuatu"),                                        "VU", "VUT", 548, 678);
        public static readonly Country VaticanState                             = new Country(new I18NString(Languages.eng, "Vatican City State (Holy See)"),                  "VA", "VAT", 336, 379);
        public static readonly Country Venezuela                                = new Country(new I18NString(Languages.eng, "Bolivarian Republic of Venezuela"),               "VE", "VEN", 862, 58);
        public static readonly Country Vietnam                                  = new Country(new I18NString(Languages.eng, "Vietnam"),                                        "VN", "VNM", 704, 84);
        public static readonly Country VirginIslands                            = new Country(new I18NString(Languages.eng, "Virgin Islands"),                                 "VI", "VIR", 850, 1340);
        //Wake Island, USA	+1 808
        public static readonly Country WallisAndFutunaIslands                   = new Country(new I18NString(Languages.eng, "Wallis and Futuna Islands"),                      "WF", "WLF", 876, 681);
        public static readonly Country WesternSahara                            = new Country(new I18NString(Languages.eng, "Western Sahara"),                                 "EH", "ESH", 732, 0);
        public static readonly Country Yemen                                    = new Country(new I18NString(Languages.eng, "Yemen"),                                          "YE", "YEM", 887, 967);
        public static readonly Country Zambia                                   = new Country(new I18NString(Languages.eng, "Zambia"),                                         "ZM", "ZMB", 894, 260);
        //Zanzibar	+255
        public static readonly Country Zimbabwe                                 = new Country(new I18NString(Languages.eng, "Zimbabwe"),                                       "ZW", "ZWE", 716, 263);

        public static readonly Country Testland                                 = new Country(new I18NString(Languages.eng, "Testland"),                                       "YY", "YYY", 999, 999);

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

                        CountryNames.Add(_Country.CountryName, _Country);
                        Alpha2Codes. Add(_Country.Alpha2Code,  _Country);
                        Alpha3Codes. Add(_Country.Alpha3Code,  _Country);
                        NumericCodes.Add(_Country.NumericCode, _Country);

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

            if (AnyString.Length == 2)
                return ParseAlpha2Code(AnyString);

            else if (AnyString.Length == 3)
                return ParseAlpha3Code(AnyString);

            else if (TryParseNumericCode(AnyString, out Country _Country))
                return _Country;

            else if (TryParseTelefonCode(AnyString, out _Country))
                return _Country;

            return ParseCountryName(AnyString);

        }

        #endregion

        #region TryParse(AnyString, out CountryValue)

        /// <summary>
        /// Tries to find the appropriate country for the given string.
        /// </summary>
        /// <param name="AnyString">Any string.</param>
        /// <param name="CountryValue">The country.</param>
        public static Boolean TryParse(String AnyString, out Country CountryValue)
        {

            if (TryParseAlpha2Code (AnyString, out CountryValue))
                return true;

            if (TryParseAlpha3Code (AnyString, out CountryValue))
                return true;

            if (TryParseNumericCode(AnyString, out CountryValue))
                return true;

            if (TryParseTelefonCode(AnyString, out CountryValue))
                return true;

            if (TryParseCountryName(AnyString, out CountryValue))
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

            ReflectData();

            foreach (var countryname in CountryNames)
                foreach (var I8Name in countryname.Key)
                    if (I8Name.Text == CountryName)
                        return countryname.Value;

            return null;

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

            ReflectData();

            Country _Country;

            if (Alpha2Codes.TryGetValue(Alpha2Code, out _Country))
                return _Country;

            return null;

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
            ReflectData();
            return Alpha2Codes.TryGetValue(Alpha2Code, out Country);
        }

        #endregion


        #region ParseAlpha3Code(Alpha3Code)

        /// <summary>
        /// Tries to find the appropriate country for the given alpha3code.
        /// </summary>
        /// <param name="Alpha3Code">An alpha3code for a country, e.g. "DEU" for Germany.</param>
        public static Country ParseAlpha3Code(String Alpha3Code)
        {

            ReflectData();

            Country _Country;

            if (Alpha3Codes.TryGetValue(Alpha3Code, out _Country))
                return _Country;

            return null;

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
            ReflectData();
            return Alpha3Codes.TryGetValue(Alpha3Code, out Country);
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

            // If one is null, but not both, return false.
            if (((Object) Country1 == null) || ((Object) Country2 == null))
                return false;

            if ((Object) Country1 == null)
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
        {

            if ((Object) Country1 == null)
                throw new ArgumentNullException(nameof(Country1),  "The given country must not be null!");

            return Country1.CompareTo(Country2) < 0;

        }

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
        {

            if ((Object) Country1 == null)
                throw new ArgumentNullException(nameof(Country1),  "The given country must not be null!");

            return Country1.CompareTo(Country2) > 0;

        }

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
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var Country = Object as Country;
            if ((Object) Country == null)
                throw new ArgumentException("The given object is not a country!");

            return CompareTo(Country);

        }

        #endregion

        #region CompareTo(Country)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Country">An object to compare with.</param>
        public Int32 CompareTo(Country Country)
        {

            if ((Object) Country == null)
                throw new ArgumentNullException(nameof(Country), "The given country must not be null!");

            return String.Compare(Alpha2Code, Country.Alpha2Code, StringComparison.Ordinal);

        }

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
        {

            if (Object == null)
                return false;

            var Country = Object as Country;
            if ((Object) Country == null)
                return false;

            return this.Equals(Country);

        }

        #endregion

        #region Equals(Country)

        /// <summary>
        /// Compares two Countrys for equality.
        /// </summary>
        /// <param name="Country">A Country to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Country Country)
        {

            if ((Object) Country == null)
                return false;

            return Alpha2Code == Country.Alpha2Code;

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => CountryName.GetHashCode() ^
               Alpha2Code. GetHashCode() ^
               Alpha3Code. GetHashCode() ^
               NumericCode.GetHashCode() ^
               TelefonCode.GetHashCode();

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
