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

        protected override void GenerateText()
        {

            foreach (var item in ((ClassBuilder)node.NodeObject).OwnedAttribute)
            {
                var tb = NodeElementBuilder.AttributeBuilder(item, model);
                
                Attributes.Add(tb);
                
            }
            foreach (var item in ((ClassBuilder)node.NodeObject).OwnedOperation)
            {

                var tb = NodeElementBuilder.FunctionBuilder(item, model);

                Functions.Add(tb) ;
            }

        }
    }
}
