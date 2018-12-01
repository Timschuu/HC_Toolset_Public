using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Elf;
using HC1_Assembler_8Bit_16Bit.Elf.Sections;

namespace HC1_Assembler_8Bit_16Bit.Interfaces
{
    public interface IElfSection
    {
        List<byte> Bytes { get; set; }
        List<byte> Generate();
        SectionHeader GenerateSectionHeader(SectionHeaderStringTable shsti, int adress, int offset);
    }
}