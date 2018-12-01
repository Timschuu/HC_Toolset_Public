using HC1_Assembler_8Bit_16Bit.SystemHalf;

namespace HC1_Assembler_8Bit_16Bit.Shared
{
    public class HalfParameterInfo
    {
        public Half Minvalue { get; }
        public Half Maxvalue { get; }

        public HalfParameterInfo(Half minvalue, Half maxvalue)
        {
            Minvalue = minvalue;
            Maxvalue = maxvalue;
        }
    }
}