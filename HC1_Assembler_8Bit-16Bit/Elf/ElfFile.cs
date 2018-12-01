using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Assembler;
using HC1_Assembler_8Bit_16Bit.Elf.Sections;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;

namespace HC1_Assembler_8Bit_16Bit.Elf
{
    public class ElfFile
    {
        private ElfHeader _elfHeader;
        private SectionHeaderStringTable _sectionHeaderStringTable;
        private StringTable _stringTable;
        private SymbolTable _symbolTable;
        private RelocationText _relocationText;
        private TextSection _textSection;

        private List<byte> Bytes { get; set; }
        
        /// <summary>
        /// Generates all relvant ELF sections and appends them into a new list of bytes.
        /// </summary>
        /// <param name="assemblyData"> The current assemblydata. </param>
        /// <returns> A list of bytes conatianing all relevant sections. </returns>
        public List<byte> GenerateSections(AssemblyData assemblyData)
        {
            Bytes = new List<byte>();
            
            Bitwidth bitwidth = Bitwidth.Bit16;
            switch (assemblyData.AssemblerDirectives.InstructionSize)
            {
                case 8:
                    bitwidth = Bitwidth.Bit8;
                    break;
                case 16:
                    bitwidth = Bitwidth.Bit16;
                    break;
                default:
                    ExceptionHandler.ThrowInstructionSizeException(Sender.Assembler, assemblyData.AssemblerDirectives.InstructionSize, assemblyData.Path);
                    break;
            }

            List<byte> program = Assembler.Assembler.AssembleToBinary(assemblyData).ToList();
            
            int estimatedElfHeaderSize = EstimateElfHeaderSize(bitwidth);
            
            _sectionHeaderStringTable = new SectionHeaderStringTable();
            _stringTable = new StringTable(assemblyData.Symboltable);
            _symbolTable = new SymbolTable(assemblyData.Symboltable, _stringTable);
            _relocationText = new RelocationText(assemblyData.SymbolLinkageInfos);
            _textSection = new TextSection(program);
            
 
            List<IElfSection> sections = new List<IElfSection>
            {
                _sectionHeaderStringTable,
                _stringTable,
                _symbolTable,
                _relocationText,
                _textSection
            };
            
            _elfHeader = new ElfHeader(bitwidth, estimatedElfHeaderSize, sections.Count);

            int estimatedSectionHeaderSize = EstimateSectionHeaderSize(sections);

            List<byte> sectionHeaders = new List<byte>();
            List<byte> sectionContens = new List<byte>();
            
            Bytes.AddRange(_elfHeader.GenerateElfHeader());
            
            foreach (IElfSection section in sections)
            {
                sectionHeaders.AddRange(section.GenerateSectionHeader(_sectionHeaderStringTable, 0, _elfHeader.Bytes.Count + estimatedSectionHeaderSize + 
                                                                                                   sectionContens.Count).Generate());
                sectionContens.AddRange(section.Bytes);
            }
            
            
            Bytes.AddRange(sectionHeaders);
            Bytes.AddRange(sectionContens);

            
            
            return Bytes;
        }

        /// <summary>
        /// Calculates the size the section headers are going to need.
        /// </summary>
        /// <param name="sections"> A collection of all sections. </param>
        /// <returns> The estimated size. </returns>
        private int EstimateSectionHeaderSize(IEnumerable<IElfSection> sections)
        {
            List<byte> estimationSectionHeaders = new List<byte>();
            foreach (IElfSection section in sections)
            {
                estimationSectionHeaders.AddRange(section.GenerateSectionHeader(_sectionHeaderStringTable, 0, 0).Generate());
            }

            return estimationSectionHeaders.Count;
        }

        /// <summary>
        /// Calculates the size the ELF header is going to need.
        /// </summary>
        /// <param name="bitwidth"> The current bitwidth. 8 or 16. </param>
        /// <returns> The estimated size. </returns>
        private int EstimateElfHeaderSize(Bitwidth bitwidth)
        {
            ElfHeader elfHeader = new ElfHeader(bitwidth, 0, 0);
            return elfHeader.GenerateElfHeader().Count;
        }
    }
}