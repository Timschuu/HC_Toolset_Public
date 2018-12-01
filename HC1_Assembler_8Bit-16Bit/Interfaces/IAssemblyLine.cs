using System.Collections.Generic;

namespace HC1_Assembler_8Bit_16Bit.Interfaces
{
    public interface IAssemblyLine
    {
        List<string> Content { get; set; }
        int Line { get; }
    }
}