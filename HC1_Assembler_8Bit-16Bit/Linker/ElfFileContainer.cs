using System.Collections.Generic;

namespace HC1_Assembler_8Bit_16Bit.Linker
{
    public class ElfFileContainer
    {
        public string File { get; }
        public List<ElfSectionPair> Sections { get; }
        public ElfHeaderLinkageInfo HeaderLinkageInfo { get; set; }

        public ElfFileContainer(string file)
        {
            File = file;
            Sections = new List<ElfSectionPair>();
        }
    }
}