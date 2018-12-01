using System;
using HC1_Assembler_8Bit_16Bit.Handler;

namespace HC1_Assembler_8Bit_16Bit.Helpers
{
    public static class Contract
    {
        public static void AssertNotNull(object item, string name)
        {
            if (item != null) return;
            NullReferenceException e = new NullReferenceException($"Variable {name} was null.");
            ExceptionHandler.ThrowInternalException(e);
            throw e;
        }
    }
}