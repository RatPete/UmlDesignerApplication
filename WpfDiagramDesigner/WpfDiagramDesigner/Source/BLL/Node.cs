﻿using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WpfDiagramDesigner.ViewModel;
using WpfDiagramDesigner.Views;

namespace WpfDiagramDesigner.Objects
{
    public abstract class Node : DiagramElement
    {

        protected NodeLayout node;
        public String NodeName { get; protected set; }
        protected readonly ViewModel.IRefreshable model;
        public System.Windows.Point Position { get; private set; }
        private Border ObjectBorder { set; get; }
        protected List<TextBox> Attributes { set; get; } = new List<TextBox>();
        protected List<TextBox> Functions { get; set; } = new List<TextBox>();
        protected TextBox Name { get; set; }
        protected List<TextBox> Enumerations { get; set; } = new List<TextBox>();
        public Node(NodeLayout node, ViewModel.IRefreshable model)
        {
            NodeName = ((NamedElementBuilder)node.NodeObject).Name;
            Position = new System.Windows.Point(node.Position.X - node.Width / 2, node.Position.Y - node.Height / 2);
            this.node = node;
            this.model = model;
        }

        public virtual void InitCanvasPosition(Canvas canvas)
        {
            GenerateCommon();
            ObjectBorder = new Border();
            StackPanel panel = new StackPanel();
            ObjectBorder.Child = panel;
            ObjectBorder.BorderBrush = Brushes.DarkRed;
            ObjectBorder.BorderThickness = new Thickness(2);
            panel.Children.Add(Name);
            panel.Children.Add(new Line() { Stroke = Brushes.DarkRed, StrokeThickness = 2, X1 = 0, X2 = node.Width - 3 });
            foreach (var element in Attributes)
                panel.Children.Add(element);
            if (Attributes.Count > 0 && Functions.Count > 0)
                panel.Children.Add(new Line() { Stroke = Brushes.DarkRed, StrokeThickness = 2, X1 = 0, X2 = node.Width - 3 });
            foreach (var element in Functions)
                panel.Children.Add(element);
            foreach (var element in Enumerations)
                panel.Children.Add(element);
            panel.Background = Brushes.LightYellow;
            
            Canvas.SetLeft(ObjectBorder, node.Position.X - node.Width / 2);
            Canvas.SetTop(ObjectBorder, node.Position.Y - node.Height / 2);
            canvas.Children.Add(ObjectBorder);
        }
        protected abstract void GenerateText();
        private void GenerateCommon()
        {
            var name = ((NamedElementBuilder)node.NodeObject).Name;
            Name = new TextBox();
            GenerateNameBox(Name);
            Name.LostFocus += (e, er) =>
             {
                 InlineParser.NameParser(Name.Text, (NamedElementBuilder)node.NodeObject);
                 GenerateNameBox(Name);
             };
            Name.ContextMenu = new ContextMenu();
            Name.ContextMenu.Items.Clear();
            var menuitem = new MenuItem();
            menuitem.Header = "Remove";
            menuitem.Click += (e, er) => { model.RemoveElement((ElementBuilder)node.NodeObject); model.Refresh();  };
            Name.ContextMenu.Items.Add(menuitem);
            GenerateText();
        }
        private void GenerateNameBox(TextBox name)
        {

            name.Text = ((NamedElementBuilder)node.NodeObject).Name;
            name.Foreground = Brushes.Black;
            name.FontSize = 8.0;
            name.Background = Brushes.Transparent;
            name.BorderBrush = Brushes.Transparent;
            name.BorderThickness = new System.Windows.Thickness(0);




        }

        public void AnimateElementOnCanvas(System.Windows.Point endPoint)
        {
            DoubleAnimation animX = new DoubleAnimation();
            animX.From = Position.X;
            animX.To = endPoint.X;
            animX.Duration = new System.Windows.Duration(new TimeSpan(0,0,1));
            DoubleAnimation animY = new DoubleAnimation();
            animY.From = Position.Y;
            animY.To = endPoint.Y;
            animY.Duration = new System.Windows.Duration(new TimeSpan(0, 0, 1));
            ObjectBorder.BeginAnimation(Canvas.TopProperty, animY);
            ObjectBorder.BeginAnimation(Canvas.LeftProperty, animX);
            return;
        }
    }
}
