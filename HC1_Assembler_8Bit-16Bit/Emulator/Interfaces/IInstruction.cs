using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Interfaces;

namespace HC1_Assembler_8Bit_16Bit.Emulator.Interfaces
{
    public interface IInstruction
    {
        IOperation Operation { get; }
        IList<int> Arguments { get; }
        int Line { get; }
        string Label { get; set; }
    }
}