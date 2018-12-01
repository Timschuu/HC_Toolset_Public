using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers;

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands
{
    [Command("-regval")]
    public class ShowRegisterValuesCommand : BoxedCommand, ICommand
    {
        public ShowRegisterValuesCommand(IEnumerable<string> args)
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
            emulatorContext.WriteRegisterValuesToConsole();
        }
    }
}