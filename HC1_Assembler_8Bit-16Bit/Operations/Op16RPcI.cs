using System;
using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Shared;

namespace HC1_Assembler_8Bit_16Bit.Operations
{
    public class Op16RpcI : Op16Ri, ILinkageInformationProvider
    {
        private const int Parametercount = 2;
        public Op16RpcI()
        {
            ParameterList = new List<ParameterInfo>(Parametercount)
            {
                new ParameterInfo(0, 31, 0x03E0, 5, null),
                new ParameterInfo(-16, 15, 0x001F, 0, null)
            };
        }

        public int AdjustSymbol(IOperation operation, int currentaddress, int symbolvalue)
        {
            return symbolvalue - currentaddress;
        }

        public override void Emulate(ref EmulatorContext emulatorContext, IEnumerable<int> args)
        {
            List<int> arguments = args.ToList();
            if (arguments.Count != Parametercount)
            {
                throw new InvalidOperationException();
            }

            switch (Opcode)
            {
                case 14://JLI
                    emulatorContext.SetRegisterValue(arguments[0], emulatorContext.ProgramCounter);
                    emulatorContext.ProgramCounter += arguments[1] - emulatorContext.Stepwidth;
                    break;
                case 16://BZ
                    if (emulatorContext.GetRegisterValue(arguments[0]) == 0)
                    {
                        emulatorContext.ProgramCounter += arguments[1] - emulatorContext.Stepwidth;
                    }
                    break;
                case 17://BPOS
                    if (emulatorContext.GetRegisterValue(arguments[0]) > 0)
                    {
                        emulatorContext.ProgramCounter += arguments[1] - emulatorContext.Stepwidth;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}