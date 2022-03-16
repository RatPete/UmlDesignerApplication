using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.ViewModel;
using WpfDiagramDesigner.Views;

namespace WpfDiagramDesigner.Objects
{
    public abstract class Node : DiagramElement
    {

        protected NodeLayout node;
        protected readonly ViewModel.IRefreshable model;

        protected List<TextBox> Attributes { set; get; } = new List<TextBox>();
        protected List<TextBox> Functions { get; set; } = new List<TextBox>();
        protected TextBox Name { get; set; }
        protected List<TextBox> Enumerations { get; set; } = new List<TextBox>();
        public Node(NodeLayout node, ViewModel.IRefreshable model)
        {
            this.node = node;
            this.model = model;
        }
        public void DisableTextBoxes()
        {
            foreach (var item in Attributes)
            {
                item.IsEnabled = false;
            }
            foreach (var item in Functions)
            {
                item.IsEnabled = false;
            }
            foreach (var item in Enumerations)
            {
                item.IsEnabled = false;
            }
            Name.IsEnabled = false;
        }
        public void EnableTextBoxes()
        {
            foreach (var item in Attributes)
            {
                item.IsEnabled = true;
            }
            foreach (var item in Functions)
            {
                item.IsEnabled = true;
            }
            foreach (var item in Enumerations)
            {
                item.IsEnabled = true;
            }
            Name.IsEnabled = true;
        }
        public virtual void Draw(Canvas canvas)
        {
            GenerateCommon();
            Border border = new Border();
            StackPanel panel = new StackPanel();
            panel.MouseLeftButtonDown += Border_MouseLeftButtonDown;
            border.Child = panel;
            border.BorderBrush = Brushes.DarkRed;
            border.BorderThickness = new Thickness(2);
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
            Canvas.SetLeft(border, node.Position.X - node.Width / 2);
            Canvas.SetTop(border, node.Position.Y - node.Height / 2);
            canvas.Children.Add(border);
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (RelationshipCreator.NodeClicked(((NamedElementBuilder)node.NodeObject).Name.ToString())) { 
                
                model.StartDrawingLine(e);
                model.Refresh();
            }
            else
            {
                model.EndDrawingLine(e);
                model.Refresh();
            }

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
            menuitem.Click += (e, er) => { model.RemoveElement((ElementBuilder)node.NodeObject); model.Refresh(); 
            };
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
    }
}
