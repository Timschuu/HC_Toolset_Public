using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers;

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands
{
    [Command("-startsession")]
    public class SessionCommand : ICommand
    {
        private readonly List<string> _args;
        
        public SessionCommand(IEnumerable<string> args)
        {
            List<string> files = args.ToList();
            if (files.Count != 2)
            {
                ExceptionHandler.ThrowConsoleArgumentException();
                return;
            }

            _args = files;
        }
        
        public void Execute()
        {
            Parser parser = new Parser(Program.CommandPrefix, Program.SubCommandPrefix);
            foreach (CommandType type in CommandRegister.GetEmulatorCommandRegister())
            {
                parser.RegisterCommand(type);
            }

            parser.InvalidSubCommand += (s, e) => { ExceptionHandler.ThrowConsoleCommandArgumentException(e.Command, e.Subcommand, false); };
            parser.WrongCommandUsage += (s, e) => { ExceptionHandler.ThrowConsoleArgumentException(false); };
            parser.UnknownCommand += (s, e) => { ExceptionHandler.ThrowConsoleArgumentException(false); };

            if (!int.TryParse(_args[1], out int instructionsize))
            {
                ExceptionHandler.ThrowConsoleArgumentException();
            }
            
            Emulator.Emulator.NewEmulatorContext(_args[0], instructionsize);
            EmulatorContext emulatorContext = Emulator.Emulator.GetEmulatorContext();
            Contract.AssertNotNull(emulatorContext, nameof(emulatorContext));
            emulatorContext.Parser = parser;
            
            while (true)
            {
                Console.WriteLine("Enter Command: ");
                string[] input = Console.ReadLine()?.Split(' ') ?? Array.Empty<string>();
                if (input.FirstOrDefault() == "exit")
                {
                    break;
                }
                
                for (int i = 0; i < input.Length; i++)
                {
                    parser.ParseCommand(input[i], Program.GetFilesUntilNextArgument(ref i, input));
                }
            }
        }
    }
}