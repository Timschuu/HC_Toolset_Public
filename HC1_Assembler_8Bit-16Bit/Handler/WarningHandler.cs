using System;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;

namespace HC1_Assembler_8Bit_16Bit.Handler
{
    public static class WarningHandler
    {
        private static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[Warning] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }
        
        public static void NoInstructionsizeDirective(Sender sender, string file)
        {
            WriteWarning($"[{sender}] No instructionsize directive found, using 16 bit for file \"{file.PathToFileName()}\".");
        }
    }
}