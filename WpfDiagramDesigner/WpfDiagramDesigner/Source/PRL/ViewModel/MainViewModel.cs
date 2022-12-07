using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WpfDiagramDesigner.Objects;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.Source.PRL.Views;
using WpfDiagramDesigner.UMLReader;

namespace WpfDiagramDesigner.ViewModel
{
    public class MainViewModel : IRefreshable
    {
        private Canvas canvas;
        private MainWindow window;
        public MainViewModel(Canvas canvas,MainWindow window)
        {
            this.window = window;
            this.canvas = canvas;
            canvas.ContextMenu = new ContextMenu();
            canvas.ContextMenu.Items.Clear();
            var menuitem = new MenuItem
            {
                Header = "AddClass"
            };
            menuitem.Click += (e, er) =>
            {
                UMLReader.UmlReader.CreateClass();
                Refresh();
            };
            canvas.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem
            {
                Header = "AddEnum"
            };
            menuitem.Click += (e, er) =>
            {
                UMLReader.UmlReader.CreateEnum();
                Refresh();
            };
            canvas.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem
            {
                Header = "AddInterface"
            };
            menuitem.Click += (e, er) =>
            {
                UMLReader.UmlReader.CreateInterface();
                Refresh();
            };
            canvas.ContextMenu.Items.Add(menuitem);

        }
        private bool isDisabled=false;
        List<DiagramElement> Elements { get; set; } = new List<DiagramElement>();
        public void InitDiagram(string inputstr)
        {
            if (inputstr == "")
            {
                UmlReader.LayoutReader(inputstr);
                canvas.MouseMove += Canvas_MouseMove;
                canvas.Background = Brushes.Transparent;
                canvas.Children.Clear();
                return;
            }
            try
            {
                var g = UmlReader.LayoutReader(inputstr);
            
                Elements.Clear();
            canvas.Background = Brushes.Transparent;
            canvas.Children.Clear();

            foreach (EdgeLayout edge in g.AllEdges)
            {

                if (edge.EdgeObject is DependencyBuilder && !(edge.EdgeObject is InterfaceRealizationBuilder))
                {
                    Elements.Add(new DependencyEdge(edge,this));
                }
                else if (edge.EdgeObject is InterfaceRealizationBuilder)
                {
                    Elements.Add(new InterfaceEdge(edge,this));
                }
                else if (edge.EdgeObject is GeneralizationBuilder)
                {
                    Elements.Add(new GeneralizationEdge(edge,this));
                }
                else if (edge.EdgeObject is AssociationBuilder)
                {
                    DiagramElement element = null;
                    var assoc = (AssociationBuilder)edge.EdgeObject;
                    if (assoc.OwnedEnd.Count == 0)
                    {
                        element = new AssociationEdge(edge,this);
                    }
                    else
                    {
                        if (assoc.OwnedEnd[0].Aggregation == AggregationKind.Composite)
                        {
                            element = new CompositionEdge(edge,this);
                        }
                        else if (assoc.OwnedEnd[0].Aggregation == AggregationKind.Shared)
                        {
                            element = new AggregationEdge(edge,this);
                        }
                        else
                        {
                            element = new OneWayAssociationEdge(edge,this);
                        }
                    }
                    Elements.Add(element);
                }


            }

            foreach (NodeLayout node in g.Nodes)
            {
                if (node.NodeObject is ClassBuilder)
                {
                    Elements.Add(new WpfDiagramDesigner.Objects.ClassNode(node, this));
                }
                else if (node.NodeObject is InterfaceBuilder)
                {
                    Elements.Add(new WpfDiagramDesigner.Objects.InterfaceNode(node, this));
                }
                else if (node.NodeObject is EnumerationBuilder)
                {
                    Elements.Add(new WpfDiagramDesigner.Objects.EnumNode(node, this));
                }
            }
            canvas.Width = g.Width;
            canvas.Height = g.Height;
            canvas.MouseMove += Canvas_MouseMove;

            }
            catch (System.Xml.XmlException)
            {
                InfoPopup popup = new InfoPopup("A fájl megnyitása közben hiba történt, kérlek nézd meg hogy a fájl formátuma megfelel-e az alapvető XMI szabályoknak.", PopupGlobalPosition.Position);
                popup.ShowDialog();
            }
        }

        internal void CancelLineDraw()
        {
            if (canvas.Children.Contains(path))
            {
                canvas.Children.Remove(path);
            }
            if (canvas.Children.Contains(head))
            {
                canvas.Children.Remove(head);
            }
            path = new Path();
            head = new Path();
            drawingStarted = false;
        }

