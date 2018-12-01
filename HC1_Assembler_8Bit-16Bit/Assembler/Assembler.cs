using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.Elf;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Operations;
using HC1_Assembler_8Bit_16Bit.Shared;
using HC1_Assembler_8Bit_16Bit.SystemHalf;

namespace HC1_Assembler_8Bit_16Bit.Assembler
{
    public static class Assembler
    {   
        /// <summary>
        /// Reads and parses the given assembly program to generate a class containing all relevant information.
        /// </summary>
        /// <param name="path"> The path to the assembly program. </param>
        /// <returns> An assemblyData class that contains all information found in the file. </returns>
        private static AssemblyData GenerateAssemblyData(string path)
        {
            List<string> input = new List<string>();
            try
            {
                input = File.ReadAllLines(path).ToList();
            }
            catch (Exception e)
            {
                ExceptionHandler.ThrowFileReadingException(Sender.Assembler, path, e);
            }

            try
            {
                IEnumerable<IEnumerable<string>> parsedText = TextParser.SplitText(input, new List<char> {' ', '\t'});
                List<IAssemblyLine> prog = parsedText.Zip(Enumerable.Range(0, input.Count), (s, i) => (IAssemblyLine) new AssemblyLine(s.ToList(), i + 1))
                    .ToList();

                AssemblerDirectives assemblerDirectives = AssemblerHelper.GetAssemblerDirectives(ref prog, path);
                if (assemblerDirectives.InstructionSize == 0)
                {
                    WarningHandler.NoInstructionsizeDirective(Sender.Assembler, path);
                    assemblerDirectives.InstructionSize = 16;
                }

                List<IOperation> operations = OperationRegister.GetOperations(assemblerDirectives.InstructionSize);
                if (operations == null)
                {
                    ExceptionHandler.ThrowInstructionSizeException(Sender.Assembler, assemblerDirectives.InstructionSize, path);
                }

                prog = RemoveEmptyLines(prog).ToList();
                prog = RemoveComments(prog).ToList();
                prog = AdjustLabels(prog, path);
                (List<Symbol> symboltable, int symbolindex) = GetSymbols(ref prog, path);
                prog = RemoveEmptyLines(prog).ToList();
                
                AssemblyData assemblyData = new AssemblyData(assemblerDirectives, path, prog, symboltable, operations, symbolindex);

                return assemblyData;
            }
            catch (Exception e)
            {
                ExceptionHandler.ThrowUnknownException(Sender.Assembler, e);
                return null;
            }
        }
        
        /// <summary>
        /// Generates a unicode file from any string source.
        /// </summary>
        /// <param name="path"> The path of the file that is supposed to be generated. </param>
        /// <param name="source"> Any string containing the content. </param>
        /// <param name="fileextension"> The extension that will be used for the generated file. </param>
        public static void GenerateFilesetOutput(string path, string source, string fileextension)
        {
            string filename = GetFilenameFromPath(path, fileextension);
            try
            {
                File.WriteAllText(filename, source);
                Console.WriteLine(filename + " generated");
            }
            catch (Exception e)
            {
                ExceptionHandler.ThrowFileGenerationException(Sender.Assembler, filename, e);
            }
        }

        /// <summary>
        /// Turns a path and an extension into a filename with the given extension
        /// </summary>
        /// <param name="path"> The path to the file </param>
        /// <param name="fileextension"> Any fileextension </param>
        /// <returns> The filename </returns>
        private static string GetFilenameFromPath(string path, string fileextension)
        {
            if (path.StartsWith("./") || path.StartsWith(".\\"))
            {
                path = path.Substring(2);
            }
            string pathBase = path.Split('.')[0];
            return pathBase + "." + fileextension;
        }

        /// <summary>
        /// Generates a binary file from any bytearray.
        /// </summary>
        /// <param name="path"> The path of the file that is supposed to be generated. </param>
        /// <param name="source"> Any bytearray containing the content. </param>
        /// <param name="fileextension"> The extension that will be used for the generated file. </param>
        public static void GenerateFilesetOutput(string path, byte[] source, string fileextension)
        {
            string filename = GetFilenameFromPath(path, fileextension);
            try
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    binaryWriter.Write(source);
                }

