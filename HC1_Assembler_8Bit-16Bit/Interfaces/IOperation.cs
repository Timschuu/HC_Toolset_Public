using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Shared;

namespace HC1_Assembler_8Bit_16Bit.Interfaces 
{
    public interface IOperation 
    {
        string Mnemonic { get; set; }
        // ReSharper disable once UnusedMemberInSuper.Global
        byte Opcode { get; set; }
        
        List<ParameterInfo> ParameterList { get; }

        uint ToAssembeldInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file);

        List<byte> ToBinaryInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file);
    }
}
