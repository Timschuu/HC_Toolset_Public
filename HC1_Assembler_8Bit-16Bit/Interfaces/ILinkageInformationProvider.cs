namespace HC1_Assembler_8Bit_16Bit.Interfaces
{
    public interface ILinkageInformationProvider
    {
        int AdjustSymbol(IOperation operation, int currentaddress, int symbolvalue);
    }
}