using System;
using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Linker;
using HC1_Assembler_8Bit_16Bit.Shared;
using HC1_Assembler_8Bit_16Bit.SystemHalf;

// ReSharper disable UnusedMember.Global

namespace HC1_Assembler_8Bit_16Bit.Elf.Sections
{
    [ElfSection(".symtab")]
    public class SymbolTable : IElfSection, IMappingRequester
    {
        public const string Name = ".symtab";
        private const int Type = 2;
        public List<byte> Bytes { get; set; }
        public List<Symbol> Symtab { get; }
        private readonly StringTable _stringTable;
        
        public SymbolTable(List<Symbol> symtab, StringTable stringTable)
        {
            Symtab = symtab;
            _stringTable = stringTable;
            Bytes = Generate();
        }

        [ElfSectionConstructor]
        public SymbolTable(ElfFileContainer elfFileContainer, List<byte> binaries, SectionHeader header)
        {
            NeedsMapping = false;
            StringTable stringTable = (StringTable) LinkerHelper.RequestSection(elfFileContainer, StringTable.Name);
            if (stringTable == null)
            {
                NeedsMapping = true;
            }

            List<Symbol> symtab = new List<Symbol>();
            
            for (int i = 0; i < header.Size; i += 18)
            {
                string label = "";
                int index = 0;
                if (!NeedsMapping && stringTable != null)
                {
                    label = stringTable.GetNameFromIndex(binaries.ReadFourBytes(0));
                }
                else
                {
                    index = binaries.ReadFourBytes(0);
                }
                int value = binaries.ReadFourBytes(4);
                int rawisdata = binaries[13];
                bool isdata;
                switch (rawisdata)
                {
                    case 1:
                        isdata = true;
                        break;
                    case 2:
                        isdata = false;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                int rawdefinedhere = binaries.ReadFourBytes(14);
                bool definedhere;
                switch (rawdefinedhere)
                {
                    case 0:
                        definedhere = false;
                        break;
                    case 1:
                        definedhere = true;
                        break;
                    default:
                        throw new InvalidOperationException();          
                }
                
                binaries.RemoveRange(0, 18);
                symtab.Add(new Symbol(value, label, 0, isdata, value, index, definedhere, typeof(int)));
            }

            Symtab = symtab;
        }
        
        public List<byte> Generate()
        {
            List<byte> bytelist = new List<byte>();

            foreach (Symbol sym in Symtab)
            {
                bytelist.AddFourBytes(_stringTable.FindNameIndex(sym.Label));//Index
                if (sym.Type == typeof(Half))
                {
                    bytelist.AddFourBytes(sym.HalfData);
                }
                else
                {
                    bytelist.AddFourBytes(sym.IsData ? sym.Data : sym.Address);//Value
                }
                bytelist.AddFourBytes(4);//Size
                bytelist.Add(0x01);//Info_Global
                bytelist.Add(sym.IsData ? (byte)SymbolType.Object : (byte)SymbolType.Func);//Info_Type
                bytelist.AddFourBytes(sym.DefinedHere ? 0x01 : 0x00);//Defined Here
            }
            return bytelist;
        }

        public SectionHeader GenerateSectionHeader(SectionHeaderStringTable shst, int adress, int offset)
        {
            return new SectionHeader(shst.FindNameIndex(Name), Type, 0, offset, Bytes.Count);
        }

        public bool NeedsMapping { get; set; }
        public void Map(ElfFileContainer elfFileContainer)
        {
            StringTable stringTable = (StringTable) LinkerHelper.RequestSection(elfFileContainer, StringTable.Name);

            if (stringTable == null)
            {
                ExceptionHandler.ThrowInvalidFileException(Sender.Linker, elfFileContainer.File);
                return;
            }

            foreach (Symbol symbol in Symtab)
            {
                symbol.Label = stringTable.GetNameFromIndex(symbol.Index);
            }
        }
    }
}