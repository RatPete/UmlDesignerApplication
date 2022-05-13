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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.Source.PRL.ViewModel;
using WpfDiagramDesigner.ViewModel;
using WpfDiagramDesigner.Views;

namespace WpfDiagramDesigner.Objects
{
    public abstract class Node : DiagramElement
    {

        protected NodeLayout node;
        protected readonly ViewModel.IRefreshable model;
        private System.Windows.Point currentPosition;

        protected List<TextBox> Attributes { set; get; } = new List<TextBox>();
        protected List<TextBox> Functions { get; set; } = new List<TextBox>();
        protected TextBox Name { get; set; }
        protected List<TextBox> Enumerations { get; set; } = new List<TextBox>();

        public string Id { get { return ((NamedElementBuilder)node.NodeObject).Name; } }

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

     

        Border border;
        protected StackPanel attributePanel;
        protected StackPanel functionPanel;
        protected StackPanel enumPanel;
        List<Line> lines = new List<Line>();
        public virtual void Draw(Canvas canvas)
        {
            GenerateCommon();
            border = new Border();
            StackPanel itemPanel = new StackPanel();
            itemPanel.MouseLeftButtonDown += Border_MouseLeftButtonDown;
            border.Child = itemPanel;
            border.BorderBrush = Brushes.DarkRed;
            border.BorderThickness = new Thickness(2);
            itemPanel.Children.Add(Name);
            var item = new Line() { Stroke = Brushes.DarkRed, StrokeThickness = 2, X1 = 0, X2 = node.Width - 3 };
            lines.Add(item);
            
            itemPanel.Children.Add(item);
            attributePanel = new StackPanel();
            itemPanel.Children.Add(attributePanel);
            item = new Line() { Stroke = Brushes.DarkRed, StrokeThickness = 2, X1 = 0, X2 = node.Width - 3 };
            itemPanel.Children.Add(item);
            lines.Add(item);
            functionPanel = new StackPanel();
            itemPanel.Children.Add(functionPanel);
            enumPanel = new StackPanel();
            itemPanel.Children.Add(enumPanel);
            foreach (var element in Attributes)
                attributePanel.Children.Add(element);
            
            foreach (var element in Functions)
                functionPanel.Children.Add(element);
            foreach (var element in Enumerations)
                enumPanel.Children.Add(element);
            itemPanel.Background = Brushes.LightYellow;
            enumPanel.Background = Brushes.LightYellow;
            functionPanel.Background = Brushes.LightYellow;
            attributePanel.Background = Brushes.LightYellow;
            Canvas.SetLeft(border, node.Position.X - node.Width / 2);
            Canvas.SetTop(border, node.Position.Y - node.Height / 2);
            currentPosition.X = node.Position.X - node.Width / 2;
            currentPosition.Y = node.Position.Y - node.Height / 2;
            canvas.Children.Add(border);
        }

       

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (RelationshipCreator.NodeClicked(((NamedElementBuilder)node.NodeObject).Name.ToString()))
            {

                model.StartDrawingLine(e);
                model.Refresh();
            }
            else
            {
                model.EndDrawingLine(e);
                model.Refresh();
            }

        }
        public abstract void AddFunction();

        public abstract void RemoveFunction(TextBox item, OperationBuilder operationBuilder);
        public abstract void RemoveAttribute(TextBox item, PropertyBuilder attributeBuilder);

        public abstract void AddAttribute();
        public abstract  void AddLiteral();


        public abstract  void RemoveLiteral(TextBox item, EnumerationLiteralBuilder literal);
   

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
            menuitem.Click += (e, er) =>
            {
                model.RemoveElement((ElementBuilder)node.NodeObject); model.Refresh();
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

        public void AnimateObject(AnimationValues a, Storyboard storyboard)
        {
            var nodeAnim = (NodeAnimationValues)a;
            DoubleAnimation left = new DoubleAnimation
            {
                From = currentPosition.X,
                To = nodeAnim.TargetPosition.X - nodeAnim.TargetSize.Width / 2.0f,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(left, border);
            Storyboard.SetTargetProperty(left, new PropertyPath(Canvas.LeftProperty));
            storyboard.Children.Add(left);
            currentPosition.X = nodeAnim.TargetPosition.X - nodeAnim.TargetSize.Width / 2.0;
            DoubleAnimation top = new DoubleAnimation
            {
                From = currentPosition.Y,
                To = nodeAnim.TargetPosition.Y - nodeAnim.TargetSize.Height / 2.0f,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
            };
            currentPosition.Y = nodeAnim.TargetPosition.Y - nodeAnim.TargetSize.Height / 2.0f;
            
            Storyboard.SetTarget(top, border);
            Storyboard.SetTargetProperty(top, new PropertyPath(Canvas.TopProperty));
            storyboard.Children.Add(top);
            foreach (var line in lines)
            {
                DoubleAnimation lineX2 = new DoubleAnimation
                {
                    From = line.X2,
                    To = nodeAnim.TargetSize.Width - 3,
                    Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2))
                };
                Storyboard.SetTarget(lineX2, line);
                Storyboard.SetTargetProperty(lineX2, new PropertyPath(Line.X2Property));
                storyboard.Children.Add(lineX2);
            }
        }

        public void RemoveFromCanvas(Canvas canvas)
        {
            canvas.Children.Remove(border);
        }
    }
}
