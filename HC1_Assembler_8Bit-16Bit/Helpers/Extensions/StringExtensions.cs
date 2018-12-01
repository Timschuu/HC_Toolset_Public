using System.IO;

namespace HC1_Assembler_8Bit_16Bit.Helpers.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Changes the path of a file to just its name.
        /// </summary>
        /// <param name="path"> The path of the file. </param>
        /// <returns> Its name. </returns>
        public static string PathToFileName(this string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// From an string removes anything coming behind a ";" character.
        /// </summary>
        /// <param name="s"> A string. </param>
        /// <returns> The string from which everything after a ";" has been removed. </returns>
        public static string RemoveComment(this string s)
        {
            return s.Split(';')[0];
        }
    }
}