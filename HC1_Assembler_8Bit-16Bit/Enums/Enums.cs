using System;
// ReSharper disable UnusedMember.Global

namespace HC1_Assembler_8Bit_16Bit.Enums
{
    
    [Flags]
    public enum Bitwidth
    {
        Bit32 = 0x01,
        Bit64 = 0x02,
        Bit8 = 0x03, // Not an ELF standard, but needed for HC1
        Bit16 = 0x04
    }

    [Flags]
    public enum Endianness
    {
        Le = 0x01,
        Be = 0x02
    }

    public enum ElfHeaderIndices
    {
        Bitwidth = 4,
        Headersize = 52
    }

    public enum Sender
    {
        Assembler,
        Linker,
        Emulator
    }

    public enum Register
    {
        R0 = 0,
        R1 = 1,
        R2 = 2,
        R3 = 3,
        R4 = 4,
        R5 = 5,
        R6 = 6,
        R7 = 7,
        R8 = 8,
        R9 = 9,
        R10 = 10,
        R11 = 11,
        R12 = 12,
        R13 = 13,
        R14 = 14,
        R15 = 15,
        R16 = 16,
        R17 = 17,
        R18 = 18,
        R19 = 19,
        R20 = 20,
        R21 = 21,
        R22 = 22,
        R23 = 23,
        F0 = 24,
        F1 = 25,
        F2 = 26,
        F3 = 27,
        F4 = 28,
        F5 = 29,
        F6 = 30,
        F7 = 31
    }

    public enum SymbolType
    {
        Undefinded = 0,
        Object = 1,
        Func = 2,
        Section = 3
    }
}