using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewXMLCreatorCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewXMLCreatorCore.Tests
{
    [TestClass()]
    public class UtilTests
    {
        [TestMethod()]
        public void IsArrayWithNumbersTest()
        {
            Assert.IsTrue(Util.IsArrayWithNumbers(new string[] { "1", "3", "-2", "7" }));
            Assert.IsTrue(Util.IsArrayWithNumbers(new string[] { "1", "3", "2", "5", "7", "11", "1221122" }));
            Assert.IsFalse(Util.IsArrayWithNumbers(new string[] { "1", "3c", "2", "5", "7", "11", "1b" }));
            Assert.IsFalse(Util.IsArrayWithNumbers(new string[] { "false" }));
            Assert.IsFalse(Util.IsArrayWithNumbers(new string[] { "hello", "world" }));
        }
    }
}