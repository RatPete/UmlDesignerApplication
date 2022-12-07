using MetaDslx.GraphViz;
using System;

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfDiagramDesigner.Objects
{
    class InterfaceEdge:Edge
    {
        public InterfaceEdge(EdgeLayout edge, ViewModel.IRefreshable model) : base(edge, model)
        {
        }

        protected override void SetLineStyle(Path pathLine)
        {
            LineBuilder.DashedLine(pathLine);
        }
        protected override Path CreateHead(Point lastPoint, Point endPoint)
        {
            return HeadBuilder.CreateTriangleHead(lastPoint, endPoint);
        }

        protected override void AnimateHead(Point e, Point d, Storyboard storyboard)
        {
            HeadBuilder.AnimateTriangleHead(e, d, storyboard,headPath);
        }
    }
}
