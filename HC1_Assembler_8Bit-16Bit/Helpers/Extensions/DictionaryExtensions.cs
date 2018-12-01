using System.Collections.Generic;

namespace HC1_Assembler_8Bit_16Bit.Helpers.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryTake<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, out TValue value)
        {
            value = default;
            if (!dic.ContainsKey(key)) return false;
            value = dic[key];
            return true;
        }
    }
}