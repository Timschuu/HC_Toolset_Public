using System;
using System.Collections.Generic;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Helpers;

// ReSharper disable UnusedParameter.Local

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands
{
    [Command("-h")]
    public class HelpCommand : ICommand
    {
        public HelpCommand(IEnumerable<string> args)
        {
            
        }
        
        public void Execute()
        {
            EmulatorContext emulatorContext = Emulator.Emulator.GetEmulatorContext();
            Contract.AssertNotNull(emulatorContext, nameof(emulatorContext));

            Console.WriteLine("Available Commands:");
            Console.WriteLine(emulatorContext.Parser.GetHelpString());
        }
    }
}