using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDiagramDesigner.Views;

namespace WpfDiagramDesigner.Objects
{
    class EnumNode : Node
    {
        public EnumNode( NodeLayout node, ViewModel.IRefreshable model) : base(node,model)
        {
        }

        public override void AddAttribute()
        {
        }

        public override void AddFunction()
        {
        }

        public override void AddLiteral()
        {
            var enumerationLiteral = UMLReader.UmlReader.CreateEnumerationLiteral();
            ((EnumerationBuilder)node.NodeObject).OwnedLiteral.Add(enumerationLiteral);
            var graphicalLiteral = NodeElementBuilder.EnumBuilder(enumerationLiteral, this, model);
            Enumerations.Add(graphicalLiteral);
            enumPanel.Children.Add(graphicalLiteral);
            model.Refresh();
        }

        public override void RemoveAttribute(TextBox item, PropertyBuilder attributeBuilder)
        {
        }

        public override void RemoveFunction(TextBox item, OperationBuilder operationBuilder)
        {
        }

        public override void RemoveLiteral(TextBox item, EnumerationLiteralBuilder literal)
        {
            ((EnumerationBuilder)node.NodeObject).OwnedLiteral.Remove(literal);
            enumPanel.Children.Remove(item);
            Enumerations.Remove(item);
        }

        protected override void GenerateText()
        {
            var menuItem = new MenuItem
            {
                Header = "Add enumeration literal"
            };
            menuItem.Click += (e, er) =>
            {
                var enumerationLiteral = UMLReader.UmlReader.CreateEnumerationLiteral();
                ((EnumerationBuilder)node.NodeObject).OwnedLiteral.Add(enumerationLiteral);
                var graphicalLiteral = NodeElementBuilder.EnumBuilder(enumerationLiteral,this, model);
                Enumerations.Add(graphicalLiteral);
                enumPanel.Children.Add(graphicalLiteral);
                model.Refresh();
            };
            Name.ContextMenu.Items.Add(menuItem);
            var enumeration = (EnumerationBuilder)node.NodeObject;
            foreach (var item in enumeration.OwnedLiteral)
            {
                Enumerations.Add(NodeElementBuilder.EnumBuilder(item,this, model));
            }
            

        }

        protected override void RefreshAttributes()
        {
        }

        protected override void RefreshEnums()
        {
            //TODO
        }

        protected override void RefreshFunctions()
        {
        }
    }
}
