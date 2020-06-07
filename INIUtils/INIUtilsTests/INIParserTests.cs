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
    public class INIParserTests
    {
        static readonly string _testINIFilePath =
            Path.Combine(Environment.CurrentDirectory, "TestsData", Path.GetRandomFileName()) + ".txt";

        INIReader _parser = null;

        [TestInitialize()]
        public void CreateTestFile()
        {
            INIWriter builder = new INIWriter();
            builder.AppendKVP("Int32TypeValue", 123);
            builder.AppendKVP("FloatTypeValue", -123.6F);
            builder.AppendKVP("DoubleTypeValue", 123.4);
            builder.AppendKVP("BooleanTypeValue", false);

            builder.AppendKVP("StringTypeValue", "123");
            builder.AppendKVP("NullStringTypeValue", null);
            builder.AppendKVP("EmptyStringTypeValue", "");
            builder.AppendKVP("SpaceStringTypeValue", " ");
            builder.AppendKVP("KVPPatternStringTypeValue", "(some text 1) = (some text 2)");
            builder.AppendKVP("UnsupportedCharsStringTypeValue", "a" + Environment.NewLine + "b");
            builder.AppendKVP("UnsupportedCharsStringTypeValue2", "|");

            //builder.AppendKVP("Key with spaces", 999); // Исключение как и положено

            builder.Save(_testINIFilePath);

            _parser = new INIReader(_testINIFilePath);
        }

        [TestMethod()]
        public void GetParamTest()
        {
            int actualInt32 = _parser.GetInt32("Int32TypeValue");
            float actualFloat = _parser.GetSingle("FloatTypeValue");
            double actualDouble = _parser.GetDouble("DoubleTypeValue");
            bool actualBool = _parser.GetBoolean("BooleanTypeValue");
            string actualString1 = _parser.GetString("StringTypeValue");
            string actualString2 = _parser.GetString("NullStringTypeValue");
            string actualString3 = _parser.GetString("EmptyStringTypeValue");
            string actualString4 = _parser.GetString("SpaceStringTypeValue");
            string actualString5 = _parser.GetString("KVPPatternStringTypeValue");
            string actualString6 = _parser.GetString("UnsupportedCharsStringTypeValue");
            string actualString7 = _parser.GetString("UnsupportedCharsStringTypeValue2");

            Assert.AreEqual(123, actualInt32);
            Assert.AreEqual(-123.6F, actualFloat);
            Assert.AreEqual(123.4, actualDouble);
            Assert.AreEqual(false, actualBool);
            Assert.AreEqual("123", actualString1);
            Assert.AreEqual(null, actualString2);
            Assert.AreEqual(" ", actualString4);
            Assert.AreEqual("(some text 1) = (some text 2)", actualString5);
            Assert.AreEqual("a" + Environment.NewLine + "b", actualString6);
            Assert.AreEqual("|", actualString7);
        }

        [TestMethod()]
        public void IsParamExistTest_ParamExist()
        {
            bool expected = true;
            bool actual = _parser.IsParamExist("DoubleTypeValue");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IsParamExistTest_ParamNotExist()
        {
            bool expected = false;
            bool actual = _parser.IsParamExist("IamNotExist");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IsParamExistTest_NullKey()
        {
            bool expected = false;
            bool actual = _parser.IsParamExist(null);

            Assert.AreEqual(expected, actual);
        }
    }
}