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
using WpfDiagramDesigner.Views;

namespace WpfDiagramDesigner.Objects
{
    class InterfaceNode : Node
    {
        public InterfaceNode(NodeLayout node, ViewModel.IRefreshable model) : base( node,model)
        {
        }

        protected override void GenerateText()
        {
            foreach (var item in ((InterfaceBuilder)node.NodeObject).OwnedOperation)
            {

                var tb = NodeElementBuilder.FunctionBuilder(item, model);
                Functions.Add(tb);
            }

        }
    }
}
