﻿using MetaDslx.GraphViz;
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
        private bool isDisabled=false;
        List<DiagramElement> Elements { get; set; } = new List<DiagramElement>();
        public void InitDiagram(string inputstr)
        {
            if (inputstr == "")
            {
                UmlReader.LayoutReader(inputstr);
                return;
            }
            var g = UmlReader.LayoutReader(inputstr);
            Elements.Clear();
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

        private void RefreshDiagram()
        {
            var g = UmlReader.RefreshLayout();
            Elements.Clear();
            canvas.Children.Clear();
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

        public void RemoveElement(ElementBuilder el)
        {
            UMLReader.UmlReader.RemoveElementFromModel(el);
        }
    }
}
