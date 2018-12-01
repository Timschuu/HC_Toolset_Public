using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers;

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.EmulatorCommands
{
    [Command("-show")]
    public class ShowAssemblyCommand : BoxedCommand, ICommand
    {
        public ShowAssemblyCommand(IEnumerable<string> args)
        {
            if (!args.Any()) return;
            ExceptionHandler.ThrowConsoleArgumentException(false);
            Cancel = true;
        }
        
        public void Execute()
        {
            if (Cancel)
            {
                return;
            }

            EmulatorContext context = Emulator.Emulator.GetEmulatorContext();
            Contract.AssertNotNull(context, nameof(context));
            context.WriteAssemblyToConsole();
        }
    }
}