using MetaDslx.GraphViz;
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
    class GeneralizationEdge : Edge
    {
        public GeneralizationEdge(EdgeLayout edge) : base(edge)
        {
        }

        protected override void AnimateHead(Point e, Point d, Storyboard storyboard)
        {
            HeadBuilder.AnimateTriangleHead(e, d,storyboard,headPath);
        }

        protected override Path CreateHead(Point lastPoint, Point endPoint)
        {
            return HeadBuilder.CreateTriangleHead(lastPoint, endPoint);
        }

        protected override void SetLineStyle(Path pathLine)
        {
            LineBuilder.NonDashedLine(pathLine);
        }
    }
}
