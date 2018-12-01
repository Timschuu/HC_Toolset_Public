namespace HC1_Assembler_8Bit_16Bit.Interfaces
{
    public interface ILinker
    {
        /// <summary>
        /// Links all files that have been added to the linker.
        /// </summary>
        void Link();
        
        /// <summary>
        /// Generates a text file with the given path and writes the content of all text sections into it.
        /// </summary>
        /// <param name="path"> Path of the file. </param>
        void GenerateOutput(string path);
        
        /// <summary>
        /// Generates a binary file with the given path and writes the content of all text sections into it.
        /// </summary>
        /// <param name="path"> Path of the file. </param>
        void GenerateDebugOutput(string path);
        
        /// <summary>
        /// Tries to read an ELF file, parses it and adds it to the linking list.
        /// </summary>
        /// <param name="file"> Path of the file. </param>
        void AddElfFile(string file);
    }
}        