using System.Collections.Generic;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Shared;

namespace HC1_Assembler_8Bit_16Bit.Assembler
{
    public class AssemblyData
    {
        public AssemblerDirectives AssemblerDirectives { get; set; }
        public string Path { get; set; }
        public List<IAssemblyLine> Program { get; set; }
        public List<Symbol> Symboltable { get; set; }
        public List<IOperation> Operations { get; set; }
        public List<SymbolLinkageInfo> SymbolLinkageInfos { get; set; }
        public int SymboltableIndexCounter { get; set; }

        public AssemblyData(AssemblerDirectives assemblerDirectives, string path, List<IAssemblyLine> program,
            List<Symbol> symboltable, List<IOperation> operations, int symbolindex)
        {
            SymbolLinkageInfos = new List<SymbolLinkageInfo>();
            
            AssemblerDirectives = assemblerDirectives;
            Path = path;
            Program = program;
            Symboltable = symboltable;
            Operations = operations;
            SymboltableIndexCounter = symbolindex;
        }
    }
}