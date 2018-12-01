using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Shared;

namespace HC1_Assembler_8Bit_16Bit.Operations
{
    public abstract class OperationBase
    {
        public List<ParameterInfo> ParameterList { get; protected set; }
        public string Mnemonic { get; set; }
        private int _op;
        public byte Opcode
        {
            get => (byte)_op;

            set => _op = value;
        }
    }
}