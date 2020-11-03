/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class LanguagesExtentions
    {

        public static String AsText(this Languages Language)
        {
            switch (Language)
            {

                default:
                    return Language.ToString();

            }
        }

        public static Languages Parse(String Text)
        {
            switch (Text?.ToLower())
            {

                default:
                    return (Languages) Enum.Parse(typeof(Languages), Text, true);

            }
        }


    }

    /// <summary>
    /// ISO 639-1/3 codes for languages.
    /// http://www.loc.gov/standards/iso639-2/php/code_list.php
    /// </summary>
    public enum Languages
    {

        unknown,

        aa,
        ab,
        ae,
        af,
        ak,
        am,
        an,
        ar,
        @as,
        av,
        ay,
        az,
        ba,
        be,
        bg,
        bh,
        bi,
        bm,
        bn,
        bo,
        br,
        bs,
        ca,
        ce,
        ch,
        co,
        cr,
        cs,
        cu,
        cv,
        cy,
        da,
        de,
        dv,
        dz,
        ee,
        el,
        en,
        eo,
        es,
        et,
        eu,
        fa,
        ff,
        fi,
        fj,
        fo,
        fr,
        fy,
        ga,
        gd,
        gl,
        gn,
        gu,
        gv,
        ha,
        he,
        hi,
        ho,
        hr,
        ht,
        hu,
        hy,
        hz,
        ia,
        id,
        ie,
        ig,
        ii,
        ik,
        io,
        @is,
        it,
        iu,
        ja,
        jv,
        ka,
        kg,
        ki,
        kj,
        kk,
        kl,
        km,
        kn,
        ko,
        kr,
        ks,
        ku,
        kv,
        kw,
        ky,
        la,
        lb,
        lg,
        li,
        ln,
        lo,
        lt,
        lu,
        lv,
        mg,
        mh,
        mi,
        mk,
        ml,
        mn,
        mr,
        ms,
        mt,
        my,
        na,
        nb,
        nd,
        ne,
        ng,
        nl,
        nn,
        no,
        nr,
        nv,
        ny,
        oc,
        oj,
        om,
        or,
        os,
        pa,
        pi,
        pl,
        ps,
        pt,
        qu,
        rm,
        rn,
        ro,
        ru,
        rw,
        sa,
        sc,
        sd,
        se,
        sg,
        si,
        sk,
        sl,
        sm,
        sn,
        so,
        sq,
        sr,
        ss,
        st,
        su,
        sv,
        sw,
        ta,
        te,
        tg,
        th,
        ti,
        tk,
        tl,
        tn,
        to,
        tr,
        ts,
        tt,
        tw,
        ty,
        ug,
        uk,
        ur,
        uz,
        ve,
        vi,
        vo,
        wa,
        wo,
        xh,
        yi,
        yo,
        za,
        zh,
        zu,


        // ISO 639-3 codes for languages.
        // http://www-01.sil.org/iso639-3/codes.asp?order=639_3
        aaa,
        aab,
        aac,
        aad,
        aae,
        aaf,
        aag,
        aah,
        aai,
        aak,
        aal,
        aan,
        aao,
        aap,
        aaq,
        aar,
        aas,
        aat,
        aau,
        aav,
        aaw,
        aax,
        aaz,
        aba,
        abb,
        abc,
        abd,
        abe,
        abf,
        abg,
        abh,
        abi,
        abj,
        abk,
        abl,
        abm,
        abn,
        abo,
        abp,
        abq,
        abr,
        abs,
        abt,
        abu,
        abv,
        abw,
        abx,
        aby,
        abz,
        aca,
        acb,
        acd,
        ace,
        acf,
        ach,
        aci,
        ack,
        acl,
        acm,
        acn,
        acp,
        acq,
        acr,
        acs,
        act,
        acu,
        acv,
        acw,
        acx,
        acy,
        acz,
        ada,
        adb,
        add,
        ade,
        adf,
        adg,
        adh,
        adi,
        adj,
        adl,
        adn,
        ado,
        adq,
        adr,
        ads,
        adt,
        adu,
        adw,
        adx,
        ady,
        adz,
        aea,
        aeb,
        aec,
        aed,
        aee,
        aek,
        ael,
        aem,
        aen,
        aeq,
        aer,
        aes,
        aeu,
        aew,
        aey,
        aez,
        afa,
        afb,
        afd,
        afe,
        afg,
        afh,
        afi,
        afk,
        afn,
        afo,
        afp,
        afr,
        afs,
        aft,
        afu,
        afz,
        aga,
        agb,
        agc,
        agd,
        age,
        agf,
        agg,
        agh,
        agi,
        agj,
        agk,
        agl,
        agm,
        agn,
        ago,
        agq,
        agr,
        ags,
        agt,
        agu,
        agv,
        agw,
        agx,
        agy,
        agz,
        aha,
        ahb,
        ahg,
        ahh,
        ahi,
        ahk,
        ahl,
        ahm,
        ahn,
        aho,
        ahp,
        ahr,
        ahs,
        aht,
        aia,
        aib,
        aic,
        aid,
        aie,
        aif,
        aig,
        aih,
        aii,
        aij,
        aik,
        ail,
        aim,
        ain,
        aio,
        aip,
        aiq,
        air,
        ait,
        aiw,
        aix,
        aiy,
        aja,
        ajg,
        aji,
        ajn,
        ajp,
        ajt,
        aju,
        ajw,
        ajz,
        aka,
        akb,
        akc,
        akd,
        ake,
        akf,
        akg,
        akh,
        aki,
        akj,
        akk,
        akl,
        akm,
        ako,
        akp,
        akq,
        akr,
        aks,
        akt,
        aku,
        akv,
        akw,
        akx,
        aky,
        akz,
        ala,
        alc,
        ald,
        ale,
        alf,
        alg,
        alh,
        ali,
        alj,
        alk,
        all,
        alm,
        aln,
        alo,
        alp,
        alq,
        alr,
        als,
        alt,
        alu,
        alv,
        alw,
        alx,
        aly,
        alz,
        ama,
        amb,
        amc,
        ame,
        amf,
        amg,
        amh,
        ami,
        amj,
        amk,
        aml,
        amm,
        amn,
        amo,
        amp,
        amq,
        amr,
        ams,
        amt,
        amu,
        amv,
        amw,
        amx,
        amy,
        amz,
        ana,
        anb,
        anc,
        and,
        ane,
        anf,
        ang,
        anh,
        ani,
        anj,
        ank,
        anl,
        anm,
        ann,
        ano,
        anp,
        anq,
        anr,
        ans,
        ant,
        anu,
        anv,
        anw,
        anx,
        any,
        anz,
        aoa,
        aob,
        aoc,
        aod,
        aoe,
        aof,
        aog,
        aoi,
        aoj,
        aok,
        aol,
        aom,
        aon,
        aor,
        aos,
        aot,
        aou,
        aox,
        aoz,
        apa,
        apb,
        apc,
        apd,
        ape,
        apf,
        apg,
        aph,
        api,
        apj,
        apk,
        apl,
        apm,
        apn,
        apo,
        app,
        apq,
        apr,
        aps,
        apt,
        apu,
        apv,
        apw,
        apx,
        apy,
        apz,
        aqa,
        aqc,
        aqd,
        aqg,
        aql,
        aqm,
        aqn,
        aqp,
        aqr,
        aqt,
        aqz,
        ara,
        arb,
        arc,
        ard,
        are,
        arg,
        arh,
        ari,
        arj,
        ark,
        arl,
        arn,
        aro,
        arp,
        arq,
        arr,
        ars,
        art,
        aru,
        arv,
        arw,
        arx,
        ary,
        arz,
        asa,
        asb,
        asc,
        ase,
        asf,
        asg,
        ash,
        asi,
        asj,
        ask,
        asl,
        asm,
        asn,
        aso,
        asp,
        asq,
        asr,
        ass,
        ast,
        asu,
        asv,
        asw,
        asx,
        asy,
        asz,
        ata,
        atb,
        atc,
        atd,
        ate,
        atg,
        ath,
        ati,
        atj,
        atk,
        atl,
        atm,
        atn,
        ato,
        atp,
        atq,
        atr,
        ats,
        att,
        atu,
        atv,
        atw,
        atx,
        aty,
        atz,
        aua,
        aub,
        auc,
        aud,
        auf,
        aug,
        auh,
        aui,
        auj,
        auk,
        aul,
        aum,
        aun,
        auo,
        aup,
        auq,
        aur,
        aus,
        aut,
        auu,
        auw,
        aux,
        auy,
        auz,
        ava,
        avb,
        avd,
        ave,
        avi,
        avk,
        avl,
        avm,
        avn,
        avo,
        avs,
        avt,
        avu,
        avv,
        awa,
        awb,
        awc,
        awd,
        awe,
        awg,
        awh,
        awi,
        awk,
        awm,
        awn,
        awo,
        awr,
        aws,
        awt,
        awu,
        awv,
        aww,
        awx,
        awy,
        axb,
        axe,
        axg,
        axk,
        axl,
        axm,
        axx,
        aya,
        ayb,
        ayc,
        ayd,
        aye,
        ayg,
        ayh,
        ayi

    }

    // Or: https://gist.github.com/JamieMason/3748498
    //  e.g. de-DE       German (Germany)
    //       de-CH German(Switzerland)

