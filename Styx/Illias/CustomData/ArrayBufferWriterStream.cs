/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Illias <https://www.github.com/Vanaheimr/Illias>
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

using System.Buffers;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    internal sealed class ArrayBufferWriterStream : Stream
    {

        private readonly ArrayBufferWriter<Byte> buffer;

        public ArrayBufferWriterStream()
        {
            buffer = new ArrayBufferWriter<Byte>();
        }

        public override Boolean CanRead
            => false;

        public override Boolean CanSeek
            => false;

        public override Boolean CanWrite
            => true;

        public override Int64 Length
            => buffer.WrittenCount;

        public override Int64 Position
        {
            get => buffer.WrittenCount;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        { }

        public override Int32 Read(Byte[] Buffer,
                                   Int32  Offset,
                                   Int32  Count)
            => throw new NotSupportedException();

        public override Int64 Seek(Int64      Offset,
                                   SeekOrigin Origin)
            => throw new NotSupportedException();

        public override void SetLength(Int64 Value)
            => throw new NotSupportedException();

        public override void Write(Byte[] Buffer,
                                   Int32  Offset,
                                   Int32  Count)
            => Write(Buffer.AsSpan(Offset, Count));

        public override void Write(ReadOnlySpan<Byte> Buffer)
        {
            var span = buffer.GetSpan(Buffer.Length);
            Buffer.CopyTo(span);
            buffer.Advance(Buffer.Length);
        }

        public Byte[] ToArray()
            => buffer.WrittenSpan.ToArray();

    }

}
