using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Interfaces;

namespace HC1_Assembler_8Bit_16Bit.Assembler
{
    public class AssemblyLine : IAssemblyLine
    {
        public List<string> Content { get; set; }
        public int Line { get; set; }

        public AssemblyLine(List<string> content, int line)
        {
            Content = content;
            Line = line;
        }
    }
}