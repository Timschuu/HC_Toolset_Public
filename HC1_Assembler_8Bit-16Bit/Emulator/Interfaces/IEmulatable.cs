using System.Collections.Generic;

namespace HC1_Assembler_8Bit_16Bit.Emulator.Interfaces
{
    public interface IEmulatable
    {
        void Emulate(ref EmulatorContext emulatorContext, IEnumerable<int> args);
        IEnumerable<int> GetArgumentsFromInstruction(int instruction);
    }
}