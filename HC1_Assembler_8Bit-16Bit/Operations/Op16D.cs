using System;
using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Assembler;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Shared;
using HC1_Assembler_8Bit_16Bit.SystemHalf;
using ParameterInfo = HC1_Assembler_8Bit_16Bit.Shared.ParameterInfo;

namespace HC1_Assembler_8Bit_16Bit.Operations
{
    public class Op16D : Operation16BitBase, IOperation
    {
        private const int Parametercount = 1;
        public Op16D()
        {
            ParameterList = new List<ParameterInfo>(Parametercount)
            {
                new ParameterInfo(-32768, 32767, 0xFFFF, 0, new HalfParameterInfo(Half.MinValue, Half.MaxValue))
            };
        }
        public uint ToAssembeldInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file)
        {
            var paras = parameters.ToList();
            if (paras.Count != Parametercount)
            {
                ExceptionHandler.ThrowParameterCountException(currentline, file, paras.Count, Parametercount);
            }

            List<BinaryParameter> binaryParameters = AssemblerHelper.CheckParameters(paras, currentline, file, ParameterList);
            return (uint) BitConverter.ToInt32(binaryParameters[0].Bytes.ToArray(), 0);
        }

        public List<byte> ToBinaryInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file)
        {
            uint instruction = ToAssembeldInstruction(symTab, parameters, currentline, file);
            return ToBinaryInstruction(instruction);
        }
    }
}