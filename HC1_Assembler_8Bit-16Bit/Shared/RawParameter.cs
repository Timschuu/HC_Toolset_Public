using System;

namespace HC1_Assembler_8Bit_16Bit.Shared
{
    public class RawParameter
    {
        public string Content { get; set; }
        public bool Resolvable { get; set; }
        public Type Type { get; set; }

        public RawParameter(string content, bool resolvable, Type type)
        {
            Content = content;
            Resolvable = resolvable;
            Type = type;
        }
    }
}