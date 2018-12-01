using HC1_Assembler_8Bit_16Bit.Elf;
using HC1_Assembler_8Bit_16Bit.Interfaces;

namespace HC1_Assembler_8Bit_16Bit.Linker
{
    public class ElfSectionPair
    {
        public IElfSection Section { get; set; }
        public SectionHeader Header { get; set; }
    }
}