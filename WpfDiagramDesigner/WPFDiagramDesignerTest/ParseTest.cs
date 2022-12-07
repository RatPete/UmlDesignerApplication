using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfDiagramDesigner;
using WpfDiagramDesigner.Source.PRL.Helper;

namespace WPFDiagramDesignerTest
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public void TestCanParseAttributeSuccess()
        {
            try
            {
                bool res = InlineParser.CanParseAttribute("+attrib:Type");
                Assert.AreEqual(res, true);
            }
            catch (ObjectNotParsableException ex)
            {
                Assert.AreEqual(false, true);
            }
        }
        [TestMethod]
        public void TestCanParseAttributeFailure()
        {
            try
            {
                bool res = InlineParser.CanParseAttribute("attrib:Type");
                Assert.AreEqual(res, false);
            }
            catch (ObjectNotParsableException ex)
            {
                Assert.AreEqual(true, true);
            }
        }
        [TestMethod]
        public void TestCanParseFunctionSuccess()
        {
            try
            {
                bool res = InlineParser.CanParseFunction("+func(param1:paramClass):void");
                Assert.AreEqual(res, true);
            }
            catch (ObjectNotParsableException ex)
            {
                Assert.AreEqual(false, true);
            }
        }
        [TestMethod]
        public void TestCanParseFunctionFailure()
        {
            try
            {
                bool res = InlineParser.CanParseFunction("func(param1:paramClass):void");
                Assert.AreEqual(res, false);
            }
            catch (ObjectNotParsableException ex)
            {
                Assert.AreEqual(true, true);
            }
        }
        [TestMethod]
        public void TestCanParseEnumerationLiteralSuccess()
        {
            try
            {
                bool res = InlineParser.CanParseEnum("ENUMLITERAL");
                Assert.AreEqual(res, true);
            }
            catch (ObjectNotParsableException ex)
            {
                Assert.AreEqual(false, true);
            }
        }
        [TestMethod]
        public void TestCanParseEnumerationLiteralFailure()
        {
            try
            {
                bool res = InlineParser.CanParseEnum("a+2c\"test\"");
                Assert.AreEqual(res, false);
            }
            catch (ObjectNotParsableException ex)
            {
                Assert.AreEqual(true, true);
            }
        }

    }
}
