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
    public class Op16Ri : Operation16BitBase, IOperation, IEmulatable
    {
        private const int Parametercount = 2;
        public Op16Ri()
        {
            ParameterList = new List<ParameterInfo>(Parametercount)
            {
                new ParameterInfo(0, 31, 0x03E0, 5, null),
                new ParameterInfo(-16, 15, 0x001F, 0, null)
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
            int p1 = BitConverter.ToInt32(binaryParameters[1].Bytes.ToArray(), 0);
            return (uint)((Opcode << 10) | p0 << 5 | p1 & ParameterList[1].Bitmask);
        }

        public List<byte> ToBinaryInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file)
        {
            uint instruction = ToAssembeldInstruction(symTab, parameters, currentline, file);
            return ToBinaryInstruction(instruction);
        }


        public virtual void Emulate(ref EmulatorContext emulatorContext, int[] args)
        {
            if (args.Length != Parametercount)
            {
                throw new InvalidOperationException();
            }
            
            switch (Opcode)
            {
                case 9:
                    //ADDI
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) + args[1]);
                    break;
                case 10:
                    //SUBI
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) - args[1]);
                    break;
                case 11:
                    //MULI
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) * args[1]);
                    break;
                case 12:
                    //DIVI
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) / args[1]);
                    break;
                case 18:
                    //SLOI
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) << args[1]);
                    break;
                case 19:
                    //SARI
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) >> args[1]);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public int[] GetArgumentsFromInstruction(int instruction)
        {
            return new []
            {
                (instruction & 0x03E0) >> 5,
                (instruction & 0x001F) > 15 ? -((~instruction & 0x001F) + 1): instruction & 0x001F
            };
        }
    }
}