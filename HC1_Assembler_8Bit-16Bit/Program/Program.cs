using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Handler;

namespace HC1_Assembler_8Bit_16Bit.Program
{
    public static class Program
    {    
        public static void Main(string[] args)
        {
            ProcessArguments(args);
        }

        public const string CommandPrefix = "-";
        public const string SubCommandPrefix = "--";

        public static Parser Parser { get; private set; }
        
        private static void ProcessArguments(IReadOnlyList<string> args)
        {
            Parser = new Parser(CommandPrefix, SubCommandPrefix);
            foreach (CommandType type in CommandRegister.GetGlobalCommandRegister())
            {
                Parser.RegisterCommand(type);
            }

            Parser.InvalidSubCommand += (s, e) => { ExceptionHandler.ThrowConsoleCommandArgumentException(e.Command, e.Subcommand); };
            Parser.WrongCommandUsage += (s, e) => { ExceptionHandler.ThrowConsoleArgumentException(); };
            Parser.UnknownCommand += (s, e) => { ExceptionHandler.ThrowConsoleArgumentException(); };
            
            if (!args.Any())
            {
                ExceptionHandler.ThrowConsoleArgumentException();
            }

            for (int i = 0; i < args.Count; i++)
            {
                Parser.ParseCommand(args[i], GetFilesUntilNextArgument(ref i, args));
            }
        }

        public static IEnumerable<string> GetFilesUntilNextArgument(ref int i, IReadOnlyList<string> args)
        {
            List<string> assemblyList = new List<string>();
            i++;
            while (i < args.Count && (!args[i].StartsWith(CommandPrefix) || args[i].StartsWith(SubCommandPrefix)))
            {
                assemblyList.Add(args[i]);
                i++;
            }

            i--;
            return assemblyList;
        }
    }
}