using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            CalculateDiagonalPoints(lastPoint, endPoint, out Point point1, out Point point2);
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

        public static void AnimateArrowHead(Point e, Point d, Storyboard storyboard, Path headPath)
        {
            var figure = CreateArrowFigure(e,d);
            PointAnimation starAnimation = new PointAnimation
            {
                From = ((PathGeometry)headPath.Data).Figures[0].StartPoint,
                To = e,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(starAnimation, headPath);
            Storyboard.SetTargetProperty(starAnimation, new PropertyPath("Data.Figures[0].StartPoint"));
            storyboard.Children.Add(starAnimation);
            int i = 0;
            foreach (var item in figure.Segments)
            {
                PointAnimation pointAnimation = new PointAnimation
                {
                    From = ((LineSegment)((PathGeometry)headPath.Data).Figures[0].Segments[i]).Point,
                    To = ((LineSegment)item).Point,
                    Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
                };
                Storyboard.SetTarget(pointAnimation, headPath);
                Storyboard.SetTargetProperty(pointAnimation, new PropertyPath($"Data.Figures[0].Segments[{i}].Point"));
                storyboard.Children.Add(pointAnimation);
                i++;
            }
        }

        public static void AnimateTriangleHead(Point e, Point d, Storyboard storyboard,Path headPath)
        {
           var figure= CreateTriangleFigure(e, d);
           PointAnimation starAnimation = new PointAnimation
            {
                From =((PathGeometry) headPath.Data).Figures[0].StartPoint,
                To = e,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(starAnimation, headPath);
            Storyboard.SetTargetProperty(starAnimation, new PropertyPath("Data.Figures[0].StartPoint"));
            storyboard.Children.Add(starAnimation);
            int i = 0;
            foreach (var item in figure.Segments)
            {
                PointAnimation pointAnimation = new PointAnimation
                {
                    From = ((LineSegment)((PathGeometry)headPath.Data).Figures[0].Segments[i]).Point,
                    To = ((LineSegment)item).Point,
                    Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
                };
                Storyboard.SetTarget(pointAnimation, headPath);
                Storyboard.SetTargetProperty(pointAnimation, new PropertyPath($"Data.Figures[0].Segments[{i}].Point"));
                storyboard.Children.Add(pointAnimation);
                i++;
            }
        }

        private static PathFigure CreateTriangleFigure(Point lastPoint, Point endPoint)
        {
            var figure = new PathFigure
            {
                IsClosed = true,
                StartPoint = lastPoint,
            };
            CalculateDiagonalPoints(lastPoint, endPoint, out Point point1, out Point point2);
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
        public static void AnimateNoHead(Point lastPoint,Point endPoint,Storyboard storyboard,Path headPath)
        {
            PointAnimation starAnimation = new PointAnimation
            {
                From = ((PathGeometry)headPath.Data).Figures[0].StartPoint,
                To = lastPoint,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(starAnimation, headPath);
            Storyboard.SetTargetProperty(starAnimation, new PropertyPath("Data.Figures[0].StartPoint"));
            storyboard.Children.Add(starAnimation);
            PointAnimation pointAnimation = new PointAnimation
            {
                From = ((LineSegment)((PathGeometry)headPath.Data).Figures[0].Segments[0]).Point,
                To = endPoint,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(pointAnimation, headPath);
            Storyboard.SetTargetProperty(pointAnimation, new PropertyPath($"Data.Figures[0].Segments[0].Point"));
            storyboard.Children.Add(pointAnimation);
            
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
        public static Path CreateFullDiamondHead(Point lastPoint,Point endPoint)
        {
            Path path = CreateEmptyDiamondHead(lastPoint, endPoint);
            path.Fill = Brushes.Black;
            return path;
        }
        private static void CalculateDiagonalPoints(Point lastPoint,Point endPoint,out Point point1,out Point point2)
        {
            Matrix m = Matrix.Identity;
            m.Rotate(90);
            Vector v1 = new Vector(lastPoint.X, lastPoint.Y);
            Vector v2 = new Vector(endPoint.X, endPoint.Y);
            Vector v3 = v2 - v1;
            v3.Normalize();
            v3 = m.Transform(v3) * 3;
            point1 = new Point((v3 + v1).X, (v3 + v1).Y);
            Vector v3Neg = v3;
            v3Neg.Negate();
            point2 = new Point(((v3Neg + v1)).X, ((v3Neg + v1)).Y);
        }
        public static Path CreateEmptyDiamondHead(Point lastPoint, Point endPoint)
        {
            Point middlePoint = new Point((lastPoint.X + endPoint.X) / 2, (lastPoint.Y + endPoint.Y) / 2);
            CalculateDiagonalPoints(middlePoint, endPoint, out Point point1, out Point point2);
            PathGeometry polygon = new PathGeometry();
            PathFigure figure = new PathFigure
            {
                StartPoint = endPoint,
                IsClosed = true
            };
            var seg = new LineSegment() { Point = point1 };
            var seg2 = new LineSegment() { Point = lastPoint };
            var seg3 = new LineSegment() { Point = point2 };
            figure.Segments.Add(seg);
            figure.Segments.Add(seg2);
            figure.Segments.Add(seg3);
            polygon.Figures.Add(figure);
            
            Path path = new Path()
            {
                Data = polygon,
                Stroke = Brushes.Black,
                Fill = Brushes.Transparent,
               
            };
            return path;
        }
        public static void AnimateDiamondHead(Point lastPoint, Point endPoint, Storyboard storyboard, Path headPath)
        {
           
            Point middlePoint = new Point((lastPoint.X + endPoint.X) / 2, (lastPoint.Y + endPoint.Y) / 2);
            CalculateDiagonalPoints(middlePoint, lastPoint, out Point point1, out Point point2);
            PointAnimation starAnimation = new PointAnimation
            {
                From = ((PathGeometry)headPath.Data).Figures[0].StartPoint,
                To = endPoint,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(starAnimation, headPath);
            Storyboard.SetTargetProperty(starAnimation, new PropertyPath("Data.Figures[0].StartPoint"));
            storyboard.Children.Add(starAnimation);
            PointAnimation point1anim = new PointAnimation
            {
                From = ((LineSegment)((PathGeometry)headPath.Data).Figures[0].Segments[1]).Point,
                To = point1,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(point1anim, headPath);
            Storyboard.SetTargetProperty(point1anim, new PropertyPath("Data.Figures[0].Segments[0].Point"));
            storyboard.Children.Add(point1anim);
            PointAnimation point2anim = new PointAnimation
            {
                From = ((LineSegment)((PathGeometry)headPath.Data).Figures[0].Segments[2]).Point,
                To = lastPoint,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(point2anim, headPath);
            Storyboard.SetTargetProperty(point2anim, new PropertyPath("Data.Figures[0].Segments[1].Point"));
            storyboard.Children.Add(point2anim);
            PointAnimation point3anim = new PointAnimation
            {
                From = ((LineSegment)((PathGeometry)headPath.Data).Figures[0].Segments[2]).Point,
                To = point2,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(point3anim, headPath);
            Storyboard.SetTargetProperty(point3anim, new PropertyPath("Data.Figures[0].Segments[2].Point"));
            storyboard.Children.Add(point3anim);


        }


    }

}
