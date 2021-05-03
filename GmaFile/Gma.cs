using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GmaFile
{
    public class GmaFile : Gma
    {
        private Stream stream;

        private BinaryReader reader;

        private static bool IsEmptyString(BinaryReader reader)
        {
            if (reader.ReadChar() is (char)0) return true;

            while (reader.ReadChar() is not (char)0) ;

            return false;
        }

        private static string ReadString(BinaryReader reader)
        {
            string str = "";
            while (true)
            {
                char c = reader.ReadChar();
                if (c is (char)0) break;
                str += c;
            }
            return str;
        }

        private readonly long fileBeggining;

        public readonly FileEntry[] Entries;

        public GmaFile(Stream stream)
        {
            this.stream = stream;

            Stream safeStream = Stream.Synchronized(stream);

            safeStream.Position = 0;

            BinaryReader reader = new(safeStream);

            if (!(reader.ReadChar() is 'G' && reader.ReadChar() is 'M' && reader.ReadChar() is 'A' && reader.ReadChar() is 'D'))
            {
                throw new FormatException("not gma");
            }

            reader.ReadChar();

            reader.ReadUInt64();
            reader.ReadUInt64();

            // read \0
            {
                bool isempty = IsEmptyString(reader);
                while (!isempty) isempty = IsEmptyString(reader);
            }

            // TODO: name, desc, author
            string name = ReadString(reader);
            string descJson = ReadString(reader);
            string author = ReadString(reader);
            //IsEmptyString(safeStream);
            //IsEmptyString(safeStream);
            //IsEmptyString(safeStream);

            // ver
            reader.ReadInt32();

            long offset = 0;

            List<FileEntry> entries = new();

            while (true)
            {
                uint fileNumber = reader.ReadUInt32();

                if (fileNumber is 0) break;

                FileEntry fileEntry = new();
                fileEntry.Name = ReadString(reader);
                fileEntry.Size = reader.ReadInt64();
                fileEntry.CRC = reader.ReadUInt32();
                fileEntry.Offset = offset;
                fileEntry.FileId = fileNumber;

                fileEntry.Content = () =>
                {
                    byte[] bytes = new byte[fileEntry.Size];

                    safeStream.Position = fileBeggining;

                    if (fileEntry.Size < int.MaxValue)
                    {
                        safeStream.Read(bytes, 0, (int)fileEntry.Size);
                    }
                    else throw new NotImplementedException();

                    return bytes;
                };

                entries.Add(fileEntry);

                offset += fileEntry.Size;
            }

            fileBeggining = safeStream.Position;

            Entries = entries.ToArray();
        }

        public override FileEntry[] Files
        {
            get
            {
                return null;
            }
        }
    }
}
