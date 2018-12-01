using HC1_Assembler_8Bit_16Bit.Linker;

namespace HC1_Assembler_8Bit_16Bit.Interfaces
{
    public interface IMappingRequester
    {
        bool NeedsMapping { get; }
        void Map(ElfFileContainer elfFileContainer);
    }
}