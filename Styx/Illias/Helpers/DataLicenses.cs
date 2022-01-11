/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The data licenses within the electric vehicle domain.
    /// </summary>
    public class DataLicense
    {

        #region Properties

        /// <summary>
        /// The unique identification of the data license.
        /// </summary>
        public DataLicense_Id       Id            { get; }

        /// <summary>
        /// The description of the data license.
        /// </summary>
        public String               Description   { get; }

        /// <summary>
        /// Optional URIs for more information.
        /// </summary>
        public IEnumerable<String>  URIs          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data license.
        /// </summary>
        /// <param name="Id">The unique identification of the data license.</param>
        /// <param name="Description">The description of the data license.</param>
        /// <param name="URIs">Optional URIs for more information.</param>
        public DataLicense(DataLicense_Id   Id,
                           String           Description,
                           params String[]  URIs)
        {

            #region Initial checks

            if (Description.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Description),  "The description of the data license must not be null or empty!");

            #endregion

            this.Id           = Id;
            this.Description  = Description;
            this.URIs         = URIs ?? new String[0];

        }

        #endregion


        #region (static) Definitions

        /// <summary>
        /// No license, ask the data source for more details.
        /// </summary>
        public static readonly DataLicense None                              = new DataLicense(DataLicense_Id.Parse("None"), "None", "");



        // Open Data licenses

        /// <summary>
        /// Open Data Commons: Public Domain Dedication and License (PDDL)
        /// </summary>
        public static readonly DataLicense PublicDomainDedicationAndLicense  = new DataLicense(DataLicense_Id.Parse("PDDL"),
                                                                                               "Open Data Commons: Public Domain Dedication and License",
                                                                                               "http://opendatacommons.org/licenses/pddl/");

        /// <summary>
        /// Open Data Commons: Attribution License (ODC-By)
        /// </summary>
        public static readonly DataLicense AttributionLicense                = new DataLicense(DataLicense_Id.Parse("ODC-By"),
                                                                                               "Open Data Commons: Attribution License",
                                                                                               "http://opendatacommons.org/licenses/by/");

        /// <summary>
        /// Open Data Commons: Open Data Commons Open Database License (ODbL)
        /// Attribution and Share-Alike for Data/Databases
        /// </summary>
        public static readonly DataLicense OpenDatabaseLicense               = new DataLicense(DataLicense_Id.Parse("ODbL"),
                                                                                               "Open Data Commons: Open Data Commons Open Database License",
                                                                                               "http://opendatacommons.org/licenses/odbl/",
                                                                                               "http://opendatacommons.org/licenses/odbl/summary/",
                                                                                               "http://opendatacommons.org/licenses/odbl/1.0/");




        // Special German licenses

        /// <summary>
        /// Datenlizenz Deutschland – Namensnennung – Version 2.0
        /// </summary>
        public static readonly DataLicense DatenlizenzDeutschland_BY_2       = new DataLicense(DataLicense_Id.Parse("dl-de/by-2-0"),
                                                                                               "Datenlizenz Deutschland – Namensnennung – Version 2.0",
                                                                                               "https://www.govdata.de/dl-de/by-2-0");

        /// <summary>
        /// Datenlizenz Deutschland – Namensnennung – Version 2.0
        /// </summary>
        public static readonly DataLicense DatenlizenzDeutschland_Zero_2     = new DataLicense(DataLicense_Id.Parse("dl-de/zero-2-0"),
                                                                                               "Datenlizenz Deutschland – Namensnennung – Version 2.0",
                                                                                               "https://www.govdata.de/dl-de/zero-2-0");

        /// <summary>
        /// GeoLizenz V1.3 – Open
        /// </summary>
        public static readonly DataLicense GeoLizenz_OpenData_1_3_1          = new DataLicense(DataLicense_Id.Parse("GeoLizenz_V1.3"),
                                                                                               "GeoLizenz V1.3 – Open",
                                                                                               "https://www.geolizenz.org/index/page.php?p=GL/opendata",
                                                                                               "https://www.geolizenz.org/modules/geolizenz/docs/1.3.1/GeoLizenz_V1.3_Open_050615_V1.pdf",
                                                                                               "https://www.geolizenz.org/modules/geolizenz/docs/1.3.1/Erl%C3%A4uterungen_GeoLizenzV1.3_Open_06.06.2015_V1.pdf");




        // Creative Commons licenses

        /// <summary>
        /// Creative Commons Attribution 4.0 International (CC BY 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_4              = new DataLicense(DataLicense_Id.Parse("CC BY 4.0"),
                                                                                               "Creative Commons Attribution 4.0 International",
                                                                                               "http://creativecommons.org/licenses/by/4.0/",
                                                                                               "http://creativecommons.org/licenses/by/4.0/legalcode");

        /// <summary>
        /// Creative Commons Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_SA_4           = new DataLicense(DataLicense_Id.Parse("CC BY-SA 4.0"),
                                                                                               "Creative Commons Attribution-ShareAlike 4.0 International",
                                                                                               "http://creativecommons.org/licenses/by-sa/4.0/",
                                                                                               "http://creativecommons.org/licenses/by-sa/4.0/legalcode");

        /// <summary>
        /// Creative Commons Attribution-NoDerivs 4.0 International (CC BY-ND 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_ND_4           = new DataLicense(DataLicense_Id.Parse("CC BY-ND 4.0"),
                                                                                               "Creative Commons Attribution-NoDerivs 4.0 International",
                                                                                               "http://creativecommons.org/licenses/by-nd/4.0/",
                                                                                               "http://creativecommons.org/licenses/by-nd/4.0/legalcode");

        /// <summary>
        /// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_NC_4           = new DataLicense(DataLicense_Id.Parse("CC BY-NC 4.0"),
                                                                                               "Creative Commons Attribution-NonCommercial 4.0 International",
                                                                                               "http://creativecommons.org/licenses/by-nc/4.0/",
                                                                                               "http://creativecommons.org/licenses/by-nc/4.0/legalcode");

        /// <summary>
        /// Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_NC_SA_4        = new DataLicense(DataLicense_Id.Parse("CC BY-NC-SA 4.0"),
                                                                                               "Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International",
                                                                                               "http://creativecommons.org/licenses/by-nc-sa/4.0/",
                                                                                               "http://creativecommons.org/licenses/by-nc-sa/4.0/legalcode");

        /// <summary>
        /// Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International (CC BY-NC-ND 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_NC_ND_4        = new DataLicense(DataLicense_Id.Parse("CC BY-NC-ND 4.0"),
                                                                                               "Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International",
                                                                                               "http://creativecommons.org/licenses/by-nc-nd/4.0/",
                                                                                               "http://creativecommons.org/licenses/by-nc-nd/4.0/legalcode");

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Description;

        #endregion

    }

}
