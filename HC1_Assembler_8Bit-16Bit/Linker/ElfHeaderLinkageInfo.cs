using HC1_Assembler_8Bit_16Bit.Enums;

namespace HC1_Assembler_8Bit_16Bit.Linker
{
    public class ElfHeaderLinkageInfo
    {
        public int InstructionSize { get; }
        public int HeaderSize { get; }
        public Endianness Endianness { get; }
        public int SectionHeaderOffset { get; }
        public int SectionHeaderCount { get; }

        public ElfHeaderLinkageInfo(int instructionSize, int headerSize, Endianness endianness, int sectionHeaderOffset, int sectionHeaderCount)
        {
            InstructionSize = instructionSize;
            HeaderSize = headerSize;
            Endianness = endianness;
            SectionHeaderOffset = sectionHeaderOffset;
            SectionHeaderCount = sectionHeaderCount;
        }
    }
}