namespace HC1_Assembler_8Bit_16Bit.Linker
{
    public class RecreatedSymbolLinkageInfo
    {
        public int Offset { get; }
        public int Index { get; }
        public int ParameterNumber { get; }

        public RecreatedSymbolLinkageInfo(int offset, int index, int parameterNumber)
        {
            Offset = offset;
            Index = index;
            ParameterNumber = parameterNumber;
        }
    }
}