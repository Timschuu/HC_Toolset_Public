using System.Collections.Generic;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Program.Commands.AssemblerCommands;
using HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands;
using HelpCommand = HC1_Assembler_8Bit_16Bit.Program.Commands.AssemblerCommands.HelpCommand;

namespace HC1_Assembler_8Bit_16Bit.Program
{
    public static class CommandRegister
    {
        public static IEnumerable<CommandType> GetGlobalCommandRegister()
        {
            yield return new CommandType(typeof(AssembleCommand), "-a File[s] \t\t\t Assembles all whitespace seperated files to relocateable elf files.");
            yield return new CommandType(typeof(AssembleAndLinkCommand), "-al File[s] [--o Outputfile] \t Assembles and links all files and creates the given outputfile.");
            yield return new CommandType(typeof(HelpCommand), "-h         \t\t\t Shows help.");
            yield return new CommandType(typeof(LinkCommand), "-l File[s] [--o Outputfile] \t Links all files and creates the given outputfile.");
            yield return new CommandType(typeof(SymboltableCommand), "-s File[s] \t\t\t Generates a unicode symboltabel for each file.");
            yield return new CommandType(typeof(UnicodeCommand), "-u File[s] \t\t\t Assembles all whitespace seperated files to unicode representation.");
            yield return new CommandType(typeof(VhdlCommand), "-v File[s] \t\t\t Assembles all whitespace seperated files to vhdl representations.");
            yield return new CommandType(typeof(SessionCommand), "-startsession Program Instructionsize \n\t\t\t\t Starts a new emulator and interactive console.");
        }

        public static IEnumerable<CommandType> GetEmulatorCommandRegister()
        {
            yield return new CommandType(typeof(Commands.EmulatorCommands.HelpCommand), "-h         \t\t Shows help.");
            yield return new CommandType(typeof(ShowRegisterValuesCommand), "-regval \t\t Shows the values of all registers.");
            yield return new CommandType(typeof(SetRegisterValueCommand), "-setreg Value \t\t Sets the value of a register.");
            yield return new CommandType(typeof(ShowMemoryValuesCommand), "-memval From To \t Shows the memory values from all memory addresses between From and To.");
            yield return new CommandType(typeof(StepCommand), "-step \t\t\t Executes the next instruction.");
            yield return new CommandType(typeof(ExecuteProgramCommand), "-exec [--to pc] \t Executes the entire program [until the given programcounter is reached].");
            yield return new CommandType(typeof(BreakpointCommand), "-break [--add pc / --remove pc / --clear / --show] \n\t\t\t Lets you control your breakpoints.");
            yield return new CommandType(typeof(ShowPcCommand), "-pc \t\t\t Displays the current programcounter.");
            yield return new CommandType(typeof(ShowAssemblyCommand), "-show \t\t\t Shows the current assembly.");
        }
    }
}