using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfDiagramDesigner.Objects
{
    public static class HeadBuilder
    {
        private static PathFigure CreateArrowFigure(Point lastPoint, Point endPoint)
        {
            
            var figure = new PathFigure
            {
                IsClosed = false,
                StartPoint = lastPoint
            };
            Matrix m = Matrix.Identity;
            m.Rotate(90);
            Vector v1 = new Vector(lastPoint.X, lastPoint.Y);
            Vector v2 = new Vector(endPoint.X, endPoint.Y);
            Vector v3 = v2 - v1;
            v3.Normalize();
            v3 = m.Transform(v3) * 3;
            Point point1 = new Point((v3 + v1).X, (v3 + v1).Y);
            Vector v3Neg = v3;
            v3Neg.Negate();
            Point point2 = new Point(((v3Neg + v1)).X, ((v3Neg + v1)).Y);
            var seg = new LineSegment() { Point = point1, IsStroked = false };
            var seg2 = new LineSegment() { Point = endPoint };
            var seg3 = new LineSegment() { Point = point2 };
            var seg4 = new LineSegment() { Point = lastPoint, IsStroked = false };
            var seg5 = new LineSegment() { Point = endPoint };
            figure.Segments.Add(seg);
            figure.Segments.Add(seg2);
            figure.Segments.Add(seg3);
            figure.Segments.Add(seg4);
            figure.Segments.Add(seg5);
            return figure;
        }
        private static PathFigure CreateTriangleFigure(Point lastPoint, Point endPoint)
        {
            var figure = new PathFigure
            {
                IsClosed = true,
                StartPoint = lastPoint,
            };
            Matrix m = Matrix.Identity;
            m.Rotate(90);
            Vector v1 = new Vector(lastPoint.X, lastPoint.Y);
            Vector v2 = new Vector(endPoint.X, endPoint.Y);
            Vector v3 = v2 - v1;
            v3.Normalize();
            v3 = m.Transform(v3) * 3;
            Point point1 = new Point((v3 + v1).X, (v3 + v1).Y);
            Vector v3Neg = v3;
            v3Neg.Negate();
            Point point2 = new Point(((v3Neg + v1)).X, ((v3Neg + v1)).Y);
            var seg = new LineSegment() { Point = point1 };
            var seg2 = new LineSegment() { Point = endPoint };
            var seg3 = new LineSegment() { Point = point2 };
            var seg4 = new LineSegment() { Point = lastPoint };
            figure.Segments.Add(seg);
            figure.Segments.Add(seg2);
            figure.Segments.Add(seg3);
            figure.Segments.Add(seg4);
            return figure;

        }
        public static Path CreateTriangleHead(Point lastPoint, Point endPoint)
        {
            var pathGeo = new PathGeometry();
            var figure = CreateTriangleFigure(lastPoint, endPoint);
            pathGeo.Figures.Add(figure);
            var path = new Path();
            path.Data = pathGeo;
            path.Stroke = Brushes.Black;
            return path;
        }
        public static Path CreateArrowHead(Point lastPoint, Point endPoint)
        {
            var pathGeo = new PathGeometry();
            var figure = CreateArrowFigure(lastPoint, endPoint);
            pathGeo.Figures.Add(figure);
            var path = new Path
            {
                Data = pathGeo,
                Stroke = Brushes.Black,
                
            };
            return path;
        }
        public static Path CreateNoHead(Point lastPoint, Point endPoint)
        {
            var pathGeo = new PathGeometry();
            var figure = new PathFigure
            {
                IsClosed = false,
                StartPoint = lastPoint
            };
            var seg2 = new LineSegment() { Point = endPoint };
            figure.Segments.Add(seg2);
            pathGeo.Figures.Add(figure);
            var path = new Path
            {
                Data = pathGeo,
                Stroke = Brushes.Black
            };
            return path;
        }
        public static Path CreateDiamondHead(Point lastPoint, Point endPoint)
        {
            Point middlePoint = new Point((lastPoint.X + endPoint.X) / 2, (lastPoint.Y + endPoint.Y) / 2);
            PathFigure data1 = CreateArrowFigure(middlePoint, lastPoint);
            PathFigure data2 = CreateArrowFigure(middlePoint,endPoint);
            PathGeometry geo = new PathGeometry();
            geo.Figures.Add(data1);
            geo.Figures.Add(data2);
            Path path = new Path()
            {
                Data = geo,
                Stroke = Brushes.Black,
                Fill = Brushes.Black
            };
            return path;
        }
    }

}
