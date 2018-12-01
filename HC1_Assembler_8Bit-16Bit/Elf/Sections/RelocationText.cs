using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Assembler;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Linker;
using HC1_Assembler_8Bit_16Bit.Shared;
// ReSharper disable UnusedMember.Global

namespace HC1_Assembler_8Bit_16Bit.Elf.Sections
{
    [ElfSection(".rela.text")]
    public class RelocationText : IElfSection
    {
        public List<byte> Bytes { get; set; }
        public const string Name = ".rela.text";
        private const int Type = 4;

        private readonly List<SymbolLinkageInfo> _symbolLinkageInfos;
        public List<RecreatedSymbolLinkageInfo> RecreatedSymbolLinkageInfos { get; }
        
        public RelocationText(List<SymbolLinkageInfo> symbolLinkageInfos)
        {
            _symbolLinkageInfos = symbolLinkageInfos;
            Bytes = Generate();
        }

        [ElfSectionConstructor]
        // ReSharper disable once UnusedParameter.Local
        public RelocationText(ElfFileContainer elfFileContainer, List<byte> binaries, SectionHeader header)
        {
            List<RecreatedSymbolLinkageInfo> symbolLinkageInfos = new List<RecreatedSymbolLinkageInfo>();
            for (int i = 0; i < header.Size; i += 9)
            {
                int offset = binaries.ReadFourBytes(0);
                int index = binaries.ReadFourBytes(4);
                int paranum = binaries[8];
                
                symbolLinkageInfos.Add(new RecreatedSymbolLinkageInfo(offset, index, paranum));
                binaries.RemoveRange(0, 9);
            }

            RecreatedSymbolLinkageInfos = symbolLinkageInfos;
        }
        
        public List<byte> Generate()
        {
            List<byte> output = new List<byte>();
            
            foreach (SymbolLinkageInfo sym in _symbolLinkageInfos)
            {
                output.AddFourBytes(sym.Offset); //Offset
                output.AddFourBytes(AssemblerHelper.CalculateSymbolTableOffset(sym.Symbol.Index)); //Index in Symtab
                output.Add((byte)sym.ParameterIndex); //Paranum
            }

            return output;
        }

        public SectionHeader GenerateSectionHeader(SectionHeaderStringTable shsti, int adress, int offset)
        {
            return new SectionHeader(shsti.FindNameIndex(Name), Type, 0, offset, Bytes.Count);
        }
    }
}