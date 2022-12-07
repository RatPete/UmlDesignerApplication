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
    class OneWayAssociationEdge : AssociationEdge
    {
        public OneWayAssociationEdge(EdgeLayout edge, ViewModel.IRefreshable model) : base(edge, model)
        {
        }
        protected override void AnimateHead(Point e, Point d, Storyboard storyboard)
        {
            HeadBuilder.AnimateArrowHead(e, d, storyboard, headPath);
        }

        protected override Path CreateHead(Point lastPoint, Point endPoint)
        {
            return HeadBuilder.CreateArrowHead(lastPoint, endPoint);
        }

        protected override void SetLineStyle(Path pathLine)
        {
            LineBuilder.NonDashedLine(pathLine);
        }
    }
}
