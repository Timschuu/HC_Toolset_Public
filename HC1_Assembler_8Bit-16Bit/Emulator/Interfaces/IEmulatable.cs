namespace HC1_Assembler_8Bit_16Bit.Emulator.Interfaces
{
    public interface IEmulatable
    {
        void Emulate(ref EmulatorContext emulatorContext, int[] args);
        int[] GetArgumentsFromInstruction(int instruction);
    }
}