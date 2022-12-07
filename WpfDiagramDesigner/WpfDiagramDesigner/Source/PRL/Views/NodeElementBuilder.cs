using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDiagramDesigner.Objects;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.Source.PRL.Views;

namespace WpfDiagramDesigner.Views
{
    public static class NodeElementBuilder
    {
        
        private static void StyleTexbox(TextBox tb)
        {
            tb.Foreground = Brushes.Black;
            tb.FontSize = 8.0;
            tb.Padding = new System.Windows.Thickness(0);
            tb.Background = Brushes.Transparent;
            tb.BorderBrush = Brushes.Transparent;
            tb.BorderThickness = new System.Windows.Thickness(0);
        }
        public static TextBox FunctionBuilder(MetaDslx.Languages.Uml.Model.OperationBuilder item, Objects.Node element, ViewModel.IRefreshable model)
        {
            TextBox tb = new TextBox();
            tb.Text=UMLReader.UmlReader.CreateFunctionText(item);
            StyleTexbox(tb);
            tb.LostFocus += (e, er) =>
            {
                try
                {
                    InlineParser.CanParseFunction(tb.Text);
                }
                catch (ObjectNotParsableException exception)
                {
                    //TODO show dialog for exception
                    ErrorPopup popup = new ErrorPopup(exception.Message,tb.PointToScreen(new System.Windows.Point(0,0)));
                    var result = popup.ShowDialog();
                    if (result.HasValue && result.Value)
                    {
                        model.RefocusElement(tb);
                    }
                    else
                    {
                        tb.Text = UMLReader.UmlReader.CreateFunctionText(item);
                    }

                    return;
                }
                InlineParser.FunctionParse(tb.Text, item);
                tb.Text=UMLReader.UmlReader.CreateFunctionText(item);
                StyleTexbox(tb);
                model.Refresh();
            };
            tb.ContextMenu = new ContextMenu();
            tb.ContextMenu.Items.Clear();
            var menuitem = new MenuItem();
            menuitem.Header = "Remove";
            menuitem.Click += (e, er) =>
            {
                element.RemoveFunction(tb,item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem();
            menuitem.Header = "Add";
            menuitem.Click += (e, er) =>
            {
                
                element.AddFunction();
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);


            return tb;
        }
        public static TextBox AttributeBuilder(PropertyBuilder item, Objects.Node element, ViewModel.IRefreshable model)
        {
            var tb = new TextBox();
            tb.Text=UMLReader.UmlReader.CreateAttributeText( item);
            StyleTexbox(tb);
            tb.LostFocus += (e, er) =>
            {
                try
                {
                    InlineParser.CanParseAttribute(tb.Text);
                }
                catch(ObjectNotParsableException exception)
                {
                    //TODO show dialog for exception
                    ErrorPopup popup = new ErrorPopup(exception.Message, tb.PointToScreen(new System.Windows.Point(0, 0)));
                    var result = popup.ShowDialog();
                    if (result.HasValue && result.Value)
                    {
                        model.RefocusElement(tb);
                    }
                    else
                    {
                        tb.Text = UMLReader.UmlReader.CreateAttributeText(item);
                    }
                    return;
                }
                InlineParser.AttributeParse(tb.Text, item);
                tb.Text=UMLReader.UmlReader.CreateAttributeText( item);

                model.Refresh();
            };
            tb.ContextMenu = new ContextMenu();
            tb.ContextMenu.Items.Clear();
            var menuitem = new MenuItem();
            menuitem.Header = "Remove";
            menuitem.Click += (e, er) =>
            {
                //tb.Text=UMLReader.UmlReader.CreateAttributeText( item);
                element.RemoveAttribute(tb,item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem();
            menuitem.Header = "Add";
            menuitem.Click += (e, er) =>
            {
                element.AddAttribute();
                //tb.Text= UMLReader.UmlReader.CreateAttributeText( item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            return tb;
        }

        public static TextBox EnumBuilder(EnumerationLiteralBuilder enumerationLiteral,Objects.Node element, ViewModel.IRefreshable model)
        {
            var tb = new TextBox();
            StyleTexbox(tb);
            tb.Text= CreateEnumText( enumerationLiteral);
            tb.LostFocus += (e, er) =>
            {
                try
                {
                    if (InlineParser.CanParseEnum(tb.Text))
                    {
                        enumerationLiteral.Name = tb.Text;

                        tb.Text = enumerationLiteral.Name;
                        StyleTexbox(tb);
                        model.Refresh();
                    }
                    else
                    {
                        tb.Text = enumerationLiteral.Name;
                    }
                }
                catch (ObjectNotParsableException exception)
                {
                    //TODO show dialog for exception
                    ErrorPopup popup = new ErrorPopup(exception.Message, tb.PointToScreen(new System.Windows.Point(0, 0)));
                    var result=popup.ShowDialog();
                    if (result.HasValue && result.Value)
                    {

                        model.RefocusElement(tb);
                    }
                    else
                    {
                        tb.Text = CreateEnumText(enumerationLiteral);
                    }
                    return;
                }
                InlineParser.EnumParser(tb.Text, enumerationLiteral);
                tb.Text=CreateEnumText( enumerationLiteral);
                model.Refresh();
            };
            tb.ContextMenu = new ContextMenu();

            var menuitem = new MenuItem { Header = "Remove" };
            menuitem.Click += (e, er) =>
            {
                element.RemoveLiteral(tb,enumerationLiteral);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem();
            menuitem.Header = "Add";
            menuitem.Click += (e, er) =>
            {
                element.AddLiteral();
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);

            return tb;
        }

        private static string CreateEnumText( EnumerationLiteralBuilder enumerationLiteral)
        {
            return enumerationLiteral.Name;
        }
    }
}