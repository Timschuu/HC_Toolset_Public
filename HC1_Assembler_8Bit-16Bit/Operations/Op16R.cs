using System;
using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Assembler;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Emulator.Interfaces;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Shared;

namespace HC1_Assembler_8Bit_16Bit.Operations
{
    public class Op16R : Operation16BitBase, IOperation, IEmulatable
    {
        private const int Parametercount = 1;
        public Op16R()
        {
            ParameterList = new List<ParameterInfo>(Parametercount)
            {
                new ParameterInfo(0, 0x03FF, 0x03FF, 0, null)
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
            int p0 = BitConverter.ToInt32(binaryParameters[0].Bytes.ToArray(), 0);
            return (uint)((Opcode << 10) | p0);
        }

        public List<byte> ToBinaryInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file)
        {
            uint instruction = ToAssembeldInstruction(symTab, parameters, currentline, file);
            return ToBinaryInstruction(instruction);
        }

        public void Emulate(ref EmulatorContext emulatorContext, int[] args)
        {
            if (args.Length != Parametercount)
            {
                throw new InvalidOperationException();
            }
            
            switch (Opcode)
            {
                case 13:
                    //NOT
                    emulatorContext.SetRegisterValue(args[0], ~emulatorContext.GetRegisterValue(args[0]));
                    break;
                default:
                    throw new NotImplementedException();  
            }
        }

        public int[] GetArgumentsFromInstruction(int instruction)
        {
            return new []
            {
                instruction & 0x3FF
            };
        }
    }
}