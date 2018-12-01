using System;
using System.Collections.Generic;
using System.Linq;
using HC1_Assembler_8Bit_16Bit.SystemHalf;

namespace HC1_Assembler_8Bit_16Bit.Helpers.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Gets the bytes of the given integer and adds them at the end of the list.
        /// </summary>
        /// <param name="bytelist"> A list of bytes. </param>
        /// <param name="number"> Any integer. </param>
        public static void AddFourBytes(this List<byte> bytelist, int number)
        {
            bytelist.AddRange(BitConverter.GetBytes(number));         
        }

        /// <summary>
        /// Gets the bytes of the given half and adds them at the end of the list.
        /// </summary>
        /// <param name="bytelist"> A list of bytes. </param>
        /// <param name="number"> Any half. </param>
        public static void AddFourBytes(this List<byte> bytelist, Half number)
        {
            List<byte> halfvalue = Half.GetBytes(number).ToList();
            halfvalue.AddRange(new List<byte>{0,0});
            bytelist.AddRange(halfvalue);
        }

        /// <summary>
        /// Returns the integer that gets converted from 4 bytes starting at the given offset.
        /// </summary>
        /// <param name="bytelist"> A list of bytes. </param>
        /// <param name="offset"> Offset where to start reading. </param>
        /// <returns> The converted integer. </returns>
        public static int ReadFourBytes(this List<byte> bytelist, int offset)
        {
            return BitConverter.ToInt32(bytelist.ToArray(), offset);
        }
        
        /// <summary>
        /// Returns the integer that gets converted from 2 bytes starting at the given offset.
        /// </summary>
        /// <param name="bytelist"> A list of bytes. </param>
        /// <param name="offset"> Offset where to start reading. </param>
        /// <returns> The converted integer. </returns>
        public static int ReadTwoBytes(this List<byte> bytelist, int offset)
        {
            return BitConverter.ToInt16(bytelist.ToArray(), offset);
        }
        
        public static int ReadTwoBytesReversed(this List<byte> bytelist, int offset)
        {
            return BitConverter.ToInt16(bytelist.Skip(offset).Take(2).Reverse().ToArray(), 0);
        }
    }
}