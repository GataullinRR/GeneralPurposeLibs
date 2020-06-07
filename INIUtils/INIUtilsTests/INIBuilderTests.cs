using Microsoft.VisualStudio.TestTools.UnitTesting;
using INIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.IO;

namespace INIUtils.Tests
{
    [TestClass()]
    public class INIBuilderTests
    {
        static readonly string _testsDataPath = 
            Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName() + ".txt");

        [TestMethod()]
        public void BuildAndSaveTest()
        {
            INIWriter builder = new INIWriter();
            builder.AppendKVP("Int32Key", -12);
            builder.AppendKVP("FloatKey", 1.1F);
            builder.AppendKVP("DoubleKey", 1.2);
            builder.AppendKVP("BooleanKey", true);
            builder.AppendKVP("StringKey", "TestString");
            builder.AppendKVP("NullStringKey", null);
            builder.AppendKVP("EmptyStringKey", "");
            builder.AppendKVP("SpaceStringKey", " ");
            builder.AppendKVP("UnsupportedCharsStringTypeKey", "a" + Environment.NewLine + "b");

            bool saveResult = builder.Save(_testsDataPath);
            Assert.IsTrue(saveResult);
            
            string expected =
@"Int32Key = -12
FloatKey = 1.1
DoubleKey = 1.2
BooleanKey = True
StringKey = TestString
NullStringKey = NULL
EmptyStringKey = 
SpaceStringKey =  
UnsupportedCharsStringTypeKey = a|13||10|b";
            string actual = File.ReadAllText(_testsDataPath);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void BuildTest_NullValue()
        {
            INIWriter builder = new INIWriter(); 
            builder.AppendKVP("NullKeyTest", null);

            bool saveResult = builder.Save(_testsDataPath);
            Assert.IsTrue(saveResult);

            string expected = "NullKeyTest = NULL";
            string actual = File.ReadAllText(_testsDataPath);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void BuildTest_NullKey()
        {
            INIWriter builder = new INIWriter();
            try
            {
                builder.AppendKVP(null, 123);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail();
        }
    }
}