using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Shared;
using HC1_Assembler_8Bit_16Bit.SystemHalf;
using ParameterInfo = HC1_Assembler_8Bit_16Bit.Shared.ParameterInfo;

namespace HC1_Assembler_8Bit_16Bit.Assembler
{
    public static class AssemblerHelper
    {      
        /// <summary>
        /// Checks all given parameters about their valid range and throws an exception if any parameter is out of range.
        /// </summary>
        /// <param name="paras"> The parameters. </param>
        /// <param name="line"> The current line in the assembly source file. </param>
        /// <param name="file"> The current assembly source file. </param>
        /// <param name="parainfo"> A list of binary parameter representations. </param>
        /// <returns></returns>
        public static List<BinaryParameter> CheckParameters(List<RawParameter> paras, int line, string file, List<ParameterInfo> parainfo)
        {
            List<BinaryParameter> checkedParameters = new List<BinaryParameter>();
            
            for(int i = 0; i < paras.Count; i++)
            {
                if (paras[i].Resolvable)
                {
                    if (paras[i].Type == typeof(int))
                    {
                        int p = int.Parse(paras[i].Content);

                        if (p > parainfo[i].Maxvalue || p < parainfo[i].Minvalue)
                        {
                            ExceptionHandler.ThrowParameterOutOfRangeException(line, file, p, parainfo[i].Minvalue, parainfo[i].Maxvalue);
                        }
                
                        checkedParameters.Add(new BinaryParameter(paras[i].Type, BitConverter.GetBytes(p).ToList()));
                    }
                    else if (paras[i].Type == typeof(Half))
                    {
                        Half h = Half.Parse(paras[i].Content);
                        
                        if (parainfo[i].HalfParameterInfo == null)
                        {
                            ExceptionHandler.ThrowInvalidFloatParameterException(line, file);
                            return null;
                        }

                        if (h > parainfo[i].HalfParameterInfo.Maxvalue || h < parainfo[i].HalfParameterInfo.Minvalue)
                        {
                            ExceptionHandler.ThrowParameterOutOfRangeException(line, file, parainfo[i].HalfParameterInfo.Minvalue, parainfo[i].HalfParameterInfo.Maxvalue);
                        }

                        List<byte> halfvalue = Half.GetBytes(h).ToList();
                        halfvalue.AddRange(new List<byte>{0,0});
                        checkedParameters.Add(new BinaryParameter(paras[i].Type, halfvalue));
                    }
                }
                else
                {
                    checkedParameters.Add(new BinaryParameter(typeof(int), new List<byte>{ 0, 0, 0, 0 }));
                }
            }

            return checkedParameters;
        }

        public static int CalculateSymbolTableOffset(int index)
        {
            return index * 18;
        }
        
        /// <summary>
        /// Tries to find all assembler directives and removes them from the source.
        /// </summary>
        /// <param name="prog"> List of strings containing the program. </param>
        /// <param name="file"> The filename in case an error occurs. </param>
        /// <returns> AssemblerDirectives object containing all found directives. </returns>
        public static AssemblerDirectives GetAssemblerDirectives(ref List<IAssemblyLine> prog, string file)
        {
            AssemblerDirectives assemblerDirectives = new AssemblerDirectives();

            foreach (IAssemblyLine line in prog)
            {
                IEnumerable<string> splitline = line.Content.ToList();
                if (!splitline.Any() || !splitline.First().StartsWith("#")) continue;
                
                if (splitline.Count() < 2)
                {
                    ExceptionHandler.ThrowAssemblerDirectiveException(splitline.First(), line.Line, file);
                    return assemblerDirectives;
                }
                    
                string directive = splitline.First().Substring(1).ToLower();
                string para = splitline.ElementAt(1).RemoveComment();

                PropertyInfo propertyInfo = typeof(AssemblerDirectives).GetProperties().FirstOrDefault(p => p.Name.ToLower() == directive);
                if (propertyInfo == null)
                {
                    ExceptionHandler.ThrowAssemblerDirectiveException(splitline.First() + " " + splitline.ElementAt(1), line.Line, file);
                    return assemblerDirectives;
                }
                    
                try
                {
                    var convertedpara = Convert.ChangeType(para, propertyInfo.PropertyType);
                    propertyInfo.SetValue(assemblerDirectives, convertedpara);            
                }
                catch (Exception)
                {
                    ExceptionHandler.ThrowAssemblerDirectiveException(splitline.First() + " " + splitline.ElementAt(1), line.Line, file);
                }

                line.Content = new List<string>();
            }
            return assemblerDirectives;
        }
    }
}