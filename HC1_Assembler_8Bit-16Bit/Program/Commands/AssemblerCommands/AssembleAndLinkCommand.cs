using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace HC1_Assembler_8Bit_16Bit.Program.Commands.AssemblerCommands
{
    [Command("-al")]
    public class AssembleAndLinkCommand : ICommand
    {
        private readonly List<string> _args;
        private string _outputname = "";
        private bool _debug;

        public AssembleAndLinkCommand(IEnumerable<string> args)
        {
            List<string> files = args.ToList();
            if (files.Count == 0)
            {
                ExceptionHandler.ThrowConsoleArgumentException();
                return;
            }

            _args = files;
        }
        
        public void Execute()
        {
            foreach (string file in _args)
            {
                Assembler.Assembler.GenerateFilesetOutput(file, Assembler.Assembler.AssembleToByteArray(file), "o");
            }
            ILinker linker = new Linker.Linker();
            foreach (string file in _args)
            {
                string objectfile = file.Substring(0, file.Length - 3);
                objectfile += "o";
                linker.AddElfFile(objectfile);
            }
            linker.Link();

            if (_debug)
            {
                linker.GenerateDebugOutput("debug.txt");
                return;
            }
            
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
                ExceptionHandler.ThrowInvalidCommandArgumentUsageException("-al", "--o");
            }

            _outputname = args[0];
        }

        [CommandArgument("--d")]
        private void DebugSubCommand(List<string> args)
        {
            _debug = true;
        }
    }
}