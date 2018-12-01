using System;
using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Linker;

namespace HC1_Assembler_8Bit_16Bit.Elf.Sections
{
    [ElfSection(".shstrtab")]
    public class SectionHeaderStringTable : IElfSection
    {
        private const string Name = ".shstrtab";
        private const int Type = 3;

        private readonly List<string> _sectionStrings = new List<string>
        {
            ".shstrtab",
            ".strtab",
            ".text",
            ".data",
            ".symtab",
            ".rela.text"
        };

        public List<byte> Bytes { get; set; }

        public SectionHeaderStringTable()
        {
            Bytes = Generate();
        }

        [ElfSectionConstructor]
        // ReSharper disable once UnusedParameter.Local
        public SectionHeaderStringTable(ElfFileContainer elfFileContainer, List<byte> binaries, SectionHeader header)
        {
            _sectionStrings = ElfHelper.AsciilistToStringlist(binaries, header.Size);
            binaries.RemoveRange(0, header.Size);
        }

        public List<byte> Generate()
        {
            return ElfHelper.StringlistToAsciilist(_sectionStrings);
        }

        public SectionHeader GenerateSectionHeader(SectionHeaderStringTable shsti, int adress, int offset)
        {
            return new SectionHeader(FindNameIndex(Name), Type, 0, offset, Bytes.Count);
        }

        public string GetNameFromIndex(int index)
        {
            int indexfinder = 0;
            if (index == 0)
            {
                return _sectionStrings[0];
            }
            for (int i = 0; i < _sectionStrings.Count - 1; i++)
            {
                indexfinder += _sectionStrings[i].Length + 1;
                if (indexfinder == index)
                {
                    return _sectionStrings[i + 1];
                }
            }
            throw new InvalidOperationException();
        }

        public int FindNameIndex(string name)
        {
            int listindex = _sectionStrings.IndexOf(name);
            if (listindex == -1)
            {
                return -1;
            }

            if (listindex == 0)
            {
                return 0;
            }

            int index = 0;

            for (int i = 0; i < listindex; i++)
            {
                index += _sectionStrings[i].Length + 1;
            }

            return index;

        }
    }
}