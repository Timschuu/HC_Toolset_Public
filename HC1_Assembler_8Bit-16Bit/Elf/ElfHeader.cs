using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;

namespace HC1_Assembler_8Bit_16Bit.Elf
{
    public class ElfHeader
    {
        private readonly Bitwidth _bitwidth;
        private readonly int _sectionHeaderOffset;
        public List<byte> Bytes { get; private set; }
        private readonly int _sectionHeaderCount;

        public ElfHeader(Bitwidth bitwidth, int sectionHeaderOffset, int sectionHeaderCount)
        {
            _bitwidth = bitwidth;
            _sectionHeaderOffset = sectionHeaderOffset;
            _sectionHeaderCount = sectionHeaderCount;
        }
        
        /// <summary>
        /// Generates the ELF header.
        /// </summary>
        /// <returns> A list of bytes containing the entire header. </returns>
        public List<byte> GenerateElfHeader()
        {
            //Reference: http://www.cirosantilli.com/elf101.png
            //https://medium.com/@MrJamesFisher/understanding-the-elf-4bd60daac571
            Bytes = new List<byte>
            {
                0x7F, //EI_MAG
                0x45,
                0x4C,
                0x46,
                (byte) _bitwidth, //EI_CLASS
                (byte) Endianness.Be, //EI_DATA
                0x01, //EI_VERSION
                0x00, 
                0x00,
                0x00,
                0x00, //Buffer
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x01, //e_type
                0x00,
                0x00, //e_machine
                0x00,
                0x01, //e_version
                0x00,
                0x00,
                0x00,
                0x00, //e_entry
                0x00,
                0x00,
                0x00,
                0x00, //e_phoff
                0x00,
                0x00,
                0x00
            };
              
            Bytes.AddFourBytes(_sectionHeaderOffset); //e_shoff
            
            List<byte> part = new List<byte>
            {
                0x00, //Buffer
                0x00,
                0x00,
                0x00,
                0x34, //e_ehsize
                0x00,
                0x00, //e_phentsize
                0x00,
                0x00, //e_phnum
                0x00,
                0x00, //e_shentsize
                0x00,
                (byte)_sectionHeaderCount, //e_shnum
                0x00,
                0x00, //e_shstrndx
                0x00
            };
            Bytes.AddRange(part);
            return Bytes;
        }
    }

}