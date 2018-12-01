using HC1_Assembler_8Bit_16Bit.Shared;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class SymbolTests
    {
        [TestCase("1Test")]
        [TestCase("Test&")]
        [TestCase("Test12_*")]
        [TestCase("*Test")]
        [TestCase("*****")]
        public void TestInvalidSymbolNames(string name)
        {
            Assert.That(!Symbol.CheckSymbolName(name));
        }

        [TestCase("Test123")]
        [TestCase("_Test")]
        [TestCase("Test_123")]
        [TestCase("Test_Test")]
        [TestCase("A1B2C3")]
        public void TestValidSymbolNames(string name)
        {
            Assert.That(Symbol.CheckSymbolName(name));
        }
    }
}