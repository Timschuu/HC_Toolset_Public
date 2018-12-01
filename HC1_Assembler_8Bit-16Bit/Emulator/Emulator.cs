using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Elf;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Operations;

namespace HC1_Assembler_8Bit_16Bit.Emulator
{
    public static class Emulator
    {
        private static EmulatorContext _emulatorContext;

        public static void NewEmulatorContext(string programpath, int instructionsize)
        {
            List<byte> program = ElfLoader.LoadBinaryFile(programpath);
            List<IOperation> operations = OperationRegister.GetOperations(instructionsize);
            if (operations == null)
            {
                ExceptionHandler.ThrowInstructionSizeException(Sender.Emulator, instructionsize);
            }
            _emulatorContext = new EmulatorContext(instructionsize, program);
        }

        public static ref EmulatorContext GetEmulatorContext()
        {
            return ref _emulatorContext;
        } 
    }
}