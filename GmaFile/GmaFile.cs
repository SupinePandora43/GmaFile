using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GmaFile
{
    public class GmaFile
    {
        public const char Version = '\u0003';
        public const uint AppID = 4000;
        public const uint Signature = 0xBEEFCACE;

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
        public static Dictionary<string, byte[]> Extract(Stream gma)
        {
            Dictionary<string, byte[]> files = new();

            gma.Position = 0;

            BinaryReader reader = new(gma);

            if (!(reader.ReadChar() is 'G' && reader.ReadChar() is 'M' && reader.ReadChar() is 'A' && reader.ReadChar() is 'D'))
            {
                throw new FormatException("not gma");
            }

            {
                char format_version = reader.ReadChar();
                if (format_version > Version)
                {
                    Console.Error.WriteLine("too new");
                }
            }

            ulong appid = reader.ReadUInt64(); // appid
            ulong timestamp = reader.ReadUInt64(); // timestamp

            string stContent = ReadString(reader);
            while (!string.IsNullOrEmpty(stContent))
            {
                stContent = ReadString(reader);
            }

            string name = ReadString(reader);
            string descJson = ReadString(reader);
            string author = ReadString(reader);

            int version = reader.ReadInt32();

            int iFileNumber = 1;
            long iOffset = 0;

            List<FileEntry> entries = new();

            while(reader.ReadUInt32() is not 0)
            {
                FileEntry entry = new()
                {
                    strName = ReadString(reader),
                    iSize = reader.ReadInt64(),
                    iCRC = reader.ReadUInt32(),
                    iOffset = iOffset,
                    iFileNumber = (uint)iFileNumber,
                };
                entries.Add(entry);

                iOffset += entry.iSize;
                iFileNumber++;
            }

            long m_fileblock = gma.Position;

            // parse desc

            foreach(FileEntry entry in entries)
            {
                gma.Position = m_fileblock + entry.iOffset;
                files.Add(entry.strName, reader.ReadBytes((int)entry.iSize));
            }

            return files;
        }

    }
    public struct FileEntry
    {
        public string strName;
        public long iSize;
        public uint iCRC;
        public uint iFileNumber;
        public long iOffset;
    }
}
