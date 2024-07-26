using System;
using System.Reflection;

namespace Fasciculus.Eve;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                string[] names = assembly.GetManifestResourceNames();

                Console.WriteLine($"{assembly}");

                foreach (string name in names)
                {
                    Console.WriteLine($"- {name}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
