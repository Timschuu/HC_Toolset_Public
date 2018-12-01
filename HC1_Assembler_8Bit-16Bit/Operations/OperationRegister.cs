using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Interfaces;

namespace HC1_Assembler_8Bit_16Bit.Operations
{
    public static class OperationRegister
    {
        /// <summary>
        /// Returns the set of instructions depending on the given instructionsize.
        /// </summary>
        /// <param name="instructionsize"> Either 8 or 16. </param>
        /// <returns> A list of instructions. Null if instructionsize is not 8 or 16. </returns>
        public static List<IOperation> GetOperations(int instructionsize)
        {
            switch (instructionsize)
            {
                case 8:
                    return new List<IOperation>
                    {
                        new OpR          {Mnemonic = "LOAD",  Opcode = 0},
                        new OpR          {Mnemonic = "STORE", Opcode = 1},
                        new OpR          {Mnemonic = "ADD",   Opcode = 2},
                        new OpR          {Mnemonic = "SUB",   Opcode = 3},
                        new OpRr         {Mnemonic = "NAND",  Opcode = 4},
                        new OpIo(0)      {Mnemonic = "IN",    Opcode = 4},
                        new OpIo(1)      {Mnemonic = "OUT",   Opcode = 4},
                        new OpJ          {Mnemonic = "JZ",    Opcode = 5},
                        new OpJ          {Mnemonic = "JPOS",  Opcode = 6},
                        new OpJ          {Mnemonic = "J",     Opcode = 7}
                    };
                case 16:
                    return new List<IOperation>
                    {
                        new Op16Rr       {Mnemonic = "LOAD",  Opcode = 0},
                        new Op16Rr       {Mnemonic = "STORE", Opcode = 1},
                        new Op16Rr       {Mnemonic = "ADD",   Opcode = 2},
                        new Op16Rr       {Mnemonic = "SUB",   Opcode = 3},
                        new Op16Rr       {Mnemonic = "MUL",   Opcode = 4},
                        new Op16Rr       {Mnemonic = "DIV",   Opcode = 5},
                        new Op16Rr       {Mnemonic = "MOD",   Opcode = 6},
                        new Op16Rr       {Mnemonic = "AND",   Opcode = 7},
                        new Op16Rr       {Mnemonic = "OR",    Opcode = 8},
                        new Op16Ri       {Mnemonic = "ADDI",  Opcode = 9},
                        new Op16Ri       {Mnemonic = "SUBI",  Opcode = 10},
                        new Op16Ri       {Mnemonic = "MULI",  Opcode = 11},
                        new Op16Ri       {Mnemonic = "DIVI",  Opcode = 12},
                        new Op16R        {Mnemonic = "NOT",   Opcode = 13},
                        new Op16RpcI     {Mnemonic = "JLI",   Opcode = 14},
                        new Op16Rr       {Mnemonic = "JL",    Opcode = 15},
                        new Op16RpcI     {Mnemonic = "BZ",    Opcode = 16},
                        new Op16RpcI     {Mnemonic = "BPOS",  Opcode = 17},
                        new Op16Ri       {Mnemonic = "SLOI",  Opcode = 18},
                        new Op16Ri       {Mnemonic = "SARI",  Opcode = 19},
                        new Op16Rr       {Mnemonic = "SLO",   Opcode = 20},
                        new Op16Rr       {Mnemonic = "SAR",   Opcode = 21},
                        new Op16Rr       {Mnemonic = "MOV",   Opcode = 22},
                        new Op16S(true)  {Mnemonic = "LUI",   Opcode = 23},
                        new Op16S(false) {Mnemonic = "LLI",   Opcode = 24},
                        new Op16D        {Mnemonic = "DATA",  Opcode = byte.MaxValue}
                    };
                default:
                    return null;
            }
        }
    }
}