using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using WpfDiagramDesigner.Objects;
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
        List<Objects.Node> Nodes { get; set; } = new List<Objects.Node>();
        List<Objects.Edge> Edges { get; set; } = new List<Objects.Edge>();
        List<DiagramElement> Elements { get; set; } = new List<DiagramElement>();
        public void InitDiagram(string inputstr)
        { 
            var g = UmlReader.LayoutReader(inputstr);
            canvas.Children.Clear();
            foreach (NodeLayout node in g.Nodes)
            {
                if (node.NodeObject is ClassBuilder)
                {
                    Nodes.Add(new WpfDiagramDesigner.Objects.ClassNode(node,this));
                }
                else if (node.NodeObject is InterfaceBuilder)
                {
                    Nodes.Add(new WpfDiagramDesigner.Objects.InterfaceNode(node,this));
                }
                else if (node.NodeObject is EnumerationBuilder)
                {
                    Nodes.Add(new WpfDiagramDesigner.Objects.EnumNode(node,this));
                }
            }
            foreach (EdgeLayout edge in g.AllEdges)
            {

                if (edge.EdgeObject is DependencyBuilder && !(edge.EdgeObject is InterfaceRealizationBuilder))
                {
                    Edges.Add(new DependencyEdge(edge));
                }
                else if (edge.EdgeObject is InterfaceRealizationBuilder)
                {
                    Edges.Add(new InterfaceEdge(edge));
                }
                else if (edge.EdgeObject is GeneralizationBuilder)
                {
                    Edges.Add(new GeneralizationEdge(edge));
                }
                else if (edge.EdgeObject is AssociationBuilder)
                {
                    Edges.Add(new AssociationEdge(edge));
                }
                canvas.Width = g.Width;
                canvas.Height = g.Height;
                
            }
            
        }
        public void InitAll()
        {

            foreach (var element in Nodes)
            {
                element.InitCanvasPosition(canvas);
            }
            foreach (var element in Edges)
            {
                element.InitCanvasPosition(canvas);
            }
        }
        public void Refresh()
        {
            RefreshDiagram();
        }


        private void RefreshDiagram()
        {
            var g = UmlReader.RefreshLayout();
            this.Elements.Clear();
            //canvas.Children.Clear();
            var Edges = new List<Edge>();
            var Nodes = new List<Objects.Node>();
            foreach (NodeLayout node in g.Nodes)
            {
                if (node.NodeObject is ClassBuilder)
                {
                    Nodes.Add(new WpfDiagramDesigner.Objects.ClassNode(node, this));
                }
                else if (node.NodeObject is InterfaceBuilder)
                {
                    Nodes.Add(new WpfDiagramDesigner.Objects.InterfaceNode(node, this));
                }
                else if (node.NodeObject is EnumerationBuilder)
                {
                    Nodes.Add(new WpfDiagramDesigner.Objects.EnumNode(node, this));
                }
            }
            foreach (EdgeLayout edge in g.AllEdges)
            {

                if (edge.EdgeObject is DependencyBuilder && !(edge.EdgeObject is InterfaceRealizationBuilder))
                {
                    Edges.Add(new DependencyEdge(edge));
                }
                else if (edge.EdgeObject is InterfaceRealizationBuilder)
                {
                    Edges.Add(new InterfaceEdge(edge));
                }
                else if (edge.EdgeObject is GeneralizationBuilder)
                {
                    Edges.Add(new GeneralizationEdge(edge));
                }
                else if (edge.EdgeObject is AssociationBuilder)
                {
                    Edges.Add(new AssociationEdge(edge));
                }
            }
            canvas.Width = g.Width;
            canvas.Height = g.Height;
            AnimateAll(Edges,Nodes);

        }

        private void AnimateAll(List<Edge> edges,List<Objects.Node> nodes)
        {
            foreach (var item in nodes)
            {
                var animatedNode=Nodes.Find((j) =>
                {
                    return j.NodeName == item.NodeName;
                });
                if (animatedNode == null)
                {
                    item.InitCanvasPosition(canvas);
                }
                else
                {
                    animatedNode.AnimateElementOnCanvas(item.Position);
                }
            }
        }

        public void RemoveElement(ElementBuilder el)
        {
            UMLReader.UmlReader.RemoveElementFromModel(el);
        }
    }
}
