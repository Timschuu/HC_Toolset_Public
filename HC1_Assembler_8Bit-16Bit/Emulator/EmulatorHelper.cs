using System.Collections.Generic;
using System.Linq;

namespace HC1_Assembler_8Bit_16Bit.Emulator
{
    public static class EmulatorHelper
    {
        public static string GetArgumentString(IEnumerable<int> arguments)
        {
            string buf = arguments.Aggregate("", (current, arg) => current + (arg + " "));
            return buf.Trim();
        }
    }
}