using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Handler;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.AssemblerCommands
{
    [Command("-a")]
    public class AssembleCommand : ICommand
    {
        private readonly List<string> _args;
        
        public AssembleCommand(IEnumerable<string> args)
        {
            List<string> files = args.ToList();
            if (!files.Any())
            {
                ExceptionHandler.ThrowConsoleArgumentException();
            }
            _args = files;
        }
        
        public void Execute()
        {
            foreach (string file in _args)
            {
                Assembler.Assembler.GenerateFilesetOutput(file, Assembler.Assembler.AssembleToByteArray(file), "o");
            }
        }
    }
}