// Language is an ISO 639-1 language with code, name and native name.
//type Language struct {
//    Code       string
//    Name       string
//    NativeName string
//}

// Languages is a map of all ISO 639-1 languages using the two character lowercase language code as key.
//var Languages = map[string]Language{
//    "aa": {Code: "aa", Name: "Afar", NativeName: "Afaraf"},
//    "ab": {Code: "ab", Name: "Abkhaz", NativeName: "аҧсуа бызшәа"},
//    "ae": {Code: "ae", Name: "Avestan", NativeName: "avesta"},
//    "af": {Code: "af", Name: "Afrikaans", NativeName: "Afrikaans"},
//    "ak": {Code: "ak", Name: "Akan", NativeName: "Akan"},
//    "am": {Code: "am", Name: "Amharic", NativeName: "አማርኛ"},
//    "an": {Code: "an", Name: "Aragonese", NativeName: "aragonés"},
//    "ar": {Code: "ar", Name: "Arabic", NativeName: "اللغة العربية"},
//    "as": {Code: "as", Name: "Assamese", NativeName: "অসমীয়া"},
//    "av": {Code: "av", Name: "Avaric", NativeName: "авар мацӀ"},
//    "ay": {Code: "ay", Name: "Aymara", NativeName: "aymar aru"},
//    "az": {Code: "az", Name: "Azerbaijani", NativeName: "azərbaycan dili"},
//    "ba": {Code: "ba", Name: "Bashkir", NativeName: "башҡорт теле"},
//    "be": {Code: "be", Name: "Belarusian", NativeName: "беларуская мова"},
//    "bg": {Code: "bg", Name: "Bulgarian", NativeName: "български език"},
//    "bh": {Code: "bh", Name: "Bihari", NativeName: "भोजपुरी"},
//    "bi": {Code: "bi", Name: "Bislama", NativeName: "Bislama"},
//    "bm": {Code: "bm", Name: "Bambara", NativeName: "bamanankan"},
//    "bn": {Code: "bn", Name: "Bengali", NativeName: "বাংলা"},
//    "bo": {Code: "bo", Name: "Tibetan Standard", NativeName: "བོད་ཡིག"},
//    "br": {Code: "br", Name: "Breton", NativeName: "brezhoneg"},
//    "bs": {Code: "bs", Name: "Bosnian", NativeName: "bosanski jezik"},
//    "ca": {Code: "ca", Name: "Catalan", NativeName: "català"},
//    "ce": {Code: "ce", Name: "Chechen", NativeName: "нохчийн мотт"},
//    "ch": {Code: "ch", Name: "Chamorro", NativeName: "Chamoru"},
//    "co": {Code: "co", Name: "Corsican", NativeName: "corsu"},
//    "cr": {Code: "cr", Name: "Cree", NativeName: "ᓀᐦᐃᔭᐍᐏᐣ"},
//    "cs": {Code: "cs", Name: "Czech", NativeName: "čeština"},
//    "cu": {Code: "cu", Name: "Old Church Slavonic", NativeName: "ѩзыкъ словѣньскъ"},
//    "cv": {Code: "cv", Name: "Chuvash", NativeName: "чӑваш чӗлхи"},
//    "cy": {Code: "cy", Name: "Welsh", NativeName: "Cymraeg"},
//    "da": {Code: "da", Name: "Danish", NativeName: "dansk"},
//    "de": {Code: "de", Name: "German", NativeName: "Deutsch"},
//    "dv": {Code: "dv", Name: "Divehi", NativeName: "Dhivehi"},
//    "dz": {Code: "dz", Name: "Dzongkha", NativeName: "རྫོང་ཁ"},
//    "ee": {Code: "ee", Name: "Ewe", NativeName: "Eʋegbe"},
//    "el": {Code: "el", Name: "Greek", NativeName: "Ελληνικά"},
//    "en": {Code: "en", Name: "English", NativeName: "English"},
//    "eo": {Code: "eo", Name: "Esperanto", NativeName: "Esperanto"},
//    "es": {Code: "es", Name: "Spanish", NativeName: "Español"},
//    "et": {Code: "et", Name: "Estonian", NativeName: "eesti"},
//    "eu": {Code: "eu", Name: "Basque", NativeName: "euskara"},
//    "fa": {Code: "fa", Name: "Persian", NativeName: "فارسی"},
//    "ff": {Code: "ff", Name: "Fula", NativeName: "Fulfulde"},
//    "fi": {Code: "fi", Name: "Finnish", NativeName: "suomi"},
//    "fj": {Code: "fj", Name: "Fijian", NativeName: "Vakaviti"},
//    "fo": {Code: "fo", Name: "Faroese", NativeName: "føroyskt"},
//    "fr": {Code: "fr", Name: "French", NativeName: "Français"},
//    "fy": {Code: "fy", Name: "Western Frisian", NativeName: "Frysk"},
//    "ga": {Code: "ga", Name: "Irish", NativeName: "Gaeilge"},
//    "gd": {Code: "gd", Name: "Scottish Gaelic", NativeName: "Gàidhlig"},
//    "gl": {Code: "gl", Name: "Galician", NativeName: "galego"},
//    "gn": {Code: "gn", Name: "Guaraní", NativeName: "Avañeẽ"},
//    "gu": {Code: "gu", Name: "Gujarati", NativeName: "ગુજરાતી"},
//    "gv": {Code: "gv", Name: "Manx", NativeName: "Gaelg"},
//    "ha": {Code: "ha", Name: "Hausa", NativeName: "هَوُسَ"},
//    "he": {Code: "he", Name: "Hebrew", NativeName: "עברית"},
//    "hi": {Code: "hi", Name: "Hindi", NativeName: "हिन्दी"},
//    "ho": {Code: "ho", Name: "Hiri Motu", NativeName: "Hiri Motu"},
//    "hr": {Code: "hr", Name: "Croatian", NativeName: "hrvatski jezik"},
//    "ht": {Code: "ht", Name: "Haitian", NativeName: "Kreyòl ayisyen"},
//    "hu": {Code: "hu", Name: "Hungarian", NativeName: "magyar"},
//    "hy": {Code: "hy", Name: "Armenian", NativeName: "Հայերեն"},
//    "hz": {Code: "hz", Name: "Herero", NativeName: "Otjiherero"},
//    "ia": {Code: "ia", Name: "Interlingua", NativeName: "Interlingua"},
//    "id": {Code: "id", Name: "Indonesian", NativeName: "Indonesian"},
//    "ie": {Code: "ie", Name: "Interlingue", NativeName: "Interlingue"},
//    "ig": {Code: "ig", Name: "Igbo", NativeName: "Asụsụ Igbo"},
//    "ii": {Code: "ii", Name: "Nuosu", NativeName: "ꆈꌠ꒿ Nuosuhxop"},
//    "ik": {Code: "ik", Name: "Inupiaq", NativeName: "Iñupiaq"},
//    "io": {Code: "io", Name: "Ido", NativeName: "Ido"},
//    "is": {Code: "is", Name: "Icelandic", NativeName: "Íslenska"},
//    "it": {Code: "it", Name: "Italian", NativeName: "Italiano"},
//    "iu": {Code: "iu", Name: "Inuktitut", NativeName: "ᐃᓄᒃᑎᑐᑦ"},
//    "ja": {Code: "ja", Name: "Japanese", NativeName: "日本語"},
//    "jv": {Code: "jv", Name: "Javanese", NativeName: "basa Jawa"},
//    "ka": {Code: "ka", Name: "Georgian", NativeName: "ქართული"},
//    "kg": {Code: "kg", Name: "Kongo", NativeName: "Kikongo"},
//    "ki": {Code: "ki", Name: "Kikuyu", NativeName: "Gĩkũyũ"},
//    "kj": {Code: "kj", Name: "Kwanyama", NativeName: "Kuanyama"},
//    "kk": {Code: "kk", Name: "Kazakh", NativeName: "қазақ тілі"},
//    "kl": {Code: "kl", Name: "Kalaallisut", NativeName: "kalaallisut"},
//    "km": {Code: "km", Name: "Khmer", NativeName: "ខេមរភាសា"},
//    "kn": {Code: "kn", Name: "Kannada", NativeName: "ಕನ್ನಡ"},
//    "ko": {Code: "ko", Name: "Korean", NativeName: "한국어"},
//    "kr": {Code: "kr", Name: "Kanuri", NativeName: "Kanuri"},
//    "ks": {Code: "ks", Name: "Kashmiri", NativeName: "कश्मीरी"},
//    "ku": {Code: "ku", Name: "Kurdish", NativeName: "Kurdî"},
//    "kv": {Code: "kv", Name: "Komi", NativeName: "коми кыв"},
//    "kw": {Code: "kw", Name: "Cornish", NativeName: "Kernewek"},
//    "ky": {Code: "ky", Name: "Kyrgyz", NativeName: "Кыргызча"},
//    "la": {Code: "la", Name: "Latin", NativeName: "latine"},
//    "lb": {Code: "lb", Name: "Luxembourgish", NativeName: "Lëtzebuergesch"},
//    "lg": {Code: "lg", Name: "Ganda", NativeName: "Luganda"},
//    "li": {Code: "li", Name: "Limburgish", NativeName: "Limburgs"},
//    "ln": {Code: "ln", Name: "Lingala", NativeName: "Lingála"},
//    "lo": {Code: "lo", Name: "Lao", NativeName: "ພາສາ"},
//    "lt": {Code: "lt", Name: "Lithuanian", NativeName: "lietuvių kalba"},
//    "lu": {Code: "lu", Name: "Luba-Katanga", NativeName: "Tshiluba"},
//    "lv": {Code: "lv", Name: "Latvian", NativeName: "latviešu valoda"},
//    "mg": {Code: "mg", Name: "Malagasy", NativeName: "fiteny malagasy"},
//    "mh": {Code: "mh", Name: "Marshallese", NativeName: "Kajin M̧ajeļ"},
//    "mi": {Code: "mi", Name: "Māori", NativeName: "te reo Māori"},
//    "mk": {Code: "mk", Name: "Macedonian", NativeName: "македонски јазик"},
//    "ml": {Code: "ml", Name: "Malayalam", NativeName: "മലയാളം"},
//    "mn": {Code: "mn", Name: "Mongolian", NativeName: "Монгол хэл"},
//    "mr": {Code: "mr", Name: "Marathi", NativeName: "मराठी"},
//    "ms": {Code: "ms", Name: "Malay", NativeName: "هاس ملايو‎"},
//    "mt": {Code: "mt", Name: "Maltese", NativeName: "Malti"},
//    "my": {Code: "my", Name: "Burmese", NativeName: "ဗမာစာ"},
//    "na": {Code: "na", Name: "Nauru", NativeName: "Ekakairũ Naoero"},
//    "nb": {Code: "nb", Name: "Norwegian Bokmål", NativeName: "Norsk bokmål"},
//    "nd": {Code: "nd", Name: "Northern Ndebele", NativeName: "isiNdebele"},
//    "ne": {Code: "ne", Name: "Nepali", NativeName: "नेपाली"},
//    "ng": {Code: "ng", Name: "Ndonga", NativeName: "Owambo"},
//    "nl": {Code: "nl", Name: "Dutch", NativeName: "Nederlands"},
//    "nn": {Code: "nn", Name: "Norwegian Nynorsk", NativeName: "Norsk nynorsk"},
//    "no": {Code: "no", Name: "Norwegian", NativeName: "Norsk"},
//    "nr": {Code: "nr", Name: "Southern Ndebele", NativeName: "isiNdebele"},
//    "nv": {Code: "nv", Name: "Navajo", NativeName: "Diné bizaad"},
//    "ny": {Code: "ny", Name: "Chichewa", NativeName: "chiCheŵa"},
//    "oc": {Code: "oc", Name: "Occitan", NativeName: "occitan"},
//    "oj": {Code: "oj", Name: "Ojibwe", NativeName: "ᐊᓂᔑᓈᐯᒧᐎᓐ"},
//    "om": {Code: "om", Name: "Oromo", NativeName: "Afaan Oromoo"},
//    "or": {Code: "or", Name: "Oriya", NativeName: "ଓଡ଼ିଆ"},
//    "os": {Code: "os", Name: "Ossetian", NativeName: "ирон æвзаг"},
//    "pa": {Code: "pa", Name: "Panjabi", NativeName: "ਪੰਜਾਬੀ"},
//    "pi": {Code: "pi", Name: "Pāli", NativeName: "पाऴि"},
//    "pl": {Code: "pl", Name: "Polish", NativeName: "język polski"},
//    "ps": {Code: "ps", Name: "Pashto", NativeName: "پښتو"},
//    "pt": {Code: "pt", Name: "Portuguese", NativeName: "Português"},
//    "qu": {Code: "qu", Name: "Quechua", NativeName: "Runa Simi"},
//    "rm": {Code: "rm", Name: "Romansh", NativeName: "rumantsch grischun"},
//    "rn": {Code: "rn", Name: "Kirundi", NativeName: "Ikirundi"},
//    "ro": {Code: "ro", Name: "Romanian", NativeName: "Română"},
//    "ru": {Code: "ru", Name: "Russian", NativeName: "Русский"},
//    "rw": {Code: "rw", Name: "Kinyarwanda", NativeName: "Ikinyarwanda"},
//    "sa": {Code: "sa", Name: "Sanskrit", NativeName: "संस्कृतम्"},
//    "sc": {Code: "sc", Name: "Sardinian", NativeName: "sardu"},
//    "sd": {Code: "sd", Name: "Sindhi", NativeName: "सिन्धी"},
//    "se": {Code: "se", Name: "Northern Sami", NativeName: "Davvisámegiella"},
//    "sg": {Code: "sg", Name: "Sango", NativeName: "yângâ tî sängö"},
//    "si": {Code: "si", Name: "Sinhala", NativeName: "සිංහල"},
//    "sk": {Code: "sk", Name: "Slovak", NativeName: "slovenčina"},
//    "sl": {Code: "sl", Name: "Slovene", NativeName: "slovenski jezik"},
//    "sm": {Code: "sm", Name: "Samoan", NativeName: "gagana faa Samoa"},
//    "sn": {Code: "sn", Name: "Shona", NativeName: "chiShona"},
//    "so": {Code: "so", Name: "Somali", NativeName: "Soomaaliga"},
//    "sq": {Code: "sq", Name: "Albanian", NativeName: "Shqip"},
//    "sr": {Code: "sr", Name: "Serbian", NativeName: "српски језик"},
//    "ss": {Code: "ss", Name: "Swati", NativeName: "SiSwati"},
//    "st": {Code: "st", Name: "Southern Sotho", NativeName: "Sesotho"},
//    "su": {Code: "su", Name: "Sundanese", NativeName: "Basa Sunda"},
//    "sv": {Code: "sv", Name: "Swedish", NativeName: "svenska"},
//    "sw": {Code: "sw", Name: "Swahili", NativeName: "Kiswahili"},
//    "ta": {Code: "ta", Name: "Tamil", NativeName: "தமிழ்"},
//    "te": {Code: "te", Name: "Telugu", NativeName: "తెలుగు"},
//    "tg": {Code: "tg", Name: "Tajik", NativeName: "тоҷикӣ"},
//    "th": {Code: "th", Name: "Thai", NativeName: "ไทย"},
//    "ti": {Code: "ti", Name: "Tigrinya", NativeName: "ትግርኛ"},
//    "tk": {Code: "tk", Name: "Turkmen", NativeName: "Türkmen"},
//    "tl": {Code: "tl", Name: "Tagalog", NativeName: "Wikang Tagalog"},
//    "tn": {Code: "tn", Name: "Tswana", NativeName: "Setswana"},
//    "to": {Code: "to", Name: "Tonga", NativeName: "faka Tonga"},
//    "tr": {Code: "tr", Name: "Turkish", NativeName: "Türkçe"},
//    "ts": {Code: "ts", Name: "Tsonga", NativeName: "Xitsonga"},
//    "tt": {Code: "tt", Name: "Tatar", NativeName: "татар теле"},
//    "tw": {Code: "tw", Name: "Twi", NativeName: "Twi"},
//    "ty": {Code: "ty", Name: "Tahitian", NativeName: "Reo Tahiti"},
//    "ug": {Code: "ug", Name: "Uyghur", NativeName: "ئۇيغۇرچە‎"},
//    "uk": {Code: "uk", Name: "Ukrainian", NativeName: "Українська"},
//    "ur": {Code: "ur", Name: "Urdu", NativeName: "اردو"},
//    "uz": {Code: "uz", Name: "Uzbek", NativeName: "Ўзбек"},
//    "ve": {Code: "ve", Name: "Venda", NativeName: "Tshivenḓa"},
//    "vi": {Code: "vi", Name: "Vietnamese", NativeName: "Tiếng Việt"},
//    "vo": {Code: "vo", Name: "Volapük", NativeName: "Volapük"},
//    "wa": {Code: "wa", Name: "Walloon", NativeName: "walon"},
//    "wo": {Code: "wo", Name: "Wolof", NativeName: "Wollof"},
//    "xh": {Code: "xh", Name: "Xhosa", NativeName: "isiXhosa"},
//    "yi": {Code: "yi", Name: "Yiddish", NativeName: "ייִדיש"},
//    "yo": {Code: "yo", Name: "Yoruba", NativeName: "Yorùbá"},
//    "za": {Code: "za", Name: "Zhuang", NativeName: "Saɯ cueŋƅ"},
//    "zh": {Code: "zh", Name: "Chinese", NativeName: "中文"},
//    "zu": {Code: "zu", Name: "Zulu", NativeName: "isiZulu"},
//}


