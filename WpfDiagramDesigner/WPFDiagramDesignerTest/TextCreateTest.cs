using MetaDslx.Languages.Uml.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WpfDiagramDesigner.UMLReader;

namespace WPFDiagramDesignerTest
{
    [TestClass]
    public class TextCreateTest
    {
        [TestInitialize]
        public void Inititilize()
        {
            UmlReader.LayoutReader("");
            
        }
        [TestMethod]
        public void TestCreateAttributeText()
        {
            string attributeName = "Test";
            string attributeType = "Type";
            var attribute=UmlReader.CreateAttribute();
            var type = UmlReader.UmlFactory.Type();
            type.Name = attributeType;
            attribute.Name = attributeName;
            attribute.Type = type;
            attribute.Visibility = VisibilityKind.Private;
            var text=UmlReader.CreateAttributeText(attribute);
            Assert.AreEqual("-Test:Type",text);
        }

        [TestMethod]
        public void TestFunctionTextOnlyReturn()
        {
            string functionName = "Test";
            string functionReturnType = "Type";
            var function = UmlReader.CreateFuntion();
            var returntype = UmlReader.UmlFactory.Type();
            returntype.Name = functionReturnType;
            function.Name = functionName;
            var parameter = UmlReader.UmlFactory.Parameter();
            parameter.Direction = ParameterDirectionKind.Return;
            parameter.Operation = function;
            parameter.Type = returntype;
            function.Visibility = VisibilityKind.Private;
            var text = UmlReader.CreateFunctionText(function);
            Assert.AreEqual("-Test( ) : Type", text);
        }
        [TestMethod]
        public void TestFunctionTextReturnAndOneParam()
        {
            string functionName = "Test";
            string functionReturnType = "Type";
            var function = UmlReader.CreateFuntion();
            var returntype = UmlReader.UmlFactory.Type();
            returntype.Name = functionReturnType;
            function.Name = functionName;
            var parameter = UmlReader.UmlFactory.Parameter();
            parameter.Direction = ParameterDirectionKind.Return;
            parameter.Operation = function;
            parameter.Type = returntype;
            var paramtype1 = UmlReader.UmlFactory.Type();
            paramtype1.Name = "Type2";
            var parameter1 = UmlReader.UmlFactory.Parameter();
            parameter1.Direction = ParameterDirectionKind.In;
            parameter1.Operation = function;
            parameter1.Name = "param1";
            parameter1.Type = paramtype1;
            function.Visibility = VisibilityKind.Private;
            var text = UmlReader.CreateFunctionText(function);
            Assert.AreEqual("-Test(param1:Type2 ) : Type", text);
        }
        [TestMethod]
        public void TestFunctionTextReturnAndMultipleParams()
        {
            string functionName = "Test";
            string functionReturnType = "Type";
            var function = UmlReader.CreateFuntion();
            var returntype = UmlReader.UmlFactory.Type();
            returntype.Name = functionReturnType;
            function.Name = functionName;
            var parameter = UmlReader.UmlFactory.Parameter();
            parameter.Direction = ParameterDirectionKind.Return;
            parameter.Operation = function;
            parameter.Type = returntype;
            var paramtype1 = UmlReader.UmlFactory.Type();
            paramtype1.Name = "Type2";
            var parameter1 = UmlReader.UmlFactory.Parameter();
            parameter1.Direction = ParameterDirectionKind.In;
            parameter1.Operation = function;
            parameter1.Name = "param1";
            parameter1.Type = paramtype1;
            var paramtype2 = UmlReader.UmlFactory.Type();
            paramtype2.Name = "Type3";
            var parameter2 = UmlReader.UmlFactory.Parameter();
            parameter2.Direction = ParameterDirectionKind.In;
            parameter2.Operation = function;
            parameter2.Name = "param2";
            parameter2.Type = paramtype2;
            function.Visibility = VisibilityKind.Private;
            var text = UmlReader.CreateFunctionText(function);
            Assert.AreEqual("-Test(param1:Type2 ,param2:Type3) : Type", text);
        }

    }
}
