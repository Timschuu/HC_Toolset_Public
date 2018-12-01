using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers;

// ReSharper disable UnusedMember.Local

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands
{
    [Command("-exec")]
    public class ExecuteProgramCommand : BoxedCommand, ICommand
    {
        private int? _toProgramCounter;
        
        public ExecuteProgramCommand(IEnumerable<string> args)
        {
            if (args.Any())
            {
                ExceptionHandler.ThrowConsoleArgumentException(false);
                Cancel = true;
            }
        }
        
        public void Execute()
        {
            if (Cancel)
            {
                return;
            }
            
            EmulatorContext emulatorContext = Emulator.Emulator.GetEmulatorContext();
            Contract.AssertNotNull(emulatorContext, nameof(emulatorContext));

            int counter = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            if (_toProgramCounter == null)
            {
                while (emulatorContext.EmulateNext(false))
                {
                    counter++;
                }
                stopwatch.Stop();
            }
            else
            {
                while (emulatorContext.ProgramCounter < _toProgramCounter)
                {
                    if (!emulatorContext.EmulateNext(false))
                    {
                        break;
                    }
                }
            }

            Console.WriteLine($"Executed {counter} instructions in {stopwatch.Elapsed}.");
            
        }

        [CommandArgument("--to")]
        private void ToCommandArgument(IEnumerable<string> args)
        {
            List<string> arguments = args.ToList();
            if (arguments.Count != 1 || !int.TryParse(arguments[0], out int pc))
            {
                ExceptionHandler.ThrowConsoleArgumentException(false);
                Cancel = true;
                return;
            }

            _toProgramCounter = pc;

        }
    }
}