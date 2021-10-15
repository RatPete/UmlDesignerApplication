using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfDiagramDesigner.Objects
{
    public static class LineBuilder
    {
        public static void DashedLine(Path path)
        {
            path.Stroke = Brushes.Black;
            path.StrokeDashArray = new DoubleCollection() { 6, 1 };
        }
        public static void NonDashedLine(Path path)
        {
            path.Stroke = Brushes.Black;
            path.StrokeDashArray = null;
        }
    }
}
