using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfDiagramDesigner.Objects
{
    class CompositionEdge : AssociationEdge
    {
        public CompositionEdge(EdgeLayout edge, ViewModel.IRefreshable model) : base(edge, model)
        {
        }
        protected override void AnimateHead(Point e, Point d, Storyboard storyboard)
        {
            HeadBuilder.AnimateDiamondHead(e, d, storyboard, headPath);
        }

        protected override Path CreateHead(Point lastPoint, Point endPoint)
        {
            return HeadBuilder.CreateFullDiamondHead(lastPoint, endPoint);
        }

    }
}
