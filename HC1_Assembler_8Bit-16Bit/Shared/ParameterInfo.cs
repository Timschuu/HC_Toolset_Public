namespace HC1_Assembler_8Bit_16Bit.Shared
{
    public class ParameterInfo
    {
        public int Minvalue { get; }
        public int Maxvalue { get; }
        public int Bitmask { get; }
        public int BitOffset { get; }
        public HalfParameterInfo HalfParameterInfo { get; set; }

        public ParameterInfo(int minvalue, int maxvalue, int bitmask, int bitOffset, HalfParameterInfo halfParameterInfo)
        {
            Minvalue = minvalue;
            Maxvalue = maxvalue;
            Bitmask = bitmask;
            BitOffset = bitOffset;
            HalfParameterInfo = halfParameterInfo;
        }
    }
}