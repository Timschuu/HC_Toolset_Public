using System;
using System.Collections.Generic;

namespace HC1_Assembler_8Bit_16Bit.Shared
{
    public class BinaryParameter
    {
        public Type Type { get; set; }
        public List<byte> Bytes { get; set; }

        public BinaryParameter(Type type, List<byte> bytes)
        {
            Type = type;
            Bytes = bytes;
        }
    }
}