using System;
using System.IO;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;
using HC1_Assembler_8Bit_16Bit.SystemHalf;

namespace HC1_Assembler_8Bit_16Bit.Handler
{
    public static class ExceptionHandler
    {
        private static void WriteException(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[Error] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        public static void ThrowInstructionSizeException(Sender sender, int instructionSize, string file)
        {
            WriteException($"[{sender}] Unknown instructionsize: {instructionSize} in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }

        public static void ThrowInternalException(Exception e)
        {
            WriteException("An internal exception occured. Please check the errorlog for futher information.");
            WriteErrorLog(e);
            Environment.Exit(1);
        }
        
        public static void ThrowInstructionSizeException(Sender sender, int instructionSize)
        {
            WriteException($"[{sender}] Unknown instructionsize: {instructionSize}.");
            Environment.Exit(1);
        }
        
        public static void ThrowEndianessException(Sender sender, string file)
        {
            WriteException($"[{sender}] Unknown endianess in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }

        public static void ThrowMnemonicException(string mnem, int line, string file)
        {
            WriteException($"[Assembler] Mnemonic \"{mnem}\" on line {line} in file \"{file.PathToFileName()}\" not supported.");
            Environment.Exit(1);
        }

        public static void ThrowDifferentInstructionsizeException()
        {
            WriteException("[Linker] Cannot link elf files with different instruction sizes.");
            Environment.Exit(1);
        }

        public static void ThrowInvalidFloatParameterException(int line, string file)
        {
            WriteException($"[Assembler] Invalid usage of float parameter on line {line} in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }

        public static void ThrowLabelNotFoundException(string label)
        {
            WriteException($"[Linker] Label \"{label}\" could not be resolved.");
            Environment.Exit(1);
        }

        public static void ThrowParameterOutOfRangeException(int line, string file, int value, int min, int max)
        {
            WriteException($"[Assembler] Parameter out of valid range on line {line} in file \"{file.PathToFileName()}\". " +
                           $"Parameter has value {value} (Range is {min} to {max}).");
            Environment.Exit(1);
        }
        
        public static void ThrowParameterOutOfRangeException(int line, string file, Half min, Half max)
        {
            WriteException($"[Assembler] Parameter out of valid range on line {line} in file \"{file.PathToFileName()}\". " +
                           $"(Range is {min} to {max}).");
            Environment.Exit(1);
        }

        public static void ThrowParameterOutOfRangeException(string label, string instruction, int value, int min, int max)
        {
            WriteException($"[Linker] Value of label \"{label}\" exceeds parameter range of instruction \"{instruction}\". " +
                           $"Parameter has value {value} (Range is {min} to {max}).");
            Environment.Exit(1);
        }

        public static void ThrowParameterCountException(int line, string file, int given, int expected)
        {
            WriteException($"[Assembler] Instruction on line {line} expected {expected} parameters but got {given} in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }

        public static void ThrowInvalidHexNumberException(int line, string file)
        {
            WriteException($"[Assembler] Invalid hex number on line {line} in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }

        public static void ThrowFileGenerationException(Sender sender, string filename, Exception e)
        {
            WriteException($"[{sender}] File \"{filename.PathToFileName()}\" could not be generated.");
            WriteErrorLog(e);
            Environment.Exit(1);
        }

        public static void ThrowInvalidSymbolNameException(string name, int line, string file)
        {
            WriteException($"[Assembler] Invalid symbol name \"{name}\" on line {line} in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }

        public static void ThrowFileReadingException(Sender sender, string filename, Exception e)
        {
            WriteException($"[{sender}] File \"{filename.PathToFileName()}\" could not be found.");
            WriteErrorLog(e);
            Environment.Exit(1);
        }

        public static void ThrowUnknownException(Sender sender, Exception e)
        {
            WriteException($"[{sender}] Unknown error, please check the errorlog file for more information.");
            WriteErrorLog(e);
            Environment.Exit(1);
        }

        public static void ThrowEmptyLabelException(int line, string file)
        {
            WriteException($"[Assembler] Empty label at line {line} in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }
        
        public static void ThrowConsoleArgumentException(bool endProgram = true)
        {
            WriteException("Invalid arguments. Use -h to display help on how to use the assembler and linker.");
            if (endProgram)
            {
                Environment.Exit(1);
            }
        }

        public static void ThrowConsoleCommandArgumentException(string command, string commandargument, bool endProgram = true)
        {
            WriteException($"Invalid arguments. Argument \"{commandargument}\" is not defined for command \"{command}\"." +
                           " Use -h to display help on how to use the assembler and linker.");
            if (endProgram)
            {
                Environment.Exit(1);
            }
        }

        public static void ThrowInvalidCommandArgumentUsageException(string command, string commandargument)
        {
            WriteException($"Invalid usage of \"{commandargument}\" for command \"{command}\"." +
                           " Use -h to display help on how to use the assembler and linker.");
            Environment.Exit(1);
        }

        public static void ThrowAssemblerDirectiveException(string line, int linenumber, string file)
        {
            WriteException($"[Assembler] Invalid assembler directive: \"{line}\" on line {linenumber} in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }

        public static void ThrowMultipleLabelDeclarationException(string label, string file)
        {
            WriteException($"[Assembler] Label \"{label}\" declared multiple times in file \"{file.PathToFileName()}\".");
            Environment.Exit(1);
        }
        
        public static void ThrowMultipleLabelDeclarationException(string label)
        {
            WriteException($"[Linker] Label \"{label}\" declared multiple times.");
            Environment.Exit(1);
        }

        public static void ThrowMainLabelNotFoundException()
        {
            WriteException("[Linker] Required label \"main\" could not be found.");
            Environment.Exit(1);
        }

        public static void ThrowMultipleMainLabelException()
        {
            WriteException("[Linker] Unique label \"main\" has been found multiple times.");
            Environment.Exit(1);
        }

        public static void ThrowInvalidRegisterOperationException(int register)
        {
            WriteException($"[Emulator] Failed to access register {register}.");
            Environment.Exit(1);
        }
        
        public static void ThrowInvalidMemoryOperationException(int address)
        {
            WriteException($"[Emulator] Failed to access memory address {address}.");
            Environment.Exit(1);
        }

        public static void ThrowNoEmulatorSessionFoundException()
        {
            WriteException("[Emulator] No emulator session has been found. You need to start the emulator before running this command.");
            Environment.Exit(1);
        }

        public static void ThrowFileTypeException(Sender sender, string file)
        {
            WriteException($"[{sender}] File \"{file.PathToFileName()}\" is not an ELF file.");
            Environment.Exit(1);
        }

        public static void ThrowInvalidFileException(Sender sender, string file)
        {
            WriteException($"[{sender}] File \"{file.PathToFileName()}\" is invalid.");
            Environment.Exit(1);
        }

        private static void WriteErrorLog(Exception e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string errorlogpath = path + "\\..\\" + "errorlog.txt";
            using (StreamWriter streamWriter = File.AppendText(errorlogpath))
            {
                streamWriter.WriteLine("[" + DateTime.Now + "] " + e + "\n");
            }
        }
    }
}