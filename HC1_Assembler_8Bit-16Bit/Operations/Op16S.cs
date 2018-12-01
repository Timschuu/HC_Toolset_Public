using System;
using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Assembler;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Emulator.Interfaces;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Shared;
using HC1_Assembler_8Bit_16Bit.SystemHalf;

namespace HC1_Assembler_8Bit_16Bit.Operations
{
    public class Op16S : Operation16BitBase, IOperation, ILinkageInformationProvider, IEmulatable
    {
        private const int Parametercount = 2;
        private readonly bool _loadUpper;

        public Op16S(bool loadUpper)
        {
            _loadUpper = loadUpper;
            ParameterList = new List<ParameterInfo>(Parametercount)
            {
                new ParameterInfo(0, 3, 0x0300, 8, null),
                new ParameterInfo(-32768, 32767, 0x00FF, 0, new HalfParameterInfo(Half.MinValue, Half.MaxValue))
            };
        }

        public uint ToAssembeldInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file)
        {
            var paras = parameters.ToList();
            if (paras.Count != Parametercount)
            {
                ExceptionHandler.ThrowParameterCountException(currentline, file, paras.Count, Parametercount);
            }

            if (!int.TryParse(paras[0].Content, out _))
            {
                ExceptionHandler.ThrowMnemonicException(paras[0].Content, currentline, file);
            }

            if (paras[0].Resolvable)
            {
                paras[0] = new RawParameter((int.Parse(paras[0].Content) - 22).ToString(), true, paras[0].Type);  //Adjust Register for Special Function Register
            }
            
            List<BinaryParameter> binaryParameters = AssemblerHelper.CheckParameters(paras, currentline, file, ParameterList);
            int p0 = BitConverter.ToInt32(binaryParameters[0].Bytes.ToArray(), 0);
            int p1 = BitConverter.ToInt32(binaryParameters[1].Bytes.ToArray(), 0);
            return (uint)((Opcode << 10) | p0 << 8 | AdjustSymbol(this, 0, p1));
        }

        public List<byte> ToBinaryInstruction(IEnumerable<Symbol> symTab, IEnumerable<RawParameter> parameters, int currentline, string file)
        {
            uint instruction = ToAssembeldInstruction(symTab, parameters, currentline, file);
            return ToBinaryInstruction(instruction);
        }

        public int AdjustSymbol(IOperation operation, int currentaddress, int symbolvalue)
        {
            if (_loadUpper)
            {
                return (symbolvalue & 0xFF00) >> 8;
            }

            return symbolvalue & 0x000000FF;
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
                //TODO: Does LUI clear the register?
                
                case 23:
                    //LUI
                    emulatorContext.SetRegisterValue(22 + arguments[0], arguments[1] << 8);
                    break;
                case 24:
                    //LLI
                    emulatorContext.SetRegisterValue(22 + arguments[0], emulatorContext.GetRegisterValue(22 + arguments[0]) | arguments[1]);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<int> GetArgumentsFromInstruction(int instruction)
        {
            return new List<int>
            {
                (instruction & 0x0300) >> 8,
                instruction & 0x00FF
            };
        }
    }
}