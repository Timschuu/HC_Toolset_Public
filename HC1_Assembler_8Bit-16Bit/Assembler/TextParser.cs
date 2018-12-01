using System.Collections.Generic;
using System.Linq;

namespace HC1_Assembler_8Bit_16Bit.Assembler
{
    public static class TextParser
    {
        public static IEnumerable<IEnumerable<string>> SplitText(IEnumerable<string> text, List<char> splitter)
        {
            List<List<string>> lines = text.Select(line => line.Split(splitter.ToArray()).ToList()).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            }

            return lines;
        }
    }
}