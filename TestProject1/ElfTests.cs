using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HC1_Assembler_8Bit_16Bit.Assembler;
using HC1_Assembler_8Bit_16Bit.Elf;
using HC1_Assembler_8Bit_16Bit.Elf.Sections;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Linker;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class ElfTests
    {
        [Test]
        public void TestAddFourBytes()
        {
            List<byte> bytelist = new List<byte>();
            bytelist.AddFourBytes(1);
            Assert.That(bytelist[0] == 0x01);
        }

        [Test]
        public void TestFindStringIndex()
        {
            SectionHeaderStringTable suf = new SectionHeaderStringTable();
            int i = suf.FindNameIndex(".strtab");
            Assert.That(i == 10);
        }

        private static IEnumerable<Type> GetElfSections()
        {
            return Assembly.GetAssembly(typeof(Assembler)).GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IElfSection)));
        }

        [Test]
        public void AssureCorrectConstructorUsage()
        {
            IEnumerable<Type> elfSections = GetElfSections();
            IEnumerable<Type> invalid = elfSections.Where(t =>
                t.GetConstructors().FirstOrDefault(c => c.GetCustomAttribute(typeof(ElfSectionConstructorAttribute)) != null) == null);
            Assert.That(!invalid.Any());
        }

        [Test]
        public void AssureCorrectConstructorSignature()
        {
            IEnumerable<Type> elfSections = GetElfSections();
            IEnumerable<Type> invalid = elfSections
                .Where(t => t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,
                                new[] {typeof(ElfFileContainer), typeof(List<byte>), typeof(SectionHeader)},
                                null) == null);
            Assert.That(!invalid.Any());
        }
    }
}