using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WpfDiagramDesigner.Source.PRL.ViewModel;

namespace WpfDiagramDesigner.Objects
{
    abstract class Edge : DiagramElement
    {
        protected RelationshipBuilder Data;
        protected EdgeGraphicData GraphicData;

        public string Id { get { return Data.MId.ToString(); } }
        private ViewModel.IRefreshable model;

        public Edge(EdgeLayout edge, ViewModel.IRefreshable model)
        {
            this.model = model;
            Data = (RelationshipBuilder)edge.EdgeObject;
            var data = Data.MId;
            GraphicData = new EdgeGraphicData();
            var splinePoint = edge.Splines;
            int i = 0;
            GraphicData.SplinePoints = new Point[edge.Splines.Length][];
            foreach (var pointArray in edge.Splines)
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
        protected abstract Path CreateHead(Point e, Point d);
        private List<BezierSegment> Segments = new List<BezierSegment>();
        Path pathLine;
        Path extraPathLine;
        protected Path headPath;
        Canvas canvas;
        public virtual void Draw(Canvas canvas)
        {
            this.canvas = canvas;
            var path = new PathGeometry();

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
                    var lastPoint = new Point(spline[i + 2].X, spline[i + 2].Y);
                    var secondLastPoint = new Point(spline[i + 1].X, spline[i + 1].Y);

                    if (i + 3 >= spline.Length)
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
                        Segments.Add(segment);
                        pathFigure.Segments.Add(segment);
                        headPath = CreateHead(lastPoint, new Point(spline[i + 2].X, spline[i + 2].Y));
                        canvas.Children.Add(headPath);

                    }
                    else
                    {
                        var segment = new BezierSegment(new Point(spline[i].X, spline[i].Y), new Point(spline[i + 1].X, spline[i + 1].Y), lastPoint, true);
                        Segments.Add(segment);
                        pathFigure.Segments.Add(segment);
                    }




                }
                path.Figures.Add(pathFigure);

            }
            pathLine = new Path
            {
                StrokeDashArray = new DoubleCollection() { 6, 1 },
                Data = path,
                Stroke = Brushes.Black,
            };
            SetLineStyle(pathLine);
            // pathLine.ContextMenu.Items.Clear();
            extraPathLine = new Path
            {
                StrokeDashArray = new DoubleCollection() { 6, 1 },
                Data = path,
                Stroke = Brushes.Transparent,
                StrokeThickness = 5
            };

            extraPathLine.ContextMenu = new ContextMenu();


            var menuitem = new MenuItem();
            menuitem.Header = "Remove";
            menuitem.Click += (e, er) =>
            {
                model.RemoveElement(Data); model.Refresh();
            };
            extraPathLine.ContextMenu.Items.Add(menuitem);
            canvas.Children.Add(pathLine);
            canvas.Children.Add(extraPathLine);
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

