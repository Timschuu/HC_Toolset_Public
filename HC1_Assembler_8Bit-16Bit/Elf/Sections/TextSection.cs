using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Linker;
// ReSharper disable UnusedMember.Global

namespace HC1_Assembler_8Bit_16Bit.Elf.Sections
{
    [ElfSection(".text")]
    public class TextSection : IElfSection
    {
        private const int Type = 1;
        public const string Name = ".text";
        public List<byte> Bytes { get; set; }

        public TextSection(List<byte> assembledProgram)
        {
            Bytes = assembledProgram;
        }

        [ElfSectionConstructor]
        // ReSharper disable once UnusedParameter.Local
        public TextSection(ElfFileContainer elfFileContainer, List<byte> binaries, SectionHeader header)
        {
            Bytes = binaries.GetRange(0, header.Size);
            binaries.RemoveRange(0, header.Size);
        }
        
        public List<byte> Generate()
        {
            return Bytes;
        }

        public SectionHeader GenerateSectionHeader(SectionHeaderStringTable shsti, int adress, int offset)
        {
            return new SectionHeader(shsti.FindNameIndex(Name), Type, 0, offset, Bytes.Count);
        }
    }
}