using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDiagramDesigner.Views;

namespace WpfDiagramDesigner.Objects
{
    class ClassNode : Node
    {
        public ClassNode(NodeLayout node, ViewModel.IRefreshable model) : base(node,model)
        {
        }

        public override void AddAttribute()
        {
            var attrib = UMLReader.UmlReader.CreateAttribute();
            ((ClassBuilder)node.NodeObject).OwnedAttribute.Add(attrib);
            var graphAttrib = NodeElementBuilder.AttributeBuilder(attrib, this, model);
            Attributes.Add(graphAttrib);
            attributePanel.Children.Add(graphAttrib);
            model.Refresh();
        }

        public override void AddFunction()
        {
            var operation = UMLReader.UmlReader.CreateFuntion();
            ((ClassBuilder)node.NodeObject).OwnedOperation.Add(operation);
            var graphOperation = NodeElementBuilder.FunctionBuilder(operation, this, model);
            Functions.Add(graphOperation);
            functionPanel.Children.Add(graphOperation);
            model.Refresh();
        }

        public override void AddLiteral()
        {
        }

        public override void RemoveAttribute(TextBox item, PropertyBuilder attributeBuilder)
        {
            ((ClassBuilder)node.NodeObject).OwnedAttribute.Remove(attributeBuilder);
            Attributes.Remove(item);
            attributePanel.Children.Remove(item);
        }

        public override void RemoveFunction(TextBox item, OperationBuilder operationBuilder)
        {
            ((ClassBuilder)node.NodeObject).OwnedOperation.Remove(operationBuilder);
            Functions.Remove(item);
            functionPanel.Children.Remove(item);
        }

        public override void RemoveLiteral(TextBox item, EnumerationLiteralBuilder literal)
        {
           
        }

        protected override void GenerateText()
        {
            var menuItem = new MenuItem
            {
                Header = "Add Attribute"
            };
            menuItem.Click += (e, er) =>
            {
                var attrib = UMLReader.UmlReader.CreateAttribute();
                ((ClassBuilder)node.NodeObject).OwnedAttribute.Add(attrib);
                var graphAttrib=NodeElementBuilder.AttributeBuilder(attrib,this, model);
                Attributes.Add(graphAttrib);
                attributePanel.Children.Add(graphAttrib);
                model.Refresh();
            };
            Name.ContextMenu.Items.Add(menuItem);
            menuItem = new MenuItem
            {
                Header = "Add Function"
            };
            menuItem.Click += (e, er) =>
            {
                var operation = UMLReader.UmlReader.CreateFuntion();
                ((ClassBuilder)node.NodeObject).OwnedOperation.Add(operation);
                var graphOperation = NodeElementBuilder.FunctionBuilder(operation,this, model);
                Functions.Add(graphOperation);
                functionPanel.Children.Add(graphOperation);
                model.Refresh();
            };
            Name.ContextMenu.Items.Add(menuItem);

            foreach (var item in ((ClassBuilder)node.NodeObject).OwnedAttribute)
            {
                var tb = NodeElementBuilder.AttributeBuilder(item,this, model);
                Attributes.Add(tb);
                
            }
            foreach (var item in ((ClassBuilder)node.NodeObject).OwnedOperation)
            {

                var tb = NodeElementBuilder.FunctionBuilder(item,this, model);

                Functions.Add(tb) ;
            }

        }
    }
}