//CULTURE   SPEC.CULTURE  ENGLISH NAME
//--------------------------------------------------------------
//                        Invariant Language (Invariant Country)
//af          af-ZA       Afrikaans
//af-ZA       af-ZA       Afrikaans (South Africa)
//ar          ar-SA       Arabic
//ar-AE       ar-AE       Arabic (U.A.E.)
//ar-BH       ar-BH       Arabic (Bahrain)
//ar-DZ       ar-DZ       Arabic (Algeria)
//ar-EG       ar-EG       Arabic (Egypt)
//ar-IQ       ar-IQ       Arabic (Iraq)
//ar-JO       ar-JO       Arabic (Jordan)
//ar-KW       ar-KW       Arabic (Kuwait)
//ar-LB       ar-LB       Arabic (Lebanon)
//ar-LY       ar-LY       Arabic (Libya)
//ar-MA       ar-MA       Arabic (Morocco)
//ar-OM       ar-OM       Arabic (Oman)
//ar-QA       ar-QA       Arabic (Qatar)
//ar-SA       ar-SA       Arabic (Saudi Arabia)
//ar-SY       ar-SY       Arabic (Syria)
//ar-TN       ar-TN       Arabic (Tunisia)
//ar-YE       ar-YE       Arabic (Yemen)
//az          az-Latn-AZ  Azeri
//az-Cyrl-AZ  az-Cyrl-AZ  Azeri (Cyrillic, Azerbaijan)
//az-Latn-AZ  az-Latn-AZ  Azeri (Latin, Azerbaijan)
//be          be-BY       Belarusian
//be-BY       be-BY       Belarusian (Belarus)
//bg          bg-BG       Bulgarian
//bg-BG       bg-BG       Bulgarian (Bulgaria)
//bs-Latn-BA  bs-Latn-BA  Bosnian (Bosnia and Herzegovina)
//ca          ca-ES       Catalan
//ca-ES       ca-ES       Catalan (Catalan)
//cs          cs-CZ       Czech
//cs-CZ       cs-CZ       Czech (Czech Republic)
//cy-GB       cy-GB       Welsh (United Kingdom)
//da          da-DK       Danish
//da-DK       da-DK       Danish (Denmark)
//de          de-DE       German
//de-AT       de-AT       German (Austria)
//de-DE       de-DE       German (Germany)
//de-CH       de-CH       German (Switzerland)
//de-LI       de-LI       German (Liechtenstein)
//de-LU       de-LU       German (Luxembourg)
//dv          dv-MV       Divehi
//dv-MV       dv-MV       Divehi (Maldives)
//el          el-GR       Greek
//el-GR       el-GR       Greek (Greece)
//en          en-US       English
//en-029      en-029      English (Caribbean)
//en-AU       en-AU       English (Australia)
//en-BZ       en-BZ       English (Belize)
//en-CA       en-CA       English (Canada)
//en-GB       en-GB       English (United Kingdom)
//en-IE       en-IE       English (Ireland)
//en-JM       en-JM       English (Jamaica)
//en-NZ       en-NZ       English (New Zealand)
//en-PH       en-PH       English (Republic of the Philippines)
//en-TT       en-TT       English (Trinidad and Tobago)
//en-US       en-US       English (United States)
//en-ZA       en-ZA       English (South Africa)
//en-ZW       en-ZW       English (Zimbabwe)
//es          es-ES       Spanish
//es-AR       es-AR       Spanish (Argentina)
//es-BO       es-BO       Spanish (Bolivia)
//es-CL       es-CL       Spanish (Chile)
//es-CO       es-CO       Spanish (Colombia)
//es-CR       es-CR       Spanish (Costa Rica)
//es-DO       es-DO       Spanish (Dominican Republic)
//es-EC       es-EC       Spanish (Ecuador)
//es-ES       es-ES       Spanish (Spain)
//es-GT       es-GT       Spanish (Guatemala)
//es-HN       es-HN       Spanish (Honduras)
//es-MX       es-MX       Spanish (Mexico)
//es-NI       es-NI       Spanish (Nicaragua)
//es-PA       es-PA       Spanish (Panama)
//es-PE       es-PE       Spanish (Peru)
//es-PR       es-PR       Spanish (Puerto Rico)
//es-PY       es-PY       Spanish (Paraguay)
//es-SV       es-SV       Spanish (El Salvador)
//es-UY       es-UY       Spanish (Uruguay)
//es-VE       es-VE       Spanish (Venezuela)
//et          et-EE       Estonian
//et-EE       et-EE       Estonian (Estonia)
//eu          eu-ES       Basque
//eu-ES       eu-ES       Basque (Basque)
//fa          fa-IR       Persian
//fa-IR       fa-IR       Persian (Iran)
//fi          fi-FI       Finnish
//fi-FI       fi-FI       Finnish (Finland)
//fo          fo-FO       Faroese
//fo-FO       fo-FO       Faroese (Faroe Islands)
//fr          fr-FR       French
//fr-BE       fr-BE       French (Belgium)
//fr-CA       fr-CA       French (Canada)
//fr-FR       fr-FR       French (France)
//fr-CH       fr-CH       French (Switzerland)
//fr-LU       fr-LU       French (Luxembourg)
//fr-MC       fr-MC       French (Principality of Monaco)
//gl          gl-ES       Galician
//gl-ES       gl-ES       Galician (Galician)
//gu          gu-IN       Gujarati
//gu-IN       gu-IN       Gujarati (India)
//he          he-IL       Hebrew
//he-IL       he-IL       Hebrew (Israel)
//hi          hi-IN       Hindi
//hi-IN       hi-IN       Hindi (India)
//hr          hr-HR       Croatian
//hr-BA       hr-BA       Croatian (Bosnia and Herzegovina)
//hr-HR       hr-HR       Croatian (Croatia)
//hu          hu-HU       Hungarian
//hu-HU       hu-HU       Hungarian (Hungary)
//hy          hy-AM       Armenian
//hy-AM       hy-AM       Armenian (Armenia)
//id          id-ID       Indonesian
//id-ID       id-ID       Indonesian (Indonesia)
//is          is-IS       Icelandic
//is-IS       is-IS       Icelandic (Iceland)
//it          it-IT       Italian
//it-CH       it-CH       Italian (Switzerland)
//it-IT       it-IT       Italian (Italy)
//ja          ja-JP       Japanese
//ja-JP       ja-JP       Japanese (Japan)
//ka          ka-GE       Georgian
//ka-GE       ka-GE       Georgian (Georgia)
//kk          kk-KZ       Kazakh
//kk-KZ       kk-KZ       Kazakh (Kazakhstan)
//kn          kn-IN       Kannada
//kn-IN       kn-IN       Kannada (India)
//ko          ko-KR       Korean
//kok         kok-IN      Konkani
//kok-IN      kok-IN      Konkani (India)
//ko-KR       ko-KR       Korean (Korea)
//ky          ky-KG       Kyrgyz
//ky-KG       ky-KG       Kyrgyz (Kyrgyzstan)
//lt          lt-LT       Lithuanian
//lt-LT       lt-LT       Lithuanian (Lithuania)
//lv          lv-LV       Latvian
//lv-LV       lv-LV       Latvian (Latvia)
//mi-NZ       mi-NZ       Maori (New Zealand)
//mk          mk-MK       Macedonian
//mk-MK       mk-MK       Macedonian (Former Yugoslav Republic of Macedonia)
//mn          mn-MN       Mongolian
//mn-MN       mn-MN       Mongolian (Cyrillic, Mongolia)
//mr          mr-IN       Marathi
//mr-IN       mr-IN       Marathi (India)
//ms          ms-MY       Malay
//ms-BN       ms-BN       Malay (Brunei Darussalam)
//ms-MY       ms-MY       Malay (Malaysia)
//mt-MT       mt-MT       Maltese (Malta)
//nb-NO       nb-NO       Norwegian, Bokmal (Norway)
//nl          nl-NL       Dutch
//nl-BE       nl-BE       Dutch (Belgium)
//nl-NL       nl-NL       Dutch (Netherlands)
//nn-NO       nn-NO       Norwegian, Nynorsk (Norway)
//no          nb-NO       Norwegian
//ns-ZA       ns-ZA       Northern Sotho (South Africa)
//pa          pa-IN       Punjabi
//pa-IN       pa-IN       Punjabi (India)
//pl          pl-PL       Polish
//pl-PL       pl-PL       Polish (Poland)
//pt          pt-BR       Portuguese
//pt-BR       pt-BR       Portuguese (Brazil)
//pt-PT       pt-PT       Portuguese (Portugal)
//quz-BO      quz-BO      Quechua (Bolivia)
//quz-EC      quz-EC      Quechua (Ecuador)
//quz-PE      quz-PE      Quechua (Peru)
//ro          ro-RO       Romanian
//ro-RO       ro-RO       Romanian (Romania)
//ru          ru-RU       Russian
//ru-RU       ru-RU       Russian (Russia)
//sa          sa-IN       Sanskrit
//sa-IN       sa-IN       Sanskrit (India)
//se-FI       se-FI       Sami (Northern) (Finland)
//se-NO       se-NO       Sami (Northern) (Norway)
//se-SE       se-SE       Sami (Northern) (Sweden)
//sk          sk-SK       Slovak
//sk-SK       sk-SK       Slovak (Slovakia)
//sl          sl-SI       Slovenian
//sl-SI       sl-SI       Slovenian (Slovenia)
//sma-NO      sma-NO      Sami (Southern) (Norway)
//sma-SE      sma-SE      Sami (Southern) (Sweden)
//smj-NO      smj-NO      Sami (Lule) (Norway)
//smj-SE      smj-SE      Sami (Lule) (Sweden)
//smn-FI      smn-FI      Sami (Inari) (Finland)
//sms-FI      sms-FI      Sami (Skolt) (Finland)
//sq          sq-AL       Albanian
//sq-AL       sq-AL       Albanian (Albania)
//sr          sr-Latn-CS  Serbian
//sr-Cyrl-BA  sr-Cyrl-BA  Serbian (Cyrillic) (Bosnia and Herzegovina)
//sr-Cyrl-CS  sr-Cyrl-CS  Serbian (Cyrillic, Serbia)
//sr-Latn-BA  sr-Latn-BA  Serbian (Latin) (Bosnia and Herzegovina)
//sr-Latn-CS  sr-Latn-CS  Serbian (Latin, Serbia)
//sv          sv-SE       Swedish
//sv-FI       sv-FI       Swedish (Finland)
//sv-SE       sv-SE       Swedish (Sweden)
//sw          sw-KE       Kiswahili
//sw-KE       sw-KE       Kiswahili (Kenya)
//syr         syr-SY      Syriac
//syr-SY      syr-SY      Syriac (Syria)
//ta          ta-IN       Tamil
//ta-IN       ta-IN       Tamil (India)
//te          te-IN       Telugu
//te-IN       te-IN       Telugu (India)
//th          th-TH       Thai
//th-TH       th-TH       Thai (Thailand)
//tn-ZA       tn-ZA       Tswana (South Africa)
//tr          tr-TR       Turkish
//tr-TR       tr-TR       Turkish (Turkey)
//tt          tt-RU       Tatar
//tt-RU       tt-RU       Tatar (Russia)
//uk          uk-UA       Ukrainian
//uk-UA       uk-UA       Ukrainian (Ukraine)
//ur          ur-PK       Urdu
//ur-PK       ur-PK       Urdu (Islamic Republic of Pakistan)
//uz          uz-Latn-UZ  Uzbek
//uz-Cyrl-UZ  uz-Cyrl-UZ  Uzbek (Cyrillic, Uzbekistan)
//uz-Latn-UZ  uz-Latn-UZ  Uzbek (Latin, Uzbekistan)
//vi          vi-VN       Vietnamese
//vi-VN       vi-VN       Vietnamese (Vietnam)
//xh-ZA       xh-ZA       Xhosa (South Africa)
//zh-CN       zh-CN       Chinese (People's Republic of China)
//zh-HK       zh-HK       Chinese (Hong Kong S.A.R.)
//zh-CHS      (none)      Chinese (Simplified)
//zh-CHT      (none)      Chinese (Traditional)
//zh-MO       zh-MO       Chinese (Macao S.A.R.)
//zh-SG       zh-SG       Chinese (Singapore)
//zh-TW       zh-TW       Chinese (Taiwan)
//zu-ZA       zu-ZA       Zulu (South Africa)

}
