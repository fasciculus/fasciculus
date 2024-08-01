using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
using System;

namespace Fasciculus.Eve;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            ReadResources.Read();

            Console.WriteLine(SolarSystems.Get("Jita")?.Name);

            Adjacencies.Create(SolarSystems.Safe);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
