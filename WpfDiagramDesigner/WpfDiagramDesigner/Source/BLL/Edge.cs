using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfDiagramDesigner.Objects
{
    abstract class Edge : DiagramElement
    {
        protected RelationshipBuilder Data;
        protected EdgeGraphicData GraphicData;
        public Edge(EdgeLayout edge)
        {
            Data =(RelationshipBuilder) edge.EdgeObject;
            GraphicData = new EdgeGraphicData();
            var splinePoint = edge.Splines;
            int i = 0;
            GraphicData.SplinePoints=new Point[edge.Splines.Length][];
            foreach(var pointArray in edge.Splines)
            {
                int j = 0;
                GraphicData.SplinePoints[i] = new Point[pointArray.Length];
                foreach (Point2D point2d in pointArray)
                {
                    GraphicData.SplinePoints[i][j] = new Point(point2d.X, point2d.Y);
                    j++;
                }
                i++;
            }
        }
        protected abstract Path CreateHead(Point e,Point d );
        public virtual void Draw(Canvas canvas)
        {
            var visual = new DrawingVisual();
            visual.RenderOpen();
            var path = new PathGeometry();
            var pathGeo = new PathGeometry();
            for (int j = 0; j < GraphicData.SplinePoints.Length; j++)
            {
                var spline = GraphicData.SplinePoints[j];

                var pathFigure = new PathFigure
                {
                    IsClosed = false,
                    StartPoint = new Point(spline[0].X, spline[0].Y)
                };
                for (int i = 1; i < spline.Length; i += 3)
                {
                    var lastPoint = new Point(spline[i+2].X,spline[i+2].Y);
                    var secondLastPoint = new Point(spline[i + 1].X, spline[i + 1].Y);

                    if (i+3 >= spline.Length)
                    {
                       
                        Point newLastPoint = new Point();
                        if (lastPoint.Y - secondLastPoint.Y > 0)
                            newLastPoint.Y = lastPoint.Y - 3;
                        else if (lastPoint.Y - secondLastPoint.Y == 0)
                        {
                            newLastPoint.Y = lastPoint.Y;
                        }
                        else
                        {
                            newLastPoint.Y = lastPoint.Y + 3;
                        }
                        newLastPoint.X = (newLastPoint.Y - secondLastPoint.Y) * (lastPoint.X - secondLastPoint.X) / (lastPoint.Y - secondLastPoint.Y) + secondLastPoint.X;
                        lastPoint = newLastPoint;
                        var segment = new BezierSegment(new Point(spline[i].X, spline[i].Y), new Point(spline[i + 1].X, spline[i + 1].Y), lastPoint, true);
                        pathFigure.Segments.Add(segment);
                        canvas.Children.Add(CreateHead(lastPoint, new Point(spline[i + 2].X, spline[i + 2].Y)));

                    }
                    else
                    {
                        var segment = new BezierSegment(new Point(spline[i].X, spline[i].Y), new Point(spline[i + 1].X, spline[i + 1].Y), lastPoint, true);
                        pathFigure.Segments.Add(segment);
                    }
                    



                }
                path.Figures.Add(pathFigure);

            }
            var pathLine = new Path();
            pathLine.StrokeDashArray = new DoubleCollection() { 6, 1 };
            pathLine.Data = path;
            pathLine.Stroke = Brushes.Black;
            SetLineStyle(pathLine);
            canvas.Children.Add(pathLine);
        }
        protected virtual void SetLineStyle(Path pathLine)
        {
            pathLine.Stroke = Brushes.Black;
        }

        public void EnableTextBoxes()
        {
            
        }

        public void DisableTextBoxes()
        {
         
        }
    }
}
