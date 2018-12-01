using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Shared;

namespace HC1_Assembler_8Bit_16Bit.Operations
{
    internal class OpIo : Operation8BitBase, IOperation
    {
        private const int Parametercount = 0;
        public int DataSubst { get; }

        public OpIo(int dataSubst) 
        {
            DataSubst = dataSubst;
            ParameterList = new List<ParameterInfo>(Parametercount);
        }
        
        public uint ToAssembeldInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file)
        {
            var paras = parameters.ToList();
            if (paras.Count != Parametercount)
            {
                ExceptionHandler.ThrowParameterCountException(currentline, file, paras.Count, Parametercount);
            }
            return (uint)((Opcode << 5) | DataSubst);
        }
        
        public List<byte> ToBinaryInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file)
        {
            uint instruction = ToAssembeldInstruction(symTab, parameters, currentline, file);
            return ToBinaryInstruction(instruction);
        }
    }
}
