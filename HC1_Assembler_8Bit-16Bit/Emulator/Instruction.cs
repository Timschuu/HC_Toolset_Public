using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Emulator.Interfaces;
using HC1_Assembler_8Bit_16Bit.Interfaces;

namespace HC1_Assembler_8Bit_16Bit.Emulator
{
    public class Instruction : IInstruction
    {
        public IOperation Operation { get; }
        public IList<int> Arguments { get; }
        public int Line { get; }
        public string Label { get; set; } = null;

        public Instruction(IOperation operation, IList<int> arguments, int line)
        {
            Operation = operation;
            Arguments = arguments;
            Line = line;
        }
    }
}