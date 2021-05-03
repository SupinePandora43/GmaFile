using System;
using System.IO;
using System.Text;

namespace GmaFile.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var fs = File.OpenRead("tr.gma");
            /*var result = GmaFile.Extract(fs);

            foreach(var key in result.Keys)
            {
                Console.WriteLine(key);

                if (key.EndsWith(".lua")) Console.WriteLine(Encoding.UTF8.GetString(result[key]));
            }
            */

            var gma = new GmaFile(fs);

            foreach(var f in gma.Entries)
            {
                Console.WriteLine(f.Name);
            }
        }
    }
}
