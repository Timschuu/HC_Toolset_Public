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

        public void Emulate(ref EmulatorContext emulatorContext, IEnumerable<int> args)
        {
            List<int> arguments = args.ToList();
            if (arguments.Count != Parametercount)
            {
                throw new InvalidOperationException();
            }

            switch (Opcode)
            {
                case 0://LOAD
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetMemoryValue(emulatorContext.GetRegisterValue(arguments[1])));
                    break;
                case 1://STORE
                    emulatorContext.SetMemoryValue(emulatorContext.GetRegisterValue(arguments[1]), emulatorContext.GetRegisterValue(arguments[0]));
                    break;
                case 2://ADD
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) + emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 3://SUB
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) - emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 4://MUL
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) * emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 5://DIV
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) / emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 6://MOD
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) % emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 7://AND
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) & emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 8://OR
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) | emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 15://JL
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.ProgramCounter);
                    emulatorContext.ProgramCounter = emulatorContext.GetRegisterValue(arguments[1]) - emulatorContext.Stepwidth;
                    break;
                case 20://SLO
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) << emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 21://SAR
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[0]) >> emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                case 22://MOV
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.GetRegisterValue(arguments[1]));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<int> GetArgumentsFromInstruction(int instruction)
        {
            return new List<int>
            {
                (instruction & 0x03E0) >> 5,
                instruction & 0x001F
            };
        }
    }
}
