using Fasciculus.Eve.IO;
using System;

namespace Fasciculus.Eve;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(EveDirectories.Downloads);

        SdeExtractor.Run();
    }
}
