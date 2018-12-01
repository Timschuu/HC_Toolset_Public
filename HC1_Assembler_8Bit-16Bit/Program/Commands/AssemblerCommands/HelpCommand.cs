using System;
using System.Collections.Generic;
using ConsoleArgumentParser;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.AssemblerCommands
{
    [Command("-h")]
    public class HelpCommand : ICommand
    {
        public HelpCommand(IEnumerable<string> args)
        {
            
        }
        
        public void Execute()
        {
            Console.WriteLine("This is an assembler and linker for HC assembly that converts multiple assembly files to the specified output.\n");
            Console.WriteLine(Program.Parser.GetHelpString());
        }
    }
}