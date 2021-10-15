using MetaDslx.GraphViz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfDiagramDesigner.Objects
{
    class DependencyEdge : Edge
    {
        public DependencyEdge(EdgeLayout edge) : base(edge)
        {
        }

        protected override Path CreateHead(Point lastPoint, Point endPoint)
        {
            return HeadBuilder.CreateArrowHead(lastPoint, endPoint);
        }

        protected override void SetLineStyle(Path pathLine)
        {
            LineBuilder.DashedLine(pathLine);
        }
    }
}
