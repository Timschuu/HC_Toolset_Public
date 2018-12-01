using System.Collections.Generic;
using System.Linq;

namespace HC1_Assembler_8Bit_16Bit.Elf
{
    public static class ElfHelper
    {
        /// <summary>
        /// Generates a zero terminated list of ASCII representations of strings.
        /// </summary>
        /// <param name="stringlist"> A list of strings. </param>
        /// <returns> A zero terminated list of ASCII representations. </returns>
        public static List<byte> StringlistToAsciilist(IEnumerable<string> stringlist)
        {
            List<byte> bytelist = new List<byte>();

            foreach (string s in stringlist)
            {
                foreach (char c in s)
                {
                    bytelist.Add((byte)c);
                }
                bytelist.Add(0x00);
            }

            return bytelist;
        }
        /// <summary>
        /// Converts a list of zero terminated ASCII chars to a list of strings.
        /// </summary>
        /// <param name="bytelist"> A zero terminated list of ASCII representations. </param>
        /// <param name="length"> The relevant length of the ASCII list. </param>
        /// <returns> A list of strings. </returns>
        public static List<string> AsciilistToStringlist(IEnumerable<byte> bytelist, int length)
        {
            List<byte> binaries = bytelist.ToList();
            int i = 0;
            string buffer = "";
            List<string> output = new List<string>();
            while (i < length)
            {
                while (binaries[i] != 0)
                {
                    buffer += (char) binaries[i];
                    i++;
                }
                output.Add(buffer);
                buffer = "";
                i++;
            }

            return output;
        }
        
    }
}