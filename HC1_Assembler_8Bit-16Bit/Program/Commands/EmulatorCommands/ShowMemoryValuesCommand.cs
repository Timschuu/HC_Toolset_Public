using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers;

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands
{
    [Command("-memval")]
    public class ShowMemoryValuesCommand : BoxedCommand, ICommand
    {
        private List<int> _args;
        
        public ShowMemoryValuesCommand(IEnumerable<string> args)
        {
            List<int> arguments = args.Select(s =>
            {
                if (!int.TryParse(s, out int val))
                {
                    ExceptionHandler.ThrowConsoleArgumentException(false);
                    Cancel = true;
                }

                return val;
            }).ToList();
            
            if (arguments.Count != 2)
            {
                ExceptionHandler.ThrowConsoleArgumentException(false);
                Cancel = true;
            }

            _args = arguments;
        }
        
        public void Execute()
        { 
            if (Cancel)
            {
                return;
            }
            
            EmulatorContext emulatorContext = Emulator.Emulator.GetEmulatorContext();
            Contract.AssertNotNull(emulatorContext, nameof(emulatorContext));
            emulatorContext.WriteMemoryValuesToConsole(_args[0], _args[1]);
        }
    }
}