        public void DrawAll()
        {

            foreach (var element in Elements)
            {
                element.Draw(canvas);
            }
        }
        public void DisableAll()
        {
            isDisabled = true;
            foreach (var item in Elements)
            {
                item.DisableTextBoxes();
            }
            
        }
        public void EnableAll()
        {
            isDisabled = false;
            foreach (var item in Elements)
            {
                item.EnableTextBoxes();
            }
        }
        public void Refresh()
        {
            RefreshDiagram();
        }
        bool drawingStarted = false;
        public void StartDrawingLine(System.Windows.Input.MouseButtonEventArgs e)
        {
            //storyBoard.Stop();
            var element = e.GetPosition(canvas);
            path = new Path();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 2;
            canvas.Children.Add(path);
            path.Data = new LineGeometry { StartPoint = element,EndPoint=element};
            canvas.Children.Add(head);
            drawingStarted = true;

        }
        public void EndDrawingLine(System.Windows.Input.MouseButtonEventArgs e)
        {
            canvas.Children.Remove(path);
            canvas.Children.Remove(head);
            path = new Path();
            head = new Path();
            drawingStarted = false;
        }
        Path path=new Path();
        Path head = new Path();
        Storyboard storyBoard = new Storyboard();
        private void RefreshDiagram()
        {
            var g = UmlReader.RefreshLayout();
            List<DiagramElement> foundElements = new List<DiagramElement>();
            storyBoard.Children.Clear();
            foreach (var node in g.Nodes)
            {
                var sameElement = Elements.Find(item => node.NodeObject is NamedElementBuilder && item.Id == ((NamedElementBuilder)node.NodeObject).Name);
                if (sameElement != null)
                {
                    foundElements.Add(sameElement);
                    sameElement.AnimateObject(new Source.PRL.ViewModel.NodeAnimationValues { TargetPosition = new System.Drawing.Point((int)node.Position.X, (int)node.Position.Y), TargetSize = new System.Drawing.Size((int)node.Size.X, (int)node.Size.Y) },storyBoard);
                }
                else
                {
                    DiagramElement element=null;
                    if (node.NodeObject is ClassBuilder)
                    {
                        element = new WpfDiagramDesigner.Objects.ClassNode(node, this);
                        Elements.Add(element);
                    }
                    else if (node.NodeObject is InterfaceBuilder)
                    {
                        element = new WpfDiagramDesigner.Objects.InterfaceNode(node, this);
                        Elements.Add(element);
                    }
                    else if (node.NodeObject is EnumerationBuilder)
                    {
                        element = new WpfDiagramDesigner.Objects.EnumNode(node, this);
                        Elements.Add(element);
                    }
                    if (element != null)
                    {
                        foundElements.Add(element);
                        element.Draw(canvas);
                    }
                }
            }
            foreach (var edge in g.AllEdges)
            {
                var sameElement = Elements.Find(item => edge.EdgeObject is RelationshipBuilder && item.Id == ((RelationshipBuilder)edge.EdgeObject).MId.ToString());
                if (sameElement != null)
                {
                    sameElement.AnimateObject(new Source.PRL.ViewModel.EdgeAnimationValues {TargetPosition=edge.Splines},storyBoard);
                    foundElements.Add(sameElement);
                }
                else
                {
                   
                    DiagramElement element = null;
                    if (edge.EdgeObject is DependencyBuilder && !(edge.EdgeObject is InterfaceRealizationBuilder) && !(edge.EdgeObject is GeneralizationBuilder))
                    {
                        element = new DependencyEdge(edge,this);
                        Elements.Add(element);
                    }
                    else if (edge.EdgeObject is InterfaceRealizationBuilder)
                    {
                        element = new InterfaceEdge(edge,this);
                        Elements.Add(element);
                    }
                    else if (edge.EdgeObject is GeneralizationBuilder)
                    {
                        element = new GeneralizationEdge(edge,this);
                        Elements.Add(element);
                    }
                    else if (edge.EdgeObject is AssociationBuilder)
                    {
                        var assoc = (AssociationBuilder)edge.EdgeObject;
                        if (assoc.OwnedEnd.Count == 0)
                        {
                            element = new AssociationEdge(edge,this);
                        }
                        else
                        {
                            if (assoc.OwnedEnd[0].Aggregation == AggregationKind.Composite)
                            {
                                element = new CompositionEdge(edge,this);
                            }
                            else if (assoc.OwnedEnd[0].Aggregation == AggregationKind.Shared)
                            {
                                element = new AggregationEdge(edge,this);
                            }
                            else
                            {
                                element = new OneWayAssociationEdge(edge,this);
                            }
                            Elements.Add(element);
                        }
                    }
                    if (element != null)
                    {
                        foundElements.Add(element);
                        element.Draw(canvas);
                    }
                    
                }
            }
            var notFoundElement = Elements.FindAll(_ => !foundElements.Contains(_));
            foreach (var item in notFoundElement)
            {
                item.RemoveFromCanvas(canvas);
            }
            Elements.RemoveAll(_ => !foundElements.Contains(_));
            storyBoard.Begin();
            
            canvas.Width = g.Width;
            canvas.Height = g.Height;
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            if (drawingStarted)
            {
                //storyBoard.Stop();
                var geometryData = ((LineGeometry)path.Data);
                var lastPoint = e.GetPosition(canvas);
                var newLastPoint = new Point();
                var startPoint = geometryData.StartPoint;
                if ((startPoint.X - lastPoint.X) * (startPoint.X - lastPoint.X) + (startPoint.Y - lastPoint.Y) * (startPoint.Y - lastPoint.Y) < 20)
                    return;
                if (lastPoint.Y - startPoint.Y > 0)
                    newLastPoint.Y = lastPoint.Y - 3;
                else if (lastPoint.Y - startPoint.Y == 0)
                {
                    newLastPoint.Y = lastPoint.Y;
                }
                else
                {
                    newLastPoint.Y = lastPoint.Y + 3;
                }
                if (lastPoint.X - startPoint.X > 0)
                    newLastPoint.X = lastPoint.X - 3;
                else if (lastPoint.X - startPoint.X == 0)
                {
                    newLastPoint.X = lastPoint.X;
                }
                else
                {
                    newLastPoint.X = lastPoint.X + 3;
                }

                geometryData.EndPoint = newLastPoint;
                RelationshipCreator.GenerateArrow(newLastPoint, lastPoint, path, head);
                head.IsEnabled = false;
                path.IsEnabled = false;
                
            }

            
        }

        public void RemoveElement(ElementBuilder el)
        {
            UMLReader.UmlReader.RemoveElementFromModel(el);
        }

        public void RefocusElement(TextBox tb)
        {
            window.Dispatcher.BeginInvoke((ThreadStart)delegate { tb.Focus(); });
        }
    }
}
