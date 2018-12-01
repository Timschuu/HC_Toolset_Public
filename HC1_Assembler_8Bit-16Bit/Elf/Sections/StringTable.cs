using System;
using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Linker;
using HC1_Assembler_8Bit_16Bit.Shared;
// ReSharper disable UnusedMember.Global

namespace HC1_Assembler_8Bit_16Bit.Elf.Sections
{
    [ElfSection(".strtab")]
    public class StringTable : IElfSection
    {
        public const string Name = ".strtab";
        private const int Type = 3;
        public List<byte> Bytes { get; set; }
        private readonly IEnumerable<Symbol> _symtab;
        private readonly List<string> _strings;
        
        public StringTable(IEnumerable<Symbol> symboltab)
        {
            _symtab = symboltab;
            Bytes = Generate();
        }

        [ElfSectionConstructor]
        // ReSharper disable once UnusedParameter.Local
        public StringTable(ElfFileContainer elfFileContainer, List<byte> binaries, SectionHeader header)
        {
            _strings = ElfHelper.AsciilistToStringlist(binaries, header.Size);
            binaries.RemoveRange(0, header.Size);
        }
        
        public List<byte> Generate()
        {
            List<string> symbolnames = _symtab.Select(symbol => symbol.Label).ToList();
            return ElfHelper.StringlistToAsciilist(symbolnames);
        }

        public SectionHeader GenerateSectionHeader(SectionHeaderStringTable shsti, int adress, int offset)
        {
            return new SectionHeader(shsti.FindNameIndex(Name), Type, 0, offset, Bytes.Count);
        }
        
        public string GetNameFromIndex(int index)
        {
            int indexfinder = 0;
            if (index == 0)
            {
                return _strings[0];
            }
            for (int i = 0; i < _strings.Count - 1; i++)
            {
                indexfinder += _strings[i].Length + 1;
                if (indexfinder == index)
                {
                    return _strings[i + 1];
                }
            }
            throw new InvalidOperationException();
        }
        
        public int FindNameIndex(string name)
        {
            List<string> symbolnames = _symtab.Select(symbol => symbol.Label).ToList();
            
            int listindex = symbolnames.IndexOf(name);
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
                index += symbolnames[i].Length + 1;
            }

            return index;

        }
    }
}