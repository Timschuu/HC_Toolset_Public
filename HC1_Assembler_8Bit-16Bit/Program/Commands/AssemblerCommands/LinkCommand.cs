using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.AssemblerCommands
{
    [Command("-l")]
    public class LinkCommand : ICommand
    {
        private readonly List<string> _args;
        private string _outputname = "";
        public LinkCommand(IEnumerable<string> args)
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
            ILinker linker = new Linker.Linker();
            foreach (string file in _args)
            {
                linker.AddElfFile(file);
            }
            
            linker.Link();
            if (_outputname == "")
            {
                linker.GenerateOutput("program.hc");
            }
            else
            {
                linker.GenerateOutput(_outputname);
            }
        }

        [CommandArgument("--o")]
        private void OutputSubCommand(List<string> args)
        {
            if (args.Count != 1)
            {
                ExceptionHandler.ThrowConsoleCommandArgumentException("-l", "--o");
            }

            _outputname = args[0];
        }
    }
}