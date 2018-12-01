using System;
using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;

namespace HC1_Assembler_8Bit_16Bit.Elf
{
    public class SectionHeader
    {
        public int SectionHeaderStringTableIndex { get; }
        private readonly int _type;
        private readonly int _address;
        private readonly int _offset;
        public int Size { get; }
        
        /// <summary>
        /// The constructor the assembler uses to generate the section header.
        /// </summary>
        /// <param name="symboltableIndex"> The index to its name in the sectionheaderstringtable. </param>
        /// <param name="type"> Its type. </param>
        /// <param name="address"> The address this section is laying. </param>
        /// <param name="offset"> The offset from the begining of the file to the corresponding section. </param>
        /// <param name="size"> The size of the corresponding section. </param>
        public SectionHeader(int symboltableIndex, int type, int address, int offset, int size)
        {
            SectionHeaderStringTableIndex = symboltableIndex;
            _type = type;
            _address = address;
            _offset = offset;
            Size = size;
        }

        /// <summary>
        /// The constructor the linker uses to reconstruct the section header from binaries.
        /// Removes the parsed bytes after it is done.
        /// </summary>
        /// <param name="binaries"> The current ELF file that has been shortened to start at the begining of the current section header. </param>
        /// <exception cref="InvalidOperationException"> The binaries are to short to contain an ELF section header. </exception>
        public SectionHeader(List<byte> binaries)
        {
            if (binaries.Count < 40)
            {
                throw new InvalidOperationException();
            }

            SectionHeaderStringTableIndex = binaries.ReadFourBytes(0);
            _type = binaries.ReadFourBytes(4);
            _address = binaries.ReadFourBytes(12);
            _offset = binaries.ReadFourBytes(16);
            Size = binaries.ReadFourBytes(20);
            binaries.RemoveRange(0, 40);
        }
        
        /// <summary>
        /// Generates the ELF section header from the information that it has been given in the constructor.
        /// </summary>
        /// <returns> A list of bytes that contain the entire ELF section header. </returns>
        public List<byte> Generate()
        {
            List<byte> bytelist = new List<byte>();
            
            bytelist.AddFourBytes(SectionHeaderStringTableIndex);//Index to Symboltable
            bytelist.AddFourBytes(_type);//Type
            bytelist.AddFourBytes(0);//Flags
            bytelist.AddFourBytes(_address);//Address
            bytelist.AddFourBytes(_offset);//Offset
            bytelist.AddFourBytes(Size);//Size
            bytelist.AddFourBytes(0);//Link
            bytelist.AddFourBytes(0);//Info
            bytelist.AddFourBytes(0);//AddressAllign
            bytelist.AddFourBytes(0);//ENT_Size

            return bytelist;
        }
    }
}