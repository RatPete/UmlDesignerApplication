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

        protected override void GenerateText()
        {
            var enumeration = (EnumerationBuilder)node.NodeObject;
            foreach (var item in enumeration.OwnedLiteral)
            {
                Enumerations.Add(NodeElementBuilder.EnumBuilder(item, model));
            }
            

        }
    }
}
