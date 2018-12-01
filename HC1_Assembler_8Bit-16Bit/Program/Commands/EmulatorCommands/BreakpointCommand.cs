using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers;

// ReSharper disable UnusedMember.Local

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands
{
    [Command("-break")]
    public class BreakpointCommand : BoxedCommand, ICommand
    {
        private readonly EmulatorContext _emulatorContext;
        
        public BreakpointCommand(IEnumerable<string> args)
        {
            CheckForFalseInput(args);
            _emulatorContext = Emulator.Emulator.GetEmulatorContext();
            Contract.AssertNotNull(_emulatorContext, nameof(_emulatorContext));
        }

        private bool CheckForFalseInput(IEnumerable<string> args)
        {
            if (!args.Any()) return false;
            ExceptionHandler.ThrowConsoleArgumentException(false);
            Cancel = true;
            return true;

        }
    
        private int InputToInt(IEnumerable<string> args)
        {
            List<string> arguments = args.ToList();
            int output = 0;
            if (arguments.Count == 1 && int.TryParse(arguments[0], out output)) return output;
            ExceptionHandler.ThrowConsoleArgumentException(false);
            Cancel = true;
            return output;
        }

        [CommandArgument("--add")]
        private void AddCommand(IEnumerable<string> args)
        {
            int pc = InputToInt(args);
            Console.WriteLine(_emulatorContext.AddBreakpoint(pc) ? "Breakpoint added." : "Breakpoint already exists.");
        }

        [CommandArgument("--remove")]
        private void RemoveCommand(IEnumerable<string> args)
        {
            int pc = InputToInt(args);
            Console.WriteLine(_emulatorContext.RemoveBreakpoint(pc) ? "Breakpoint removed." : "Breakpoint does not exist.");
        }

        [CommandArgument("--clear")]
        private void ClearCommand(IEnumerable<string> args)
        {
            if (CheckForFalseInput(args)) return;
            _emulatorContext.ClearBreakpoints();
            Console.WriteLine("All breakpoints removed.");
        }

        [CommandArgument("--show")]
        private void ShowCommand(IEnumerable<string> args)
        {
            if (CheckForFalseInput(args)) return;
            _emulatorContext.WriteBreakpointsToConsole();
        }
        
        public void Execute()
        {

        }
    }
}