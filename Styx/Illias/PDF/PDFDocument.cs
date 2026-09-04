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
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A PDF document, opened for one purpose: to find the files embedded in it.
    ///
    /// The objects of a PDF are normally located through its cross-reference
    /// table. This reader instead scans the file for object definitions directly.
    /// That is deliberate: it treats classic cross-reference tables,
    /// cross-reference streams, incrementally updated files and files with a
    /// damaged table all the same way, and a charge transparency record is far
    /// too important to lose to a cross-reference table that some invoice
    /// generator got wrong.
    /// </summary>
    public sealed partial class PDFDocument
    {

        #region Data

        private readonly Byte[]                                          data;
        private readonly Dictionary<Int32, Int32>                        objectOffsets      = [];
        private readonly Dictionary<Int32, PDFObject>                    parsedObjects      = [];
        private readonly Dictionary<Int32, (Byte[] Data, Int32 Offset)>  compressedObjects  = [];

        private          Boolean                                         objectStreamsExpanded;

        #endregion

        #region Properties

        /// <summary>
        /// Whether this document is encrypted.
        ///
        /// Chargy does not decrypt: an encrypted PDF simply yields no attachments,
        /// because guessing at a password is not this library's business.
        /// </summary>
        public Boolean IsEncrypted    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Open a PDF document.
        /// </summary>
        /// <param name="Data">The bytes of a PDF file.</param>
        private PDFDocument(Byte[] Data)
        {

            this.data         = Data;
            this.IsEncrypted  = EncryptedTrailerRegex().IsMatch(Encoding.Latin1.GetString(Data));

            BuildObjectTable();

        }

        #endregion


        #region (static) TryOpen(Data, out Document)

        /// <summary>
        /// Try to open a PDF document.
        /// </summary>
        /// <param name="Data">The bytes of a file.</param>
        /// <param name="Document">The opened document.</param>
        public static Boolean TryOpen(ReadOnlyMemory<Byte>                    Data,
                                      [NotNullWhen(true)] out PDFDocument?    Document)
        {

            Document = null;

            var span = Data.Span;

            if (span.Length < 8 || !span[..5].SequenceEqual("%PDF-"u8))
                return false;

            try
            {
                Document = new PDFDocument(Data.ToArray());
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion


        #region Resolve      (Object)

        /// <summary>
        /// Follow an indirect reference to the object it names.
        /// </summary>
        /// <param name="Object">An object, possibly a reference.</param>
        public PDFObject? Resolve(PDFObject? Object)
        {

            var seen = 0;

            while (Object is PDFReference reference)
            {

                // A reference cycle must end the walk, not the process.
                if (++seen > 32)
                    return null;

                Object = GetObject(reference.Number);

            }

            return Object;

        }

        #endregion

        #region GetObject    (Number)

        /// <summary>
        /// Read the object with the given number.
        /// </summary>
        /// <param name="Number">The number of an object.</param>
        public PDFObject? GetObject(Int32 Number)
        {

            if (parsedObjects.TryGetValue(Number, out var cached))
                return cached;

            #region An object at a known offset within the file

            if (objectOffsets.TryGetValue(Number, out var offset))
            {

                var parser  = new PDFParser(data, offset);

                // "<number> <generation> obj"
                parser.ReadToken();
                parser.ReadToken();

                if (parser.ReadToken() == "obj")
                {

                    var value = parser.ParseObject();

                    parsedObjects[Number] = value;

                    return value;

                }

            }

            #endregion

            #region ..., or an object inside an object stream

            EnsureObjectStreamsExpanded();

            if (compressedObjects.TryGetValue(Number, out var compressed))
            {

                var value = new PDFParser(compressed.Data, compressed.Offset).ParseObject();

                parsedObjects[Number] = value;

                return value;

            }

            #endregion

            return null;

        }

        #endregion

        #region GetDictionary(Object)

        /// <summary>
        /// Resolve an object and return it as a dictionary, taking the dictionary
        /// of a stream when the object is one.
        /// </summary>
        /// <param name="Object">An object, possibly a reference.</param>
        public PDFDictionary? GetDictionary(PDFObject? Object)

            => Resolve(Object) switch {
                   PDFDictionary dictionary  => dictionary,
                   PDFStream     stream      => stream.Dictionary,
                   _                         => null
               };

        #endregion


        #region DecodeStream (Stream)

        /// <summary>
        /// Decode the bytes of a stream, applying every filter its dictionary names.
        /// </summary>
        /// <param name="Stream">A stream.</param>
        public Byte[]? DecodeStream(PDFStream Stream)
        {

            var current  = Stream.RawData;
            var filters  = FiltersOf(Stream.Dictionary);
            var parms    = DecodeParmsOf(Stream.Dictionary);

            for (var i = 0; i < filters.Count; i++)
            {

                var decoded = filters[i] switch {

                                  "FlateDecode"     or "Fl"  => Inflate       (current),
                                  "ASCIIHexDecode"  or "AHx" => ASCIIHexDecode(current),
                                  "ASCII85Decode"   or "A85" => ASCII85Decode (current),

                                  // A stream Chargy cannot decode is not an error
                                  // worth failing a verification over: it is simply
                                  // not an attachment it can read.
                                  _                          => null

                              };

                if (decoded is null)
                    return null;

                current = ApplyPredictor(
                              decoded,
                              i < parms.Count ? parms[i] : null
                          );

            }

            return current;

        }

        #endregion

        #region EmbeddedFiles()

        /// <summary>
        /// Every file embedded in this document, in the order the document names them.
        ///
        /// PDF/A-3 is what makes an electronic invoice able to carry its own
        /// machine-readable data — and what lets a charge point operator hand an
        /// EV driver a single PDF that is both a readable receipt and a verifiable
        /// charge transparency record.
        /// </summary>
        public IReadOnlyList<PDFEmbeddedFile> EmbeddedFiles()
        {

            var embeddedFiles = new List<PDFEmbeddedFile>();

            if (IsEncrypted)
                return embeddedFiles;

            var seenFileSpecifications = new HashSet<Int32>();

            #region The "/Names /EmbeddedFiles" name tree of the document catalogue

            var names = GetDictionary(Catalogue()?["Names"]);

            if (names is not null)
                CollectFromNameTree(
                    GetDictionary(names["EmbeddedFiles"]),
                    embeddedFiles,
                    seenFileSpecifications,
                    Depth: 0
                );

            #endregion

            #region ..., plus any file specification the name tree does not reach

            // PDF/A-3 also associates files through "/AF" entries on the catalogue,
            // a page or even a single graphics object. Rather than walk all of
            // those, every remaining file specification in the document is taken:
            // an attachment that is present but unreferenced is still an attachment.
            if (embeddedFiles.Count == 0)
                foreach (var number in AllObjectNumbers())
                {

                    if (seenFileSpecifications.Contains(number))
                        continue;

                    if (GetObject(number) is PDFDictionary candidate &&
                        candidate["Type"] is PDFName { Value: "Filespec" })
                    {
                        seenFileSpecifications.Add(number);
                        AddFileSpecification(candidate, embeddedFiles);
                    }

                }

            #endregion

            return embeddedFiles;

        }

        #endregion


        #region (private) BuildObjectTable()

        /// <summary>
        /// Find every "&lt;number&gt; &lt;generation&gt; obj" definition in the file.
        ///
        /// A later definition wins, because a PDF that has been updated
        /// incrementally appends the new version of an object to the end of the
        /// file and leaves the old one in place.
        /// </summary>
        private void BuildObjectTable()
        {

            var span   = data.AsSpan();
            var offset = 0;

            while (offset < span.Length)
            {

                var found = span[offset..].IndexOf("obj"u8);

                if (found < 0)
                    break;

                var keywordStart = offset + found;
                offset           = keywordStart + 3;

                // "obj" has to be a token of its own, not the tail of "endobj".
                if (keywordStart + 3 < span.Length &&
                   !PDFParser.IsWhitespace(span[keywordStart + 3]) &&
                   !PDFParser.IsDelimiter (span[keywordStart + 3]))
                    continue;

                if (TryReadObjectHeader(keywordStart, out var number, out var headerStart))
                    objectOffsets[number] = headerStart;

            }

        }

        #endregion

        #region (private) TryReadObjectHeader(KeywordStart, out Number, out HeaderStart)

        /// <summary>
        /// Read the two integers in front of an "obj" keyword.
        /// </summary>
        /// <param name="KeywordStart">Where the "obj" keyword begins.</param>
        /// <param name="Number">The number of the object.</param>
        /// <param name="HeaderStart">Where the object definition begins.</param>
        private Boolean TryReadObjectHeader(Int32      KeywordStart,
                                            out Int32  Number,
                                            out Int32  HeaderStart)
        {

            Number       = 0;
            HeaderStart  = 0;

            var index = KeywordStart - 1;

            // The generation number.
            while (index >= 0 && PDFParser.IsWhitespace(data[index]))
                index--;

            var generationEnd = index;

            while (index >= 0 && data[index] is >= 0x30 and <= 0x39)
                index--;

            if (index == generationEnd)
                return false;

            // The object number.
            while (index >= 0 && PDFParser.IsWhitespace(data[index]))
                index--;

            var numberEnd = index;

            while (index >= 0 && data[index] is >= 0x30 and <= 0x39)
                index--;

            if (index == numberEnd)
                return false;

            var numberStart = index + 1;
            var digits      = Encoding.ASCII.GetString(data, numberStart, numberEnd - numberStart + 1);

            if (!Int32.TryParse(digits, out Number))
                return false;

            HeaderStart = numberStart;

            return true;

        }

        #endregion

        #region (private) EnsureObjectStreamsExpanded()

        /// <summary>
        /// Unpack every object stream, so that the objects inside them can be found.
        ///
        /// Since PDF 1.5 most objects of a modern document live inside compressed
        /// object streams rather than in the file directly, so a reader that
        /// ignored them would find almost nothing in a recent invoice.
        /// </summary>
        private void EnsureObjectStreamsExpanded()
        {

            if (objectStreamsExpanded)
                return;

            objectStreamsExpanded = true;

            foreach (var number in AllObjectNumbers())
            {

                if (GetObject(number) is not PDFStream stream ||
                    stream.Dictionary["Type"] is not PDFName { Value: "ObjStm" })
                    continue;

                ExpandObjectStream(stream);

            }

        }

        #endregion

        #region (private) ExpandObjectStream(Stream)

        /// <summary>
        /// Register the objects held by a single object stream.
        /// </summary>
        /// <param name="Stream">An object stream.</param>
        private void ExpandObjectStream(PDFStream Stream)
        {

            if (DecodeStream(Stream)                                is not Byte[]    decoded  ||
                Resolve(Stream.Dictionary["N"])                     is not PDFNumber count    ||
                Resolve(Stream.Dictionary["First"])                 is not PDFNumber first)
                return;

            var header = new PDFParser(decoded);

            for (var i = 0; i < count.AsInt32; i++)
            {

                if (!Int32.TryParse(header.ReadToken(), out var objectNumber) ||
                    !Int32.TryParse(header.ReadToken(), out var objectOffset))
                    return;

                var absoluteOffset = first.AsInt32 + objectOffset;

                if (absoluteOffset < 0 || absoluteOffset >= decoded.Length)
                    continue;

                // An object defined in the file directly wins over one in an
                // object stream, because only the former can be an incremental update.
                if (!objectOffsets.ContainsKey(objectNumber))
                    compressedObjects[objectNumber] = (decoded, absoluteOffset);

            }

        }

        #endregion

        #region (private) Catalogue()

        /// <summary>
        /// The document catalogue, the root of everything a PDF contains.
        /// </summary>
        private PDFDictionary? Catalogue()
        {

            #region The "/Root" of a trailer

            foreach (Match match in TrailerRootRegex().Matches(Encoding.Latin1.GetString(data)).Reverse())
            {

                if (Int32.TryParse(match.Groups["number"].Value, out var number) &&
                    GetObject(number) is PDFDictionary candidate &&
                    candidate.Contains("Pages"))
                {
                    return candidate;
                }

            }

            #endregion

            #region ..., or whichever object says it is the catalogue

            foreach (var number in AllObjectNumbers())
                if (GetObject(number) is PDFDictionary candidate &&
                    candidate["Type"] is PDFName { Value: "Catalog" })
                {
                    return candidate;
                }

            #endregion

            return null;

        }

        #endregion

        #region (private) AllObjectNumbers()

        /// <summary>
        /// The numbers of every object known to this document.
        /// </summary>
        private IReadOnlyList<Int32> AllObjectNumbers()
        {

            var numbers = new List<Int32>(objectOffsets.Keys);

            numbers.AddRange(compressedObjects.Keys);
            numbers.Sort();

            return numbers;

        }

        #endregion

        #region (private) CollectFromNameTree(Node, EmbeddedFiles, Seen, Depth)

        /// <summary>
        /// Walk a name tree, collecting the file specifications it names.
        /// </summary>
        /// <param name="Node">A name tree node.</param>
        /// <param name="EmbeddedFiles">The embedded files collected so far.</param>
        /// <param name="Seen">The file specifications already collected.</param>
        /// <param name="Depth">The current nesting depth.</param>
        private void CollectFromNameTree(PDFDictionary?         Node,
                                         List<PDFEmbeddedFile>  EmbeddedFiles,
                                         HashSet<Int32>         Seen,
                                         Int32                  Depth)
        {

            if (Node is null || Depth > 32)
                return;

            #region A leaf: pairs of a name and a file specification

            if (Resolve(Node["Names"]) is PDFArray names)
                for (var i = 1; i < names.Count; i += 2)
                {

                    if (names[i] is PDFReference reference)
                        Seen.Add(reference.Number);

                    if (GetDictionary(names[i]) is PDFDictionary fileSpecification)
                        AddFileSpecification(fileSpecification, EmbeddedFiles);

                }

            #endregion

            #region ..., or a branch

            if (Resolve(Node["Kids"]) is PDFArray kids)
                foreach (var kid in kids.Items)
                    CollectFromNameTree(
                        GetDictionary(kid),
                        EmbeddedFiles,
                        Seen,
                        Depth + 1
                    );

            #endregion

        }

        #endregion

        #region (private) AddFileSpecification(FileSpecification, EmbeddedFiles)

        /// <summary>
        /// Read the file a file specification points at.
        /// </summary>
        /// <param name="FileSpecification">A file specification.</param>
        /// <param name="EmbeddedFiles">The embedded files collected so far.</param>
        private void AddFileSpecification(PDFDictionary          FileSpecification,
                                          List<PDFEmbeddedFile>  EmbeddedFiles)
        {

            if (GetDictionary(FileSpecification["EF"]) is not PDFDictionary embeddedFileDictionary)
                return;

            // "/UF" holds the Unicode name and is preferred since PDF 1.7;
            // "/F" is the older, system dependent one.
            var fileName = (Resolve(FileSpecification["UF"]) as PDFString)?.AsText() ??
                           (Resolve(FileSpecification["F"])  as PDFString)?.AsText();

            if (fileName is null || fileName.Length == 0)
                return;

            var fileStream = Resolve(embeddedFileDictionary["UF"]) as PDFStream ??
                             Resolve(embeddedFileDictionary["F"])  as PDFStream;

            if (fileStream is null)
                return;

            if (DecodeStream(fileStream) is not Byte[] content)
                return;

            EmbeddedFiles.Add(
                new PDFEmbeddedFile(
                    // A path separator would let an attachment name escape a
                    // directory once a consumer writes it out.
                    fileName[(fileName.LastIndexOfAny(['/', '\\']) + 1)..],
                    content,
                    (Resolve(GetDictionary(fileStream.Dictionary["Params"])?["Subtype"]) as PDFName)?.Value
                        ?? (Resolve(fileStream.Dictionary["Subtype"]) as PDFName)?.Value
                )
            );

        }

        #endregion


        #region (private, static) FiltersOf     (Dictionary)

        /// <summary>
        /// The filters a stream dictionary names, in the order they were applied.
        /// </summary>
        /// <param name="Dictionary">A stream dictionary.</param>
        private static IReadOnlyList<String> FiltersOf(PDFDictionary Dictionary)

            => (Dictionary["Filter"] ?? Dictionary["F"]) switch {

                   PDFName  name   => [ name.Value ],

                   PDFArray array  => [.. array.Items.OfType<PDFName>().Select(name => name.Value) ],

                   _               => []

               };

        #endregion

        #region (private, static) DecodeParmsOf (Dictionary)

        /// <summary>
        /// The decoding parameters belonging to each filter.
        /// </summary>
        /// <param name="Dictionary">A stream dictionary.</param>
        private static IReadOnlyList<PDFDictionary?> DecodeParmsOf(PDFDictionary Dictionary)

            => (Dictionary["DecodeParms"] ?? Dictionary["DP"]) switch {

                   PDFDictionary parameters  => [ parameters ],

                   PDFArray      array       => [.. array.Items.Select(item => item as PDFDictionary) ],

                   _                         => []

               };

        #endregion

        #region (private, static) Inflate       (Data)

        /// <summary>
        /// Undo a "/FlateDecode".
        /// </summary>
        /// <param name="Data">The compressed data.</param>
        private static Byte[]? Inflate(Byte[] Data)
        {

            if (Data.Length == 0)
                return [];

            // Almost every PDF wraps its deflate data in a zlib container, but
            // writers that omit the two byte header do exist.
            foreach (var raw in new[] { false, true })
            {

                try
                {

                    using var input   = new MemoryStream(Data, writable: false);
                    using var output  = new MemoryStream();

                    Stream decompressor = raw
                                              ? new DeflateStream(input, CompressionMode.Decompress)
                                              : new ZLibStream   (input, CompressionMode.Decompress);

                    using (decompressor)
                        decompressor.CopyTo(output);

                    if (output.Length > 0)
                        return output.ToArray();

                }
                catch (Exception)
                {
                    // Try the other framing.
                }

            }

            return null;

        }

        #endregion

        #region (private, static) ASCIIHexDecode(Data)

        /// <summary>
        /// Undo an "/ASCIIHexDecode".
        /// </summary>
        /// <param name="Data">The encoded data.</param>
        private static Byte[] ASCIIHexDecode(Byte[] Data)
        {

            var bytes     = new List<Byte>(Data.Length / 2);
            var current   = 0;
            var haveHigh  = false;

            foreach (var character in Data)
            {

                if (character == (Byte) '>')
                    break;

                var digit = character switch {
                                >= 0x30 and <= 0x39  => character - 0x30,
                                >= 0x41 and <= 0x46  => character - 0x41 + 10,
                                >= 0x61 and <= 0x66  => character - 0x61 + 10,
                                _                    => -1
                            };

                if (digit < 0)
                    continue;

                if (haveHigh)
                {
                    bytes.Add((Byte) ((current << 4) | digit));
                    haveHigh = false;
                }
                else
                {
                    current  = digit;
                    haveHigh = true;
                }

            }

            if (haveHigh)
                bytes.Add((Byte) (current << 4));

            return [.. bytes];

        }

        #endregion

        #region (private, static) ASCII85Decode (Data)

        /// <summary>
        /// Undo an "/ASCII85Decode".
        /// </summary>
        /// <param name="Data">The encoded data.</param>
        private static Byte[]? ASCII85Decode(Byte[] Data)
        {

            var bytes  = new List<Byte>(Data.Length * 4 / 5);
            var group  = new Int32[5];
            var count  = 0;
            var index  = 0;

            // An optional "<~" introduces the data.
            if (Data.Length >= 2 && Data[0] == (Byte) '<' && Data[1] == (Byte) '~')
                index = 2;

            for (; index < Data.Length; index++)
            {

                var character = Data[index];

                if (PDFParser.IsWhitespace(character))
                    continue;

                if (character == (Byte) '~')
                    break;

                // "z" stands for four zero bytes.
                if (character == (Byte) 'z' && count == 0)
                {
                    bytes.AddRange([0, 0, 0, 0]);
                    continue;
                }

                if (character is < 0x21 or > 0x75)
                    return null;

                group[count++] = character - 0x21;

                if (count == 5)
                {

                    var value = 0U;

                    for (var i = 0; i < 5; i++)
                        value = value * 85 + (UInt32) group[i];

                    bytes.Add((Byte) (value >> 24));
                    bytes.Add((Byte) (value >> 16));
                    bytes.Add((Byte) (value >>  8));
                    bytes.Add((Byte)  value);

                    count = 0;

                }

            }

            #region A partial final group

            if (count > 0)
            {

                if (count == 1)
                    return null;

                for (var i = count; i < 5; i++)
                    group[i] = 84;

                var value = 0U;

                for (var i = 0; i < 5; i++)
                    value = value * 85 + (UInt32) group[i];

                for (var i = 0; i < count - 1; i++)
                    bytes.Add((Byte) (value >> (24 - 8 * i)));

            }

            #endregion

            return [.. bytes];

        }

        #endregion

        #region (private, static) ApplyPredictor(Data, Parameters)

        /// <summary>
        /// Undo the PNG or TIFF predictor a stream was filtered through.
        ///
        /// Cross-reference and object streams are usually predictor encoded,
        /// because their rows of near-identical numbers compress far better that way.
        /// </summary>
        /// <param name="Data">The decompressed data.</param>
        /// <param name="Parameters">The decoding parameters of the filter.</param>
        private static Byte[] ApplyPredictor(Byte[]          Data,
                                             PDFDictionary?  Parameters)
        {

            if (Parameters?["Predictor"] is not PDFNumber predictor ||
                predictor.AsInt32 <= 1)
                return Data;

            var colors            = (Parameters["Colors"]           as PDFNumber)?.AsInt32 ?? 1;
            var bitsPerComponent  = (Parameters["BitsPerComponent"] as PDFNumber)?.AsInt32 ?? 8;
            var columns           = (Parameters["Columns"]          as PDFNumber)?.AsInt32 ?? 1;

            var bytesPerPixel     = Math.Max(1, colors * bitsPerComponent / 8);
            var bytesPerRow       = (columns * colors * bitsPerComponent + 7) / 8;

            if (bytesPerRow <= 0)
                return Data;

            // The TIFF predictor keeps no per-row tag byte and is rare enough in
            // PDFs that the data is better left alone than mangled.
            if (predictor.AsInt32 < 10)
                return Data;

            var rows       = Data.Length / (bytesPerRow + 1);
            var output     = new Byte[rows * bytesPerRow];
            var previous   = new Byte[bytesPerRow];

            for (var row = 0; row < rows; row++)
            {

                var tag      = Data[row * (bytesPerRow + 1)];
                var source   = Data.AsSpan(row * (bytesPerRow + 1) + 1, bytesPerRow);
                var current  = output.AsSpan(row * bytesPerRow,         bytesPerRow);

                source.CopyTo(current);

                for (var i = 0; i < bytesPerRow; i++)
                {

                    var left      = i >= bytesPerPixel ? current [i - bytesPerPixel] : (Byte) 0;
                    var up        = previous[i];
                    var upperLeft = i >= bytesPerPixel ? previous[i - bytesPerPixel] : (Byte) 0;

                    current[i] = tag switch {
                                     1  => (Byte) (current[i] + left),
                                     2  => (Byte) (current[i] + up),
                                     3  => (Byte) (current[i] + (left + up) / 2),
                                     4  => (Byte) (current[i] + PaethPredictor(left, up, upperLeft)),
                                     _  => current[i]
                                 };

                }

                current.CopyTo(previous);

            }

            return output;

        }

        #endregion

        #region (private, static) PaethPredictor(Left, Up, UpperLeft)

        /// <summary>
        /// The Paeth predictor of the PNG specification.
        /// </summary>
        /// <param name="Left">The byte to the left.</param>
        /// <param name="Up">The byte above.</param>
        /// <param name="UpperLeft">The byte above and to the left.</param>
        private static Byte PaethPredictor(Byte Left, Byte Up, Byte UpperLeft)
        {

            var estimate       = Left + Up - UpperLeft;
            var distanceLeft   = Math.Abs(estimate - Left);
            var distanceUp     = Math.Abs(estimate - Up);
            var distanceUpper  = Math.Abs(estimate - UpperLeft);

            return distanceLeft  <= distanceUp &&
                   distanceLeft  <= distanceUpper
                       ? Left
                       : distanceUp <= distanceUpper
                             ? Up
                             : UpperLeft;

        }

        #endregion


        #region (private) Regular expressions

        [GeneratedRegex(@"/Encrypt\s+\d+\s+\d+\s+R")]
        private static partial Regex EncryptedTrailerRegex();

        [GeneratedRegex(@"/Root\s+(?<number>\d+)\s+\d+\s+R")]
        private static partial Regex TrailerRootRegex();

        #endregion


    }

}
