using System;
using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Emulator.Interfaces;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Linker;
using HC1_Assembler_8Bit_16Bit.Operations;

namespace HC1_Assembler_8Bit_16Bit.Emulator
{
    public class AssemblyBuilder : IAssemblyBuilder
    {
        private List<IInstruction> _instructions;
        private int _instructionsize;
        
        public IEnumerable<IInstruction> Build(int[] program, int instructionsize, IEnumerable<IOperation> operations)
        {
            _instructions = new List<IInstruction>();
            _instructionsize = instructionsize;
            int linecount = 0;
            foreach (int i in program)
            {
                
                IOperation operation = LinkerHelper.GetOperationFromInstruction(i, _instructionsize);
                if (!(operation is IEmulatable emulatable))
                {
                    throw new NotImplementedException();
                }

                IInstruction instruction = new Instruction(operation, emulatable.GetArgumentsFromInstruction(i).ToList(), linecount++);
                _instructions.Add(instruction);
            }

            return _instructions;
        }

        private bool DoesInstructionJump(IOperation operation)
        {
            switch (_instructionsize)
            {
                case 8:
                    return operation is OpJ;
                case 16:
                    return operation is Op16RpcI || operation.Opcode == 15;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}