                Console.WriteLine(filename + " generated");
            }
            catch (Exception e)
            {
                ExceptionHandler.ThrowFileGenerationException(Sender.Assembler, filename, e);
            }
        }

        /// <summary>
        /// Removes all comments from a program
        /// </summary>
        /// <param name="prog"> A assembly program </param>
        /// <returns> The program without any comments </returns>
        private static IEnumerable<IAssemblyLine> RemoveComments(IReadOnlyCollection<IAssemblyLine> prog)
        {
            foreach (IAssemblyLine assemblyLine in prog)
            {
                for (int i = 0; i < assemblyLine.Content.Count; i++)
                {
                    if (!assemblyLine.Content.ElementAt(i).Contains(";")) continue;
                    
                    List<string> output = new List<string>();
                    if (i == 0)
                    {
                        assemblyLine.Content = output;
                        break;
                    }
                    
                    output.AddRange(assemblyLine.Content.GetRange(0, i));
                    string lastcode = assemblyLine.Content.ElementAt(i).Split(';')[0];
                    if (!string.IsNullOrWhiteSpace(lastcode))
                    {
                        output.Add(lastcode);
                    }

                    assemblyLine.Content = output;
                }
            }

            return prog;
        }

        /// <summary>
        /// Remove all empty lines from a program.
        /// </summary>
        /// <param name="prog"> The current program. </param>
        /// <returns> The program after all empty lines have been removed. </returns>
        private static IEnumerable<IAssemblyLine> RemoveEmptyLines(IEnumerable<IAssemblyLine> prog)
        {
            return prog.Where(l => l.Content.Any()).ToList();
        }

        /// <summary>
        /// Adjusts all labels to be in the same line as the instruction.
        /// </summary>
        /// <param name="prog"> The current program. </param>
        /// <param name="file"> The current filepath. </param>
        /// <returns> The program after its labels have been adjusted. </returns>
        private static List<IAssemblyLine> AdjustLabels(IReadOnlyList<IAssemblyLine> prog, string file)
        {
            List<IAssemblyLine> adjustedProg = new List<IAssemblyLine>();
            for (int i = 0; i < prog.Count; i++)
            {
                if (!prog[i].Content.Any())
                {
                    continue;
                }
                
                if (!prog[i].Content.First().EndsWith(":"))
                {
                    adjustedProg.Add(new AssemblyLine(prog[i].Content, prog[i].Line));
                    continue;
                }
                
                //Line contains label
                if (prog[i].Content.Count < 2)
                {
                    if (i + 1 < prog.Count && prog[i + 1].Content.Any() && !prog[i + 1].Content.First().EndsWith(":"))
                    {
                        string lab = prog[i].Content.First();
                        List<string> nextline = new List<string> {lab};
                        nextline.AddRange(prog[++i].Content);
                        adjustedProg.Add(new AssemblyLine(nextline, prog[i].Line));
                    }
                    else
                    {
                        ExceptionHandler.ThrowEmptyLabelException(prog[i].Line + 1, file);
                    }
                }
                else
                {
                    adjustedProg.Add(prog[i]);
                }
            }

            return adjustedProg;
        }

        /// <summary>
        /// Finds all symbols and removes them from the source. Source gets trimmed to the start of the mnemonics.
        /// </summary>
        /// <param name="prog"> List of strings containing an assenmby program. </param>
        /// <param name="file"> The current file path </param>
        /// <returns> A List of symbols that have been found in the program. </returns>
        private static (List<Symbol> symboltable, int symbolindex) GetSymbols(ref List<IAssemblyLine> prog, string file)
        {
            int labelIndex = 0;
            int index = 0;
            List<Symbol> symboltable = new List<Symbol>();
            foreach (IAssemblyLine line in prog)
            {
                if (line.Content.Count < 2 || !line.Content.First().EndsWith(":"))
                {
                    index++;
                    continue;
                }
                
                //Line contains label
                string lab = line.Content.First().Substring(0, line.Content.First().Length - 1);
                string content = line.Content.ElementAt(1);
                bool isData = int.TryParse(content, out int data);
                bool isFloat = Half.TryParse(content, out _);
                if (symboltable.FirstOrDefault(sym => sym.Label == lab) != null)
                {
                    ExceptionHandler.ThrowMultipleLabelDeclarationException(lab, file);
                }

                if (!Symbol.CheckSymbolName(lab))
                {
                    ExceptionHandler.ThrowInvalidSymbolNameException(lab, line.Line, file);
                }

                if (isData)
                {
                    symboltable.Add(new Symbol(index, lab, line.Line, true, data, labelIndex, true, typeof(int))); //in Symboltable hinzufuegen
                }
                else if (isFloat)
                {
                    Half half = Half.Parse(content);
                    symboltable.Add(new Symbol(index, lab, line.Line, true, half, labelIndex, true, typeof(Half)));
                }
                else
                {
                    symboltable.Add(new Symbol(index, lab, line.Line, false, data, labelIndex, true, null));
                }
                labelIndex++;
                if (isData || isFloat)
                {
                    line.Content = new List<string>();
                }
                else
                {
                    line.Content.RemoveAt(0);
                }

                index++;
            }

            return (symboltable, labelIndex);
        }

        /// <summary>
        /// Converts a symboltabel to a unicode representation.
        /// </summary>
        /// <param name="file"> The file containing the assemblydata. </param>
        /// <returns> A string containing the unicode representation of the symboltabel. </returns>
        public static string AssembleToSymboltable(string file)
        {
            return GenerateAssemblyData(file).Symboltable.Aggregate("", (current, s) => current + s.Label + " -> " + s.Address + " ");
        }

        /// <summary>
        /// Converts an assembled program to its unicode representation.
        /// </summary>
        /// <param name="file"> The file containing the assemblydata. </param>
        /// <returns> A string containing the unicode representation of the assembly. </returns>
        public static string AssembleToString(string file)
        {
            UnicodeAssembly unicodeAssembly = AssembleToUnicode(file);
            return unicodeAssembly.Assembly.Aggregate("", (current, line) => current + Convert.ToString(line, 2).PadLeft(unicodeAssembly.InstructionSize, '0') + "\n");
        }

        public static byte[] AssembleToByteArray(string file)
        {
            AssemblyData assemblyData = GenerateAssemblyData(file);          
            return new ElfFile().GenerateSections(assemblyData).ToArray();
        }

        /// <summary>
        /// Assembles a file containing assembly into VHDL representations of its instructions.
        /// </summary>
        /// <param name="file"> The assembly file. </param>
        /// <returns> One string containing the entire program as VHDL representation. </returns>
        public static string AssembleToVhdl(string file)
        {
            UnicodeAssembly unicodeAssembly = AssembleToUnicode(file);
            string ret = "";
            int i = 0;
            foreach (uint line in unicodeAssembly.Assembly)
            {
                ret += "ram(" + i + ") <= \"" + Convert.ToString(line, 2).PadLeft(unicodeAssembly.InstructionSize, '0') + "\";\n";
                i++;
            }

            return ret;
        }

        /// <summary>
        /// Assembles a file containing assembly into Unicode representations of its instructions.
        /// </summary>
        /// <param name="file"> The assembly file. </param>
        /// <returns> A collection of instructions and the found instructionsize. </returns>
        private static UnicodeAssembly AssembleToUnicode(string file)
        {
            int textoffset = 0;
            AssemblyData assemblyData = GenerateAssemblyData(file);
            List<uint> assembld = new List<uint>();
            foreach (var line in assemblyData.Program)
            {
                string mnem = line.Content.First();
                List<string> splitparas = line.Content.GetRange(1, line.Content.Count - 2);
                List<RawParameter> rawParameters = splitparas.Select(s => new RawParameter(s, true, null)).ToList();

                rawParameters = VerifyAndReplaceOperationParameters(rawParameters, assemblyData, line, textoffset);

                long? cInstr = assemblyData.Operations.FirstOrDefault(o => o.Mnemonic == mnem)
                    ?.ToAssembeldInstruction(assemblyData.Symboltable, rawParameters, line.Line, assemblyData.Path);

                if (cInstr == null)
                {
                    ExceptionHandler.ThrowMnemonicException(mnem, line.Line, assemblyData.Path);
                }
                else
                {
                    assembld.Add((uint) cInstr);
                }

                textoffset++;
            }

            return new UnicodeAssembly(assembld, assemblyData.AssemblerDirectives.InstructionSize);
        }

        /// <summary>
        /// Turns the assembly program into binary instructions.
        /// </summary>
        /// <param name="assemblyData"> The current assemblydata. </param>
        /// <returns> A collection of bytes that represent the program. </returns>
        public static IEnumerable<byte> AssembleToBinary(AssemblyData assemblyData)
        {
            List<byte> assembld = new List<byte>();
            int textoffset = 0;
            foreach (var line in assemblyData.Program)
            {
                string mnem = line.Content.First();
                List<string> splitparas = line.Content.GetRange(1, line.Content.Count - 1);
                List<RawParameter> rawParameters = splitparas.Select(s => new RawParameter(s, true, null)).ToList();

                rawParameters = VerifyAndReplaceOperationParameters(rawParameters, assemblyData, line, textoffset);
                
                List<byte> cInstr = assemblyData.Operations.FirstOrDefault(o => o.Mnemonic == mnem)
                    ?.ToBinaryInstruction(assemblyData.Symboltable, rawParameters, line.Line, assemblyData.Path);

                if (cInstr == null)
                {
                    ExceptionHandler.ThrowMnemonicException(mnem, line.Line, assemblyData.Path);
                }
                else
                {
                    assembld.AddRange(cInstr);
                }

                textoffset++;
            }

            return assembld;
        }

        /// <summary>
        /// Tries to parse all parameters of an instruction.
        /// </summary>
        /// <param name="splitparas"> The parameters. </param>
        /// <param name="assemblyData"> The current assemblydata. </param>
        /// <param name="line"> The current line of assemblycode. </param>
        /// <param name="textoffset"> The current textoffset. </param>
        /// <returns> A List of parameters that are parseable and translated to integers or floats. </returns>
        private static List<RawParameter> VerifyAndReplaceOperationParameters(List<RawParameter> splitparas, AssemblyData assemblyData, IAssemblyLine line, int textoffset)
        {
            for (int i = 0; i < splitparas.Count; i++)
            {
                if (splitparas[i].Content != "")
                {
                    splitparas[i].Resolvable = true;

                    if (TryParseHexNumber(splitparas[i], out int hexnum, line.Line, assemblyData.Path))
                    {
                        splitparas[i].Content = hexnum.ToString();
                    }
                    if (int.TryParse(splitparas[i].Content, out _))
                    {
                        splitparas[i].Type = typeof(int);
                        continue;
                    }
                    if (splitparas[i].Content.Contains('.') && Half.TryParse(splitparas[i].Content, out _))
                    {
                        splitparas[i].Type = typeof(Half);
                        continue;
                    }
                    if (!Enum.TryParse(splitparas[i].Content.ToUpper(), out Register registerval))
                    {
                        Symbol symbol = assemblyData.Symboltable.FirstOrDefault(sym => sym.IsData && sym.Label == splitparas[i].Content);
                        if (symbol != null && symbol.IsData)
                        {
                            splitparas[i].Content = symbol.GetDataString();
                            splitparas[i] = ParseParameterType(splitparas[i]);
                        }
                        else
                        {
                            if (!Symbol.CheckSymbolName(splitparas[i].Content))
                            {
                                ExceptionHandler.ThrowInvalidSymbolNameException(splitparas[i].Content, line.Line, assemblyData.Path);
                            }
                            Symbol sym = new Symbol(0, splitparas[i].Content, line.Line, false, 0, assemblyData.SymboltableIndexCounter++, false, null);
                            assemblyData.Symboltable.Add(sym);
                            assemblyData.SymbolLinkageInfos.Add(new SymbolLinkageInfo(sym, i, textoffset));
                            splitparas[i].Resolvable = false;
                        }
                    }
                    else
                    {
                        splitparas[i].Content = ((int) registerval).ToString();
                        splitparas[i].Type = typeof(int);
                    }
                }
                else
                {
                    splitparas.RemoveAt(i);
                }
            }

            return splitparas;
        }

        /// <summary>
        /// Tries to convert a raw parameter to a hex number.
        /// </summary>
        /// <param name="para"> A raw parameter. </param>
        /// <param name="number"> On succes, the translated number. Otherwise 0. </param>
        /// <param name="line"> The current line of assembly code. </param>
        /// <param name="path"> The current file. </param>
        /// <returns> True, if parameter is a hex number and can be converted, otherwise false. </returns>
        private static bool TryParseHexNumber(RawParameter para, out int number, int line, string path)
        {
            number = 0;
            if (!para.Content.StartsWith("0x")) return false;
            para.Content = para.Content.Substring(2);
            try
            {
                number = Convert.ToInt32(para.Content, 16);
                return true;
            }
            catch (Exception)
            {
                ExceptionHandler.ThrowInvalidHexNumberException(line, path);
            }

            return false;
        }
        
        /// <summary>
        /// Tries to find the type of a given parameter.
        /// </summary>
        /// <param name="para"> The parameter. </param>
        /// <returns> The parameter with its type value changed to the corresponding type if that could be found. </returns>
        private static RawParameter ParseParameterType(RawParameter para)
        {
            if (int.TryParse(para.Content, out _))
            {
                para.Type = typeof(int);
                return para;
            }
            if (para.Content.Contains('.') && Half.TryParse(para.Content, out _))
            {
                para.Type = typeof(Half);
                return para;
            }

            return para;
        }
    }
}