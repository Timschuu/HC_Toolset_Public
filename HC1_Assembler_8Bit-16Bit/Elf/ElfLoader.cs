using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;
using HC1_Assembler_8Bit_16Bit.Linker;

namespace HC1_Assembler_8Bit_16Bit.Elf
{
    public static class ElfLoader
    {
        
        /// <summary>
        /// Tries to read an ELF file and throws an exception in case it fails.
        /// </summary>
        /// <param name="path"> The path of the ELF file. </param>
        /// <returns> The binary file in an list of bytes. </returns>
        public static List<byte> LoadBinaryFile(string path)
        {
            try
            {
                byte[] input = File.ReadAllBytes(path);
                return input.ToList();
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException || e is DirectoryNotFoundException)
                {
                    ExceptionHandler.ThrowFileReadingException(Sender.Linker, path, e);
                }
                else
                {
                    ExceptionHandler.ThrowUnknownException(Sender.Linker, e);
                }
            }

            return null;
        }

        /// <summary>
        /// Reads all relevant information from the elf header.
        /// </summary>
        /// <param name="binaries"> The binary elf file that has been read. </param>
        /// <param name="file"> The path to the ELF file. </param>
        /// <returns> A class containing all relevant information found in the ELF header. </returns>
        public static ElfHeaderLinkageInfo ParseElfHeader(List<byte> binaries, string file)
        {
            if (binaries.Count < 0x34)
            {
                ExceptionHandler.ThrowInvalidFileException(Sender.Linker, file);
            }

            int rawinstructionsize = binaries[4];
            int instructionsize = 0;
            
            switch (rawinstructionsize)
            {
                case 0x03:
                    instructionsize = 8;
                    break;
                case 0x04:
                    instructionsize = 16;
                    break;
                default:
                    ExceptionHandler.ThrowInstructionSizeException(Sender.Linker, 0, file);
                    break;       
            }
            
            int rawendianess = binaries[5];
            Endianness endianess = Endianness.Be;
            switch (rawendianess)
            {
                case 0x01:
                    endianess = Endianness.Le;
                    break;
                case 0x02:
                    endianess = Endianness.Be;
                    break;
                default:
                    ExceptionHandler.ThrowEndianessException(Sender.Linker, file);
                    break;
            }
            
            int headersize = binaries[40];
            int sectionheaderoffset = binaries.ReadFourBytes(35);
            int sectionHeaderCount = binaries[48];
            binaries.RemoveRange(0, headersize);
            
            return new ElfHeaderLinkageInfo(instructionsize, headersize, endianess, sectionheaderoffset, sectionHeaderCount);
        }
        
        /// <summary>
        /// Checks the first 4 bytes of an Elf file and returns true if they match the magic elf identifier.
        /// </summary>
        /// <param name="binaries"> An read elf file. </param>
        /// <returns> True, if the file identifies as an elf file. Otherwise false. </returns>
        public static bool VerifyElfFile(List<byte> binaries)
        {
            if (binaries.Count < 4)
            {
                return false;
            }
            List<byte> checkvalue = new List<byte>
            {
                0x7F, //EI_MAG
                0x45,
                0x4C,
                0x46
            };
            return !checkvalue.Where((b, i) => binaries[i] != b).Any();
        }
    }
}