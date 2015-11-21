using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using ToolBox;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var primes = Sequence.Primes().Take(99999999);
            var aux = new SortedSet<BigInteger>(primes);
            var fileStream = new GZipStream(File.Open("primes.txt", FileMode.Create), CompressionMode.Compress);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, aux);
            fileStream.Flush();
            fileStream.Close();
            fileStream.Dispose();
        }
    }
}
