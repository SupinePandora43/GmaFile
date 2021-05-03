using System;

namespace GmaFile
{
    public abstract class Gma
    {
        public abstract FileEntry[] Files { get; }

        public struct FileEntry
        {
            public string Name;
            public Func<byte[]> Content;
            public long Size;
            public uint CRC;
            public uint FileId;
            public long Offset;
        }
    }
}