        public void AnimateObject(AnimationValues a, Storyboard storyboard)
        {
            int i = 0;
            EdgeGraphicData graphdata = new EdgeGraphicData();

            var edgeAnim = (EdgeAnimationValues)a;
            graphdata.SplinePoints = new Point[edgeAnim.TargetPosition.Length][];
            foreach (var pointArray in edgeAnim.TargetPosition)
            {
                int j = 0;
                graphdata.SplinePoints[i] = new Point[pointArray.Length];
                foreach (var point2d in pointArray)
                {
                    graphdata.SplinePoints[i][j] = new Point(point2d.X, point2d.Y);
                    j++;
                }
                i++;
            }
            List<BezierSegment> segments = new List<BezierSegment>();
            Point startPoint = graphdata.SplinePoints[0][0];
            Point newLastPoint;
            for (int j = 0; j < graphdata.SplinePoints.Length; j++)
            {
                var spline = graphdata.SplinePoints[j];
                for (i = 1; i < spline.Length; i += 3)
                {
                    var lastPoint = new Point(spline[i + 2].X, spline[i + 2].Y);
                    var secondLastPoint = new Point(spline[i + 1].X, spline[i + 1].Y);

                    if (i + 3 >= spline.Length)
                    {

                        newLastPoint = new Point();
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
                        AnimateHead(lastPoint, new Point(spline[i + 2].X, spline[i + 2].Y), storyboard);
                        segments.Add(segment);

                    }
                    else
                    {
                        var segment = new BezierSegment(new Point(spline[i].X, spline[i].Y), new Point(spline[i + 1].X, spline[i + 1].Y), lastPoint, true);
                        segments.Add(segment);
                    }

                }

            }
            int differenceOfSegments = 0;
            if (segments.Count > Segments.Count)
            {

                ExpandEdgeParts(segments.Count - Segments.Count);
            }
            else if (segments.Count < Segments.Count)
            {
                differenceOfSegments = Segments.Count - segments.Count;
                for (int j = 0; j < differenceOfSegments; j++)
                {
                    int count = ((PathGeometry)pathLine.Data).Figures[0].Segments.Count;
                    ((PathGeometry)pathLine.Data).Figures[0].Segments.RemoveAt(count - 1);
                    Segments.RemoveAt(count - 1);

                    //}
                    //for (int j = 0; j < Segments.Count - segments.Count; j++)
                    //{

                    //    segments.Add(segments[segments.Count - 1]);
                    //}
                }
            }

            PointAnimation starAnimation = new PointAnimation
            {
                From = ((PathGeometry)pathLine.Data).Figures[0].StartPoint,
                To = startPoint,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(starAnimation, pathLine);
            Storyboard.SetTargetProperty(starAnimation, new PropertyPath("Data.Figures[0].StartPoint"));
            storyboard.Children.Add(starAnimation);
            for (i = 0; i < segments.Count; i++)
            {
                PointAnimation animation = new PointAnimation
                {
                    From = Segments[i].Point1,
                    To = new Point(segments[i].Point1.X, segments[i].Point1.Y),
                    Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
                };
                Storyboard.SetTarget(animation, pathLine);
                Storyboard.SetTargetProperty(animation, new PropertyPath($"Data.Figures[0].Segments[{i}].Point1"));
                storyboard.Children.Add(animation);
                PointAnimation animation2 = new PointAnimation
                {
                    Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2)),
                    From = Segments[i].Point2,
                    To = new Point(segments[i].Point2.X, segments[i].Point2.Y)
                };
                Storyboard.SetTarget(animation2, pathLine);
                Storyboard.SetTargetProperty(animation2, new PropertyPath($"Data.Figures[0].Segments[{i}].Point2"));
                storyboard.Children.Add(animation2);
                if (i == segments.Count - 1)
                {
                    PointAnimation animation3 = new PointAnimation
                    {
                        From = Segments[i].Point3,
                        To = newLastPoint,
                        Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
                    };
                    Storyboard.SetTarget(animation3, pathLine);
                    Storyboard.SetTargetProperty(animation3, new PropertyPath($"Data.Figures[0].Segments[{i}].Point3"));
                    storyboard.Children.Add(animation3);
                }
                else
                {
                    PointAnimation animation3 = new PointAnimation
                    {
                        From = Segments[i].Point3,
                        To = new Point(segments[i].Point3.X, segments[i].Point3.Y),
                        Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
                    };
                    Storyboard.SetTarget(animation3, pathLine);
                    Storyboard.SetTargetProperty(animation3, new PropertyPath($"Data.Figures[0].Segments[{i}].Point3"));
                    storyboard.Children.Add(animation3);
                }
            }
            storyboard.Completed += (e, er) =>
            {
                if (extraPathLine != null)
                {
                    canvas.Children.Remove(extraPathLine);
                    extraPathLine = new Path
                    {
                        StrokeDashArray = new DoubleCollection() { 6, 1 },
                        Data = pathLine.Data,
                        Stroke = Brushes.Transparent,
                        StrokeThickness = 5
                    };
                    extraPathLine.ContextMenu = new ContextMenu();
                    var menuitem = new MenuItem();
                    menuitem.Header = "Remove";
                    menuitem.Click += (e, er) =>
                    {
                        model.RemoveElement(Data); model.Refresh();
                    };

                    extraPathLine.ContextMenu.Items.Add(menuitem);
                    canvas.Children.Add(extraPathLine);

                }
            };
            Segments = segments;

        }
        private void ExpandEdgeParts(int increaseBy)
        {
            canvas.Children.Remove(pathLine);
            canvas.Children.Remove(extraPathLine);
            for (int i = 0; i < increaseBy; i++)
            {
                var segment = Segments[Segments.Count - 1];
                Segments.Add(new BezierSegment(segment.Point3, segment.Point3, segment.Point3,true));
            }
            PathGeometry path = new PathGeometry();
            PathFigure figure = new PathFigure
            {
                IsClosed = false,
                StartPoint = ((PathGeometry)pathLine.Data).Figures[0].StartPoint
            };
            
            foreach (var segment in Segments)
            {

                figure.Segments.Add(segment);
            }
            path.Figures.Add(figure);
            pathLine = new Path
            {
                StrokeDashArray = new DoubleCollection() { 6, 1 },
                Data = path,
                Stroke = Brushes.Black
            };
            SetLineStyle(pathLine);
            extraPathLine = new Path
            {
                StrokeDashArray = new DoubleCollection() { 6, 1 },
                Data = path,
                Stroke = Brushes.Transparent,
                StrokeThickness = 5
            };

            extraPathLine.ContextMenu = new ContextMenu();
            var menuitem = new MenuItem();
            menuitem.Header = "Remove";
            menuitem.Click += (e, er) =>
            {
                model.RemoveElement(Data); model.Refresh();
            };

            extraPathLine.ContextMenu.Items.Add(menuitem);
            canvas.Children.Add(pathLine);
            canvas.Children.Add(extraPathLine);
        }

        public void RemoveFromCanvas(Canvas canvas)
        {
            if (canvas.Children.Contains(pathLine))
            {
                canvas.Children.Remove(pathLine);
            }
            if (canvas.Children.Contains(extraPathLine))
            {
                canvas.Children.Remove(extraPathLine);
            }
            if (canvas.Children.Contains(headPath))
            {
                canvas.Children.Remove(headPath);
            }
        }
        protected abstract void AnimateHead(Point e, Point d, Storyboard storyboard);
    }

}