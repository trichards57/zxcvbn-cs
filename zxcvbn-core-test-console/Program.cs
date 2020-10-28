using System;

namespace Zxcvbn.TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var result = Zxcvbn.Core.EvaluatePassword("Applesoranges!");

            Console.WriteLine(result.Score);
        }
    }
}
