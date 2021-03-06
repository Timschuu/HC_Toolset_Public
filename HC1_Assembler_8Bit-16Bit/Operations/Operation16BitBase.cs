using System.Collections.Generic;

namespace HC1_Assembler_8Bit_16Bit.Operations
{
    public abstract class Operation16BitBase : OperationBase
    {
        protected static List<byte> ToBinaryInstruction(uint instruction)
        {
            List<byte> bytelist = new List<byte>
            {
                (byte) ((instruction & 0xFF00) >> 8),
                (byte) (instruction & 0x00FF)
            };
            return bytelist;
        }
    }
}