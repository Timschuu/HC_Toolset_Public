using System;
using System.Text.RegularExpressions;
using HC1_Assembler_8Bit_16Bit.SystemHalf;

namespace HC1_Assembler_8Bit_16Bit.Shared 
{
    public class Symbol 
    {
        public int Address { get; set; }
        public string Label { get; set; }
        public int Line { get; }
        public bool IsData { get; }
        public int Data { get; }
        public int Index { get; }
        public bool DefinedHere { get; }
        public Half HalfData { get; set; }
        public Type Type { get; set; }
        
        public Symbol(int adr, string lab, int line, bool isData, int data, int index, bool defindedHere, Type type) 
        {
            Address = adr;
            Label = lab;
            Line = line;
            IsData = isData;
            Data = data;
            Index = index;
            DefinedHere = defindedHere;
            Type = type;
        }

        public Symbol(int address, string label, int line, bool isData, Half data, int index, bool definedHere, Type type)
        {
            Address = address;
            Label = label;
            Line = line;
            IsData = isData;
            HalfData = data;
            Index = index;
            DefinedHere = definedHere;
            Type = type;
        }

        public string GetDataString()
        {
            if (Type == typeof(int))
            {
                return Data.ToString();
            }

            if (Type == typeof(Half))
            {
                return HalfData.ToString();
            }

            throw new InvalidOperationException();
        }

        public static bool CheckSymbolName(string name)
        {
            if (name.Length > 0 && char.IsDigit(name[0]))
            {
                return false;
            }

            if (!Regex.IsMatch(name, "^[a-zA-Z0-9_]*$"))
            {
                return false;
            }

            return true;
        }
    }
}
