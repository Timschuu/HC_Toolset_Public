using System;

namespace ConsoleArgumentParser
{
    public class CommandType
    {
        public Type Command { get; set; }
        public string Helptext { get; set; }

        public CommandType(Type command, string helptext)
        {
            Command = command;
            Helptext = helptext;
        }
    }
}