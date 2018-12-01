using System;

namespace HC1_Assembler_8Bit_16Bit.Elf
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ElfSectionAttribute : Attribute
    {
        public string Name { get; }

        public ElfSectionAttribute(string name)
        {
            Name = name;
        }
    }
}