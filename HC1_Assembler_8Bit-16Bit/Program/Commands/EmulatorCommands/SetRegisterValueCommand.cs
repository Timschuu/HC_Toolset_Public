using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers;

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands
{
    [Command("-setreg")]
    public class SetRegisterValueCommand : BoxedCommand, ICommand
    {
        private readonly List<int> _arguments;
        
        public SetRegisterValueCommand(IEnumerable<string> args)
        {
            List<string> arguments = args.ToList();
            if (arguments.Count != 2)
            {
                ExceptionHandler.ThrowConsoleArgumentException(false);
                Cancel = true;
            }

            _arguments = arguments.Select(s =>
            {
                if (!int.TryParse(s, out int val))
                {
                    ExceptionHandler.ThrowConsoleArgumentException(false);
                    Cancel = true;
                }

                return val;
            }).ToList();
        }
        
        public void Execute()
        {
            if (Cancel)
            {
                return;
            }
            
            EmulatorContext emulatorContext = Emulator.Emulator.GetEmulatorContext();
            Contract.AssertNotNull(emulatorContext, nameof(emulatorContext));
            emulatorContext.SetRegisterValue(_arguments[0], _arguments[1]);
        }
    }
}