using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HC1_Assembler_8Bit_16Bit.Elf;
using HC1_Assembler_8Bit_16Bit.Elf.Sections;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Shared;
using ParameterInfo = HC1_Assembler_8Bit_16Bit.Shared.ParameterInfo;

namespace HC1_Assembler_8Bit_16Bit.Linker
{
    public class Linker : ILinker
    {
        private readonly List<ElfFileContainer> _elfFiles;
        private int _currentAddressOffset;
        private List<Symbol> _globalSymbolTable;

        public Linker()
        {
            _elfFiles = new List<ElfFileContainer>();
        }
        
        /// <summary>
        /// Tries to open an ELF file and throws an FileTypeException if it fails.
        /// </summary>
        /// <param name="file"> Path of the file. </param>
        /// <returns></returns>
        private static List<byte> TryOpenElfFile(string file)
        {
            List<byte> rawBytes = ElfLoader.LoadBinaryFile(file);
            if (!ElfLoader.VerifyElfFile(rawBytes))
            {
                ExceptionHandler.ThrowFileTypeException(Sender.Linker, file);
            }

            return rawBytes;
        }

        public void AddElfFile(string file)
        {
            List<byte> binaries = TryOpenElfFile(file);

            ElfFileContainer elfFileContainer = new ElfFileContainer(file) {HeaderLinkageInfo = ElfLoader.ParseElfHeader(binaries, file)};

            for (int i = 0; i < elfFileContainer.HeaderLinkageInfo.SectionHeaderCount; i++)
            {
                ElfSectionPair pair = new ElfSectionPair {Header = new SectionHeader(binaries)};
                elfFileContainer.Sections.Add(pair);
            }
            
            //Create SectionHeaderStringTable first
            elfFileContainer.Sections[0].Section = new SectionHeaderStringTable(elfFileContainer, binaries, elfFileContainer.Sections[0].Header);

            for (int i = 1; i < elfFileContainer.Sections.Count; i++)
            {
                string name = ((SectionHeaderStringTable) elfFileContainer.Sections[0].Section).GetNameFromIndex(elfFileContainer.Sections[i].Header
                    .SectionHeaderStringTableIndex);

                Type sectionclass = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(t => t.IsClass && t.Namespace == "HC1_Assembler_8Bit_16Bit.Elf.Sections" 
                                                   && (t.GetCustomAttribute(typeof(ElfSectionAttribute)) as ElfSectionAttribute)?.Name == name);

                if (sectionclass == null)
                {
                    ExceptionHandler.ThrowInvalidFileException(Sender.Linker, file);
                    return;
                }

                ConstructorInfo ci = sectionclass.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(c => c.GetCustomAttributes(typeof(ElfSectionConstructorAttribute), false).Length > 0);
                   
                if (ci == null)
                {
                    ExceptionHandler.ThrowInvalidFileException(Sender.Linker, file);
                    return;
                }

                if (!(ci.Invoke(new object[] {elfFileContainer, binaries, elfFileContainer.Sections[i].Header}) is IElfSection section))
                {
                    ExceptionHandler.ThrowInvalidFileException(Sender.Linker, file);
                    return;
                }
                
                elfFileContainer.Sections[i].Section = section;
            }
            
            LinkerHelper.PostMapSections(elfFileContainer);
            _elfFiles.Add(elfFileContainer);
        }

