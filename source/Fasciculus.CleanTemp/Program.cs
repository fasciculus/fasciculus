using System;
using System.Diagnostics;
using System.IO;

namespace Fasciculus.CleanTemp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string tempPath = Environment.ExpandEnvironmentVariables("%TEMP%");
            DirectoryInfo temp = new(tempPath);
            DateTime now = DateTime.UtcNow;
            TimeSpan maxAge = TimeSpan.FromDays(2);

            int deletedCount = 0;
            long deletedLength = 0;

            int failedCount = 0;
            long failedLength = 0;

            foreach (FileInfo file in temp.GetFiles("*", SearchOption.AllDirectories))
            {
                if (file.Exists)
                {
                    TimeSpan age = now - file.LastAccessTimeUtc;

                    if (age > maxAge)
                    {
                        long length = file.Length;

                        try
                        {
                            file.Delete();

                            ++deletedCount;
                            deletedLength += length;
                        }
                        catch
                        {
                            ++failedCount;
                            failedLength += length;
                        }
                    }
                }
            }

            Log($"deleted {deletedCount} files, {deletedLength >> 20} MB");
            Log($"failed {failedCount} files, {failedLength >> 20} MB");
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
