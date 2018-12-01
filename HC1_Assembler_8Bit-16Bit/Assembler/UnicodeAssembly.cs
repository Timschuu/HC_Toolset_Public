using System.Collections.Generic;

namespace HC1_Assembler_8Bit_16Bit.Assembler
{
    public class UnicodeAssembly
    {
        public IEnumerable<uint> Assembly { get; set; }
        public int InstructionSize { get; set; }

        public UnicodeAssembly(IEnumerable<uint> assem, int instructionSize)
        {
            Assembly = assem;
            InstructionSize = instructionSize;
        }
    }
}