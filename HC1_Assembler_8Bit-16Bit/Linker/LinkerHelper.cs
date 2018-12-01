using System;
using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Elf;
using HC1_Assembler_8Bit_16Bit.Elf.Sections;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Operations;

namespace HC1_Assembler_8Bit_16Bit.Linker
{
    public static class LinkerHelper
    {
        /// <summary>
        /// Gets the corresponding operation from an instrucion.
        /// </summary>
        /// <param name="instruction"> An 8 or 16 bit instruction. </param>
        /// <param name="instructionsize"> The current instructionsize. </param>
        /// <returns> The corresponding IOperation. </returns>
        /// <exception cref="InvalidOperationException"> Corrupted ELF file or unknown instruction. </exception>
        public static IOperation GetOperationFromInstruction(int instruction, int instructionsize)
        {
            List<IOperation> operations = OperationRegister.GetOperations(instructionsize);
            if (operations == null) 
            {
                throw new InvalidOperationException();
            }

            int opcode = GetInstructionOpCode(instruction, instructionsize);
            
            List<IOperation> foundOperations = operations.Where(o => o.Opcode == opcode).ToList();

            if (foundOperations.Count <= 1)
            {
                return foundOperations.FirstOrDefault();
            }
            
            int decidingParameter = instruction & 0x1F;
            IOperation operation;
            switch (decidingParameter)
            {
                case 0:
                case 1:
                    operation = foundOperations.FirstOrDefault(o => o is OpIo opIo && opIo.DataSubst == decidingParameter);
                    break;
                default:
                    operation = foundOperations.FirstOrDefault(o => !(o is OpIo));
                    break;
            }

            if (operation == null)
            {
                throw new InvalidOperationException();
            }

            return operation;
        }

        /// <summary>
        /// Gets the ppcode from an given instruction.
        /// </summary>
        /// <param name="instruction"> An 8 or 16 bit instruction. </param>
        /// <param name="instructionsize"> The current instructionsize. </param>
        /// <returns> The opcode </returns>
        /// <exception cref="InvalidOperationException"> Instructionsize is not 8 or 16. </exception>
        public static int GetInstructionOpCode(int instruction, int instructionsize)
        {
            switch (instructionsize)
            {
                case 8:
                    return (instruction & 0xE0) >> 5;
                case 16:
                    return (instruction & 0xFC00) >> 10;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Maps the names found in any stringtable to its index given in an ELF section header.
        /// </summary>
        /// <param name="elfFileContainer"> The current ELF file. </param>
        public static void PostMapSections(ElfFileContainer elfFileContainer)
        {
            foreach (ElfSectionPair pair in elfFileContainer.Sections)
            {
                if (pair.Section is IMappingRequester mappingRequester && mappingRequester.NeedsMapping)
                {
                    mappingRequester.Map(elfFileContainer);
                }
            }
        }

        /// <summary>
        /// Finds the to its name corresponding section header from an ELF file.
        /// </summary>
        /// <param name="elfFileContainer"> The current ELF file. </param>
        /// <param name="sectionname"> The name of the section header. </param>
        /// <returns> The first section header, that has been found. </returns>
        /// <exception cref="InvalidOperationException"> Sectionname is unknown or doesnt exist. </exception>
        public static SectionHeader RequestSectionHeader(ElfFileContainer elfFileContainer, string sectionname)
        {
            SectionHeader sectionHeader = elfFileContainer.Sections.FirstOrDefault(s =>
                (elfFileContainer.Sections[0].Section as SectionHeaderStringTable)?.GetNameFromIndex(s.Header
                    .SectionHeaderStringTableIndex) == sectionname)?.Header;
            
            if (sectionHeader == null)
            {
                throw new InvalidOperationException();
            }

            return sectionHeader;
        }
        
        /// <summary>
        /// Finds the to its name corresponding section from an ELF file.
        /// </summary>
        /// <param name="elfFileContainer"> The current ELF file. </param>
        /// <param name="sectionname"> The name of the section. </param>
        /// <returns> The first section, that has been found. </returns>
        /// <exception cref="InvalidOperationException"> Sectionname is unknown or doesnt exist. </exception>
        public static IElfSection RequestSection(ElfFileContainer elfFileContainer, string sectionname)
        {
            IElfSection section = elfFileContainer.Sections.FirstOrDefault(s =>
                (elfFileContainer.Sections[0].Section as SectionHeaderStringTable)?.GetNameFromIndex(s.Header
                    .SectionHeaderStringTableIndex) == sectionname)?.Section;

            if (section == null)
            {
                throw new InvalidOperationException();
            }

            return section;
        }
    }
}