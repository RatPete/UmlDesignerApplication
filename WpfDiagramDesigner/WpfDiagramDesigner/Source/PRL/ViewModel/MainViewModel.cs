using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfDiagramDesigner.Objects;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.UMLReader;

namespace WpfDiagramDesigner.ViewModel
{
    public class MainViewModel : IRefreshable
    {
        private Canvas canvas;

        public MainViewModel(Canvas canvas)
        {
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
            var g = UmlReader.LayoutReader(inputstr);
            Elements.Clear();
            canvas.Background = Brushes.Transparent;
            canvas.Children.Clear();

            foreach (NodeLayout node in g.Nodes)
            {
                if (node.NodeObject is ClassBuilder)
                {
                    Elements.Add(new WpfDiagramDesigner.Objects.ClassNode(node,this));
                }
                else if (node.NodeObject is InterfaceBuilder)
                {
                    Elements.Add(new WpfDiagramDesigner.Objects.InterfaceNode(node,this));
                }
                else if (node.NodeObject is EnumerationBuilder)
                {
                    Elements.Add(new WpfDiagramDesigner.Objects.EnumNode(node,this));
                }
            }
            foreach (EdgeLayout edge in g.AllEdges)
            {

                if (edge.EdgeObject is DependencyBuilder && !(edge.EdgeObject is InterfaceRealizationBuilder))
                {
                    Elements.Add(new DependencyEdge(edge));
                }
                else if (edge.EdgeObject is InterfaceRealizationBuilder)
                {
                    Elements.Add(new InterfaceEdge(edge));
                }
                else if (edge.EdgeObject is GeneralizationBuilder)
                {
                    Elements.Add(new GeneralizationEdge(edge));
                }
                else if (edge.EdgeObject is AssociationBuilder)
                {
                    Elements.Add(new AssociationEdge(edge));
                }
                canvas.Width = g.Width;
                canvas.Height = g.Height;
                
            }
            
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
            DrawAll();
            if (isDisabled)
            {
                DisableAll();
            }
        }
        bool drawingStarted = false;
        public void StartDrawingLine(System.Windows.Input.MouseButtonEventArgs e)
        {
            var element = e.GetPosition(canvas);
            path = new Path();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 2;
            canvas.Children.Add(path);
            path.Data = new LineGeometry { StartPoint = element,EndPoint=element};
            drawingStarted = true;

        }
        public void EndDrawingLine(System.Windows.Input.MouseButtonEventArgs e)
        {
            path = new Path();
            head = new Path();
            drawingStarted = false;
        }
        Path path=new Path();
        Path head = new Path();
        private void RefreshDiagram()
        {
            var g = UmlReader.RefreshLayout();
            Elements.Clear();
            canvas.Children.Clear();
            canvas.Children.Add(path);
            canvas.Children.Add(head);
            canvas.MouseMove += Canvas_MouseMove;
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
            foreach (EdgeLayout edge in g.AllEdges)
            {

                if (edge.EdgeObject is DependencyBuilder && !(edge.EdgeObject is InterfaceRealizationBuilder))
                {
                    Elements.Add(new DependencyEdge(edge));
                }
                else if (edge.EdgeObject is InterfaceRealizationBuilder)
                {
                    Elements.Add(new InterfaceEdge(edge));
                }
                else if (edge.EdgeObject is GeneralizationBuilder)
                {
                    Elements.Add(new GeneralizationEdge(edge));
                }
                else if (edge.EdgeObject is AssociationBuilder)
                {
                    Elements.Add(new AssociationEdge(edge));
                }
                canvas.Width = g.Width;
                canvas.Height = g.Height;

            }

        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (drawingStarted)
            {
                var geometryData = ((LineGeometry)path.Data);
                var lastPoint = e.GetPosition(canvas);
                var newLastPoint = new Point();
                var startPoint = geometryData.StartPoint;
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
                
                
            }

            
        }

        public void RemoveElement(ElementBuilder el)
        {
            UMLReader.UmlReader.RemoveElementFromModel(el);
        }
    }
}
