using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.Views;

namespace WpfDiagramDesigner.Objects
{
    class InterfaceNode : Node
    {
        public InterfaceNode(NodeLayout node, ViewModel.IRefreshable model) : base(node, model)
        {
        }

        public override void AddAttribute()
        {

        }

        public override void AddFunction()
        {
            var operation = UMLReader.UmlReader.CreateFuntion();
            ((InterfaceBuilder)node.NodeObject).OwnedOperation.Add(operation);
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

        }

        public override void RemoveFunction(TextBox item, OperationBuilder operationBuilder)
        {
            ((InterfaceBuilder)node.NodeObject).OwnedOperation.Remove(operationBuilder);
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
                Header = "Add function"
            };
            menuItem.Click += (e, er) =>
            {
                var function = UMLReader.UmlReader.CreateFuntion();
                ((InterfaceBuilder)node.NodeObject).OwnedOperation.Add(function);
                var graphicalFunction = NodeElementBuilder.FunctionBuilder(function, this, model);

                Functions.Add(graphicalFunction);
                functionPanel.Children.Add(graphicalFunction);
                model.Refresh();
            };
            Name.ContextMenu.Items.Add(menuItem);
            foreach (var item in ((InterfaceBuilder)node.NodeObject).OwnedOperation)
            {

                var tb = NodeElementBuilder.FunctionBuilder(item, this, model);
                Functions.Add(tb);
            }
            Name.FontStyle = FontStyles.Italic;

        }

        protected override void RefreshAttributes()
        {
        }

        protected override void RefreshEnums()
        {
        }

        protected override void RefreshFunctions()
        {
            Functions.Clear();
            functionPanel.Children.Clear();
            foreach (var item in ((InterfaceBuilder)(node.NodeObject)).OwnedOperation)
            {
                var graphAttrib = NodeElementBuilder.FunctionBuilder(item, this, model);
                graphAttrib.IsEnabled = RelationshipCreator.CurrentClickType == ClickType.NORMAL;
                Functions.Add(graphAttrib);
                functionPanel.Children.Add(graphAttrib);
            }


        }
    }
}
