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
    internal class Op16Rr : Operation16BitBase, IOperation, IEmulatable
    {
        private const int Parametercount = 2;
        public Op16Rr()
        {
            ParameterList = new List<ParameterInfo>(Parametercount)
            {
                new ParameterInfo(0, 31, 0x03E0, 5, null),
                new ParameterInfo(0, 31, 0x001F, 0, null)
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

        public void Emulate(ref EmulatorContext emulatorContext, int[] args)
        {
            if (args.Length != Parametercount)
            {
                throw new InvalidOperationException();
            }

            switch (Opcode)
            {
                case 0://LOAD
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetMemoryValue(emulatorContext.GetRegisterValue(args[1])));
                    break;
                case 1://STORE
                    emulatorContext.SetMemoryValue(emulatorContext.GetRegisterValue(args[1]), emulatorContext.GetRegisterValue(args[0]));
                    break;
                case 2://ADD
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) + emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 3://SUB
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) - emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 4://MUL
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) * emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 5://DIV
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) / emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 6://MOD
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) % emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 7://AND
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) & emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 8://OR
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) | emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 15://JL
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.ProgramCounter);
                    emulatorContext.ProgramCounter = emulatorContext.GetRegisterValue(args[1]) - emulatorContext.Stepwidth;
                    break;
                case 20://SLO
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) << emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 21://SAR
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[0]) >> emulatorContext.GetRegisterValue(args[1]));
                    break;
                case 22://MOV
                    emulatorContext.SetRegisterValue(args[0], emulatorContext.GetRegisterValue(args[1]));
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
                instruction & 0x001F
            };
        }
    }
}
