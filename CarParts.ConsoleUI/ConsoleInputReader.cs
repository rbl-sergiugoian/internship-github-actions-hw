using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.ConsoleUI
{
    public static class ConsoleInputReader
    {
        public static string InputString(string message)
        {
            string input = string.Empty;

            while(string.IsNullOrEmpty(input))
            {
                Console.WriteLine(message);
                input = Console.ReadLine() ?? string.Empty;
            }

            return input;
        }
        public static int InputInt(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    return result;
                }
                Console.WriteLine("input must be an integer");
            }
        }

        public static DateOnly InputDate(string message)
        {
            string input;
            DateOnly resultDate;

            while(true)
            {
                Console.WriteLine(message);
                input = Console.ReadLine() ?? string.Empty;

                if (DateOnly.TryParse(input, out resultDate))
                {
                    return resultDate;
                }

                Console.WriteLine("format of the date input must respect pattern: yyyy-mm-dd");
            }
        }

        public static Guid InputGuid(string message)
        {
            string input;
            Guid resultGuid = Guid.Empty;

            while(true)
            {
                Console.WriteLine(message);
                input = Console.ReadLine() ?? string.Empty;
                if (Guid.TryParse(input, out resultGuid))
                {
                    return resultGuid;
                }

                Console.WriteLine("input string must respect format guid format");
                return Guid.Empty;
            }
        }
    }
}
