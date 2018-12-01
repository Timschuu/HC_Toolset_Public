using System;

namespace ConsoleArgumentParser
{
    public class CommandArgumentAttribute : Attribute
    {
        public string Name { get; set; }

        public CommandArgumentAttribute(string name)
        {
            Name = name;
        }
    }
}