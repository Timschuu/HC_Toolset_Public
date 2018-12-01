using HC1_Assembler_8Bit_16Bit.Linker;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class LinkerTests
    {
        [Test]
        public void TestGetInstructionOpCode()
        {
            int instruction = 0b11000000;
            int opCode = LinkerHelper.GetInstructionOpCode(instruction, 8);
            Assert.That(opCode == 6);
        }
    }
}