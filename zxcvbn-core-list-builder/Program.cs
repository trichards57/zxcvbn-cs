using System;
using System.IO;
using System.Reflection;

namespace Zxcvbn.ListBuilder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage(Assembly.GetExecutingAssembly().GetName().Name);
                return;
            }

            var dataDir = args[0];
            var outputDir = args[1];

            var unfilteredLists = ListBuilder.ParseFrequencyLists(dataDir);
            var filteredLists = ListBuilder.FilterFrequencyLists(unfilteredLists);

            foreach (var kvp in filteredLists)
            {
                var name = kvp.Key;
                var list = kvp.Value;

                File.WriteAllLines(Path.Combine(outputDir, $"{name}.lst"), list);
            }
        }

        private static void PrintUsage(string name)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine($"    {name} dataDirectory outputDirectory");
            Console.WriteLine();
            Console.WriteLine("Converts the raw data used by zxcvbn's build_frequency_lists scripts into input files for embedding into the assembly.");
        }
    }
}
