using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Interfaces;

namespace HC1_Assembler_8Bit_16Bit.Emulator.Interfaces
{
    public interface IAssemblyBuilder
    {
        IEnumerable<IInstruction> Build(int[] program, int instructionsize, IEnumerable<IOperation> operations);
    }
}