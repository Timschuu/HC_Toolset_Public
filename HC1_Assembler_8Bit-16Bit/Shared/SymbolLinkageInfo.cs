namespace HC1_Assembler_8Bit_16Bit.Shared
{
    public class SymbolLinkageInfo
    {
        public Symbol Symbol { get; }
        public int ParameterIndex { get; }
        public int Offset { get; }

        public SymbolLinkageInfo(Symbol symbol, int parameterIndex, int offset)
        {
            Symbol = symbol;
            ParameterIndex = parameterIndex;
            Offset = offset;
        }
    }
}