        public void GenerateOutput(string path)
        {
            if (path.StartsWith("./") || path.StartsWith(".\\"))
            {
                path = path.Substring(2);
            }
            try
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(File.Create(path)))
                {
                    binaryWriter.Close();
                }
                using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.Append)))
                {
                    foreach (ElfFileContainer elfFileContainer in _elfFiles)
                    {
                        TextSection textSection = (TextSection) LinkerHelper.RequestSection(elfFileContainer, TextSection.Name);
                        binaryWriter.Write(textSection.Bytes.ToArray());
                    }
                }

                Console.WriteLine(path + " generated");
            }
            catch (Exception e)
            {
                ExceptionHandler.ThrowFileGenerationException(Sender.Linker, path, e);
            }
        }
        
        public void GenerateDebugOutput(string path)
        {
            if (path.StartsWith("./") || path.StartsWith(".\\"))
            {
                path = path.Substring(2);
            }
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                int linecount = 0;
                foreach (ElfFileContainer elfFileContainer in _elfFiles)
                {
                    SectionHeader sectionHeader = LinkerHelper.RequestSectionHeader(elfFileContainer, TextSection.Name);
                    TextSection textSection = (TextSection) LinkerHelper.RequestSection(elfFileContainer, TextSection.Name);
                    int rangecount = elfFileContainer.HeaderLinkageInfo.InstructionSize / 8;
                    for (int i = 0; i < sectionHeader.Size; i += rangecount)
                    {
                        List<byte> bytes = textSection.Bytes.GetRange(i, rangecount);
                        string buffer = "";
                        foreach (byte b in bytes)
                        {
                            buffer += Convert.ToString(b, 2).PadLeft(8, '0');
                        }
                        using (StreamWriter streamWriter = File.AppendText(path))
                        {
                            streamWriter.WriteLine((++linecount).ToString().PadLeft(4, '0') + "\t" + buffer);
                        }
                    }
                }

                Console.WriteLine(path.PathToFileName() + " generated");
            }
            catch (Exception e)
            {
                ExceptionHandler.ThrowFileGenerationException(Sender.Linker, path.PathToFileName(), e);
            }
        }
        
        public void Link()
        {
            SortElfFiles();
            CheckInstructionSizeIntegrity();
            AdjustAddresses();
            GenerateGlobalSymbolTable();
            RelocateSymbols();
        }

        /// <summary>
        /// Checks if all added ELF files have the same instructionsize. Throws an exception if they dont.
        /// </summary>
        private void CheckInstructionSizeIntegrity()
        {
            List<int> instructionsizes = _elfFiles.Select(e => e.HeaderLinkageInfo.InstructionSize).ToList();
            if (instructionsizes.Distinct().Count() != 1)
            {
                ExceptionHandler.ThrowDifferentInstructionsizeException();
            }
        }
        
        /// <summary>
        /// Adds all label declarations to one global symbol tabel and checks for any duplicates.
        /// </summary>
        private void GenerateGlobalSymbolTable()
        {
            _globalSymbolTable = new List<Symbol>();
            foreach (ElfFileContainer elfFileContainer in _elfFiles)
            {
                SymbolTable symbolTable = (SymbolTable) LinkerHelper.RequestSection(elfFileContainer, SymbolTable.Name);
                List<Symbol> localsymtab = symbolTable.Symtab.Where(s => s.DefinedHere).ToList();
                foreach (Symbol symbol in localsymtab)
                {
                    if (_globalSymbolTable.FirstOrDefault(s => s.Label == symbol.Label) != null)
                    {
                        ExceptionHandler.ThrowMultipleLabelDeclarationException(symbol.Label);
                    }
                }
                
                _globalSymbolTable.AddRange(localsymtab);
            }
        }

        /// <summary>
        /// Sorts the added ELF files so that the one containing the main label is first. If there is no or multiple
        /// main labels it throws an exception.
        /// </summary>
        private void SortElfFiles()
        {
            List<ElfFileContainer> mainfiles = _elfFiles.FindAll(f => (LinkerHelper.RequestSection(f, SymbolTable.Name) as SymbolTable)?.Symtab
                                                                      .FirstOrDefault(s => s.Label == "main" && s.DefinedHere) != null);
            if (mainfiles.Count == 0)
            {
                ExceptionHandler.ThrowMainLabelNotFoundException();
            }
            else if(mainfiles.Count > 1)
            {
                ExceptionHandler.ThrowMultipleMainLabelException();
            }
            int index = _elfFiles.FindIndex(f => f == mainfiles[0]);
            if (index == -1)
            {
                ExceptionHandler.ThrowMainLabelNotFoundException();
            }
            ElfFileContainer tmp = _elfFiles[0];
            _elfFiles[0] = _elfFiles[index];
            _elfFiles[index] = tmp;
        }

        /// <summary>
        /// Adjusts the addresses of all added ELF files.
        /// </summary>
        private void AdjustAddresses()
        {
            for (int i = 1; i < _elfFiles.Count; i++)
            {
                SymbolTable symbolTable = (SymbolTable) LinkerHelper.RequestSection(_elfFiles[i], SymbolTable.Name);
                _currentAddressOffset += LinkerHelper.RequestSectionHeader(_elfFiles[i - 1], TextSection.Name).Size /
                                         (_elfFiles[i -1].HeaderLinkageInfo.InstructionSize / 8);
                foreach (Symbol symbol in symbolTable.Symtab.Where(s => s.DefinedHere))
                {
                    symbol.Address += _currentAddressOffset;
                }
            }
        }

        /// <summary>
        /// Uses the relocation setions of all added ELF files to resolve any symbols.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Corrupted ELF file. </exception>
        private void RelocateSymbols()
        {
            int currentAddressOffset = 0;
            foreach (ElfFileContainer elfFileContainer in _elfFiles)
            {
                int rangecount = elfFileContainer.HeaderLinkageInfo.InstructionSize / 8;
                RelocationText relocationText = (RelocationText) LinkerHelper.RequestSection(elfFileContainer, RelocationText.Name);
                TextSection textSection = (TextSection) LinkerHelper.RequestSection(elfFileContainer, TextSection.Name);
                SymbolTable symbolTable = (SymbolTable) LinkerHelper.RequestSection(elfFileContainer, SymbolTable.Name);
                foreach (RecreatedSymbolLinkageInfo symbol in relocationText.RecreatedSymbolLinkageInfos)
                {
                    List<byte> instructionbytes = textSection.Bytes.GetRange(symbol.Offset * rangecount, rangecount);
                    instructionbytes.Reverse();
                    List<byte> nullbytes = new List<byte> {0, 0, 0, 0};
                    instructionbytes.AddRange(nullbytes.GetRange(0, 2 - instructionbytes.Count));
                    int instruction = BitConverter.ToInt16(instructionbytes.ToArray(), 0);
                    string symbolname = symbolTable.Symtab[symbol.Index / 18].Label;
                    Symbol sym = _globalSymbolTable.FirstOrDefault(s => s.Label == symbolname);
                    if (sym == null)
                    {
                        ExceptionHandler.ThrowLabelNotFoundException(symbolname);
                        continue;
                    }

                    IOperation op = LinkerHelper.GetOperationFromInstruction(instruction, elfFileContainer.HeaderLinkageInfo.InstructionSize);
                    if (op == null)
                    {
                        throw new InvalidOperationException();
                    }
                    ParameterInfo parameterInfo = op.ParameterList[symbol.ParameterNumber];
                    int value = sym.IsData ? sym.Data : sym.Address;
                    if (op is ILinkageInformationProvider linkageInformationProvider)
                    {
                        value = linkageInformationProvider.AdjustSymbol(op, currentAddressOffset + symbol.Offset, value);
                    }
                    if (value > parameterInfo.Maxvalue || value < parameterInfo.Minvalue)
                    {
                        ExceptionHandler.ThrowParameterOutOfRangeException(symbolname, op.Mnemonic, value, parameterInfo.Minvalue, parameterInfo.Maxvalue);
                    }
                    
                    instruction |= (value & parameterInfo.Bitmask) << parameterInfo.BitOffset;
                    
                    textSection.Bytes.RemoveRange(symbol.Offset * rangecount, rangecount);
                    textSection.Bytes.InsertRange(symbol.Offset * rangecount,
                        BitConverter.GetBytes(instruction).Reverse().ToList().GetRange(4 - rangecount, rangecount));
                }

                currentAddressOffset += textSection.Bytes.Count / rangecount;
            }
        }
